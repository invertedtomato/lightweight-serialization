using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;

namespace InvertedTomato.Serialization.LightWeightSerialization {
    public class LightWeight : ISerializer {
        private readonly object Sync = new object();
        private readonly Dictionary<Type, Delegate> Encoders = new Dictionary<Type, Delegate>(); // Func<T, TIn>
        private readonly Dictionary<Type, Delegate> Decoders = new Dictionary<Type, Delegate>(); // Func<TOut, T>
        private readonly VLQCodec VLQ = new VLQCodec();
        private readonly ModuleBuilder DynamicModule;

        public LightWeight() {
            // Prepare assembly for dynamic serilizers/deserilizers
            var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicCoders"), AssemblyBuilderAccess.Run);
            DynamicModule = dynamicAssembly.DefineDynamicModule("DynamicCoders");
        }

        public void PrepareFor<T>() {
            Encoders[typeof(T)] = GenerateEncoderFor<T>();
            Decoders[typeof(T)] = GenerateDecoderFor<T>();
        }

        public void Encode<T>(T value, Buffer<byte> buffer) {
#if DEBUG
            if (null == buffer) {
                throw new ArgumentNullException(nameof(buffer));
            }
#endif


            // Get root serilizer
            var rootSerilizer = (Func<T, ScatterTreeBuffer>)GetEncoder<T>();

            // Invoke root serilizer
            var output = rootSerilizer(value);

            // Grow buffer with sufficent room
            buffer.Grow(Math.Max(0, output.Length + output.Count * 10 - buffer.Writable));

            // Squash scatter tree into buffer
            Squash(output, buffer);
        }
        public T Decode<T>(Buffer<byte> input) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException(nameof(input));
            }
#endif

            // Get root serilizer
            var root = GetDecoder<T>();

            // Invoke root serilizer
            return (T)root.DynamicInvoke(input);
        }

        protected Delegate GetEncoder<T>() {
            lock (Sync) {
                // If there's no coder, build one
                if (!Encoders.TryGetValue(typeof(T), out var encoder)) {
                    PrepareFor<T>();
                    encoder = Encoders[typeof(T)];
                }

                // Return coder
                return encoder;
            }
        }
        protected Delegate GetEncoder(Type type) {
            var method = GetType().GetRuntimeMethods().Single(a => a.Name == nameof(GetEncoder) && a.IsGenericMethod).MakeGenericMethod(type);
            return (Delegate)method.Invoke(this, null);
        }

        protected Delegate GetDecoder<T>() {
            lock (Sync) {
                // If there's no coder, build one
                if (!Decoders.TryGetValue(typeof(T), out var decoder)) {
                    PrepareFor<T>();
                    decoder = Decoders[typeof(T)];
                }

                // Return coder
                return decoder;
            }
        }
        protected Delegate GetDecoder(Type type) {
            var method = GetType().GetRuntimeMethods().Single(a => a.Name == nameof(GetDecoder) && a.IsGenericMethod).MakeGenericMethod(type); // "typeof(LightWeight).GetRuntimeMethod(nameof(EnsureSerializer), new Type[] { })" returns NULL for generics - why?
            return (Delegate)method.Invoke(this, null);
        }

        private void Squash(ScatterTreeBuffer output, Buffer<byte> buffer) {
            if (output.Payload != null) {
                buffer.EnqueueArray(output.Payload);
            } else {
                foreach (var child in output.Children) {
                    VLQ.CompressUnsigned((ulong)child.Length, buffer);
                    Squash(child, buffer);
                }
            }
        }

        protected Delegate GenerateEncoderFor<T>() {
            if (typeof(T) == typeof(bool)) {
                return GenerateBoolEncoder();
            }

            if (typeof(T) == typeof(sbyte)) {
                return GenerateSInt8Encoder();
            }
            if (typeof(T) == typeof(short)) {
                return GenerateSInt16Encoder();
            }
            if (typeof(T) == typeof(int)) {
                return GenerateSInt32Encoder();
            }
            if (typeof(T) == typeof(long)) {
                return GenerateSInt64Encoder();
            }

            if (typeof(T) == typeof(byte)) {
                return GenerateUInt8Encoder();
            }
            if (typeof(T) == typeof(ushort)) {
                return GenerateUInt16Encoder();
            }
            if (typeof(T) == typeof(uint)) {
                return GenerateUInt32Encoder();
            }
            if (typeof(T) == typeof(ulong)) {
                return GenerateUInt64Encoder();
            }

            if (typeof(T) == typeof(string)) {
                return GenerateStringEncoder();
            }

            if (typeof(T).IsArray) {
                return GenerateArrayEncoder<T>();
            }
            if (typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T))) {
                return GenerateListEncoder<T>();
            }
            if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T))) {
                return GenerateDictionaryEncoder<T>();
            }

            if (typeof(T).GetTypeInfo().IsClass) {
                return GeneratePOCOEncoder<T>();
            }

            throw new NotSupportedException();
        }
        protected Delegate GenerateDecoderFor<T>() {
            if (typeof(T) == typeof(bool)) {
                return GenerateBoolDecoder();
            }

            if (typeof(T) == typeof(sbyte)) {
                return GenerateSInt8Decoder();
            }
            if (typeof(T) == typeof(short)) {
                return GenerateSInt16Decoder();
            }
            if (typeof(T) == typeof(int)) {
                return GenerateSInt32Decoder();
            }
            if (typeof(T) == typeof(long)) {
                return GenerateSInt64Decoder();
            }

            if (typeof(T) == typeof(byte)) {
                return GenerateUInt8Decoder();
            }
            if (typeof(T) == typeof(ushort)) {
                return GenerateUInt16Decoder();
            }
            if (typeof(T) == typeof(uint)) {
                return GenerateUInt32Decoder();
            }
            if (typeof(T) == typeof(ulong)) {
                return GenerateUInt64Decoder();
            }

            if (typeof(T) == typeof(string)) {
                return GenerateStringDecoder();
            }

            if (typeof(T).IsArray) {
                return GenerateArrayDecoder<T>();
            }
            if (typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T))) {
                return GenerateListDecoder<T>();
            }
            if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T))) {
                return GenerateDictionaryDecoder<T>();
            }

            if (typeof(T).GetTypeInfo().IsClass) {
                return GeneratePOCODecoder<T>();
            }

            throw new NotSupportedException();
        }

        private Func<bool, ScatterTreeBuffer> GenerateBoolEncoder() {
            return (value) => {
                if (value) {
                    return new ScatterTreeBuffer(new byte[] { 0x00 });
                } else {
                    return new ScatterTreeBuffer(new byte[] { });
                }
            };
        }
        private Func<Buffer<byte>, bool> GenerateBoolDecoder() {
            return (buffer) => {
                if (buffer.Readable == 0) {
                    return false;
                }
#if DEBUG
                if (buffer.Readable > 1) {
                    throw new DataFormatException("Boolean values can be no more than 1 byte long.");
                }
#endif
                if (buffer.Dequeue() != 0x00) {
                    throw new DataFormatException("Boolean values cannot be anything other than 0x00.");
                }

                return true;
            };
        }

        private Func<sbyte, ScatterTreeBuffer> GenerateSInt8Encoder() {
            return (value) => {
                if (value == 0) {
                    return ScatterTreeBuffer.Empty;
                } else {
                    return new ScatterTreeBuffer(new byte[] { (byte)value });
                }
            };
        }
        private Func<Buffer<byte>, sbyte> GenerateSInt8Decoder() {
            return (buffer) => {
                switch (buffer.Readable) {
                    case 0: return 0;
                    case 1: return (sbyte)buffer.Dequeue();
                    default: throw new DataFormatException("SInt64 values can be 0 or 1 bytes.");
                }
            };
        }

        private Func<short, ScatterTreeBuffer> GenerateSInt16Encoder() {
            var smaller = GenerateSInt8Encoder();
            return (value) => {
                if (value <= sbyte.MaxValue && value >= sbyte.MinValue) {
                    return smaller((sbyte)value);
                } else {
                    return new ScatterTreeBuffer(BitConverter.GetBytes(value));
                }
            };
        }
        private Func<Buffer<byte>, short> GenerateSInt16Decoder() {
            return (buffer) => {
                switch (buffer.Readable) {
                    case 0: return 0;
                    case 1: return buffer.Dequeue();
                    case 2: return BitConverter.ToInt16(buffer.GetUnderlying(), buffer.Start);
                    default: throw new DataFormatException("SInt64 values can be 0, 1 or 2 bytes.");
                }
            };
        }

        private Func<int, ScatterTreeBuffer> GenerateSInt32Encoder() {
            var smaller = GenerateSInt16Encoder();
            return (value) => {
                if (value <= short.MaxValue && value >= short.MinValue) {
                    return smaller((short)value);
                } else { // TODO: 3-byte encoding
                    return new ScatterTreeBuffer(BitConverter.GetBytes(value));
                }
            };
        }
        private Func<Buffer<byte>, int> GenerateSInt32Decoder() {
            return (buffer) => {
                switch (buffer.Readable) {
                    case 0: return 0;
                    case 1: return buffer.Dequeue();
                    case 2: return BitConverter.ToInt16(buffer.GetUnderlying(), buffer.Start);
                    // TODO: 3
                    case 4: return BitConverter.ToInt32(buffer.GetUnderlying(), buffer.Start);
                    default: throw new DataFormatException("SInt32 values can be 0, 1, 2 or 4 bytes.");
                }
            };
        }

        private Func<long, ScatterTreeBuffer> GenerateSInt64Encoder() {
            var smaller = GenerateSInt32Encoder();
            return (value) => {
                if (value <= int.MaxValue && value >= int.MinValue) {
                    return smaller((int)value);
                } else { // TODO: 5, 6, 7 byte encoding
                    return new ScatterTreeBuffer(BitConverter.GetBytes(value));
                }
            };
        }
        private Func<Buffer<byte>, long> GenerateSInt64Decoder() {
            return (buffer) => {
                switch (buffer.Readable) {
                    case 0: return 0;
                    case 1: return buffer.Dequeue();
                    case 2: return BitConverter.ToInt16(buffer.GetUnderlying(), buffer.Start);
                    // TODO: 3
                    case 4: return BitConverter.ToInt32(buffer.GetUnderlying(), buffer.Start);
                    // TODO 5,6,7
                    case 8: return BitConverter.ToInt64(buffer.GetUnderlying(), buffer.Start);
                    default: throw new DataFormatException("SInt64 values can be 0, 1, 2, 4 or 8 bytes.");
                }
            };
        }

        private Func<byte, ScatterTreeBuffer> GenerateUInt8Encoder() {
            return (value) => {
                if (value == 0) {
                    return ScatterTreeBuffer.Empty;
                } else {
                    return new ScatterTreeBuffer(new byte[] { value });
                }
            };
        }
        private Func<Buffer<byte>, byte> GenerateUInt8Decoder() {
            return (buffer) => {
                switch (buffer.Readable) {
                    case 0: return 0;
                    case 1: return buffer.Dequeue();
                    default: throw new DataFormatException("UInt64 values can be 0 or 1 bytes.");
                }
            };
        }

        private Func<ushort, ScatterTreeBuffer> GenerateUInt16Encoder() {
            var smaller = GenerateUInt8Encoder();
            return (value) => {
                if (value <= byte.MaxValue) {
                    return smaller((byte)value);
                } else {
                    return new ScatterTreeBuffer(BitConverter.GetBytes(value));
                }
            };
        }
        private Func<Buffer<byte>, ushort> GenerateUInt16Decoder() {
            return (buffer) => {
                switch (buffer.Readable) {
                    case 0: return 0;
                    case 1: return buffer.Dequeue();
                    case 2: return BitConverter.ToUInt16(buffer.GetUnderlying(), buffer.Start);
                    default: throw new DataFormatException("UInt64 values can be 0, 1 or 2 bytes.");
                }
            };
        }

        private Func<uint, ScatterTreeBuffer> GenerateUInt32Encoder() {
            var smaller = GenerateUInt16Encoder();
            return (value) => {
                if (value <= ushort.MaxValue) {
                    return smaller((ushort)value);
                } else {// TODO: 3 byte encoding
                    return new ScatterTreeBuffer(BitConverter.GetBytes(value));
                }
            };
        }
        private Func<Buffer<byte>, uint> GenerateUInt32Decoder() {
            return (buffer) => {
                switch (buffer.Readable) {
                    case 0: return 0;
                    case 1: return buffer.Dequeue();
                    case 2: return BitConverter.ToUInt16(buffer.GetUnderlying(), buffer.Start);
                    // TODO: 3
                    case 4: return BitConverter.ToUInt32(buffer.GetUnderlying(), buffer.Start);
                    default: throw new DataFormatException("UInt32 values can be 0, 1, 2 or 4 bytes.");
                }
            };
        }

        private Func<ulong, ScatterTreeBuffer> GenerateUInt64Encoder() {
            var smaller = GenerateUInt32Encoder();
            return (value) => {
                if (value <= uint.MaxValue) {
                    return smaller((uint)value);
                } else { // TODO: 5, 6, 7 byte encoding
                    return new ScatterTreeBuffer(BitConverter.GetBytes(value));
                }
            };
        }
        private Func<Buffer<byte>, ulong> GenerateUInt64Decoder() {
            return (buffer) => {
                switch (buffer.Readable) {
                    case 0: return 0;
                    case 1: return buffer.Dequeue();
                    case 2: return BitConverter.ToUInt16(buffer.GetUnderlying(), buffer.Start);
                    // TODO: 3
                    case 4: return BitConverter.ToUInt32(buffer.GetUnderlying(), buffer.Start);
                    // TODO 5,6,7
                    case 8: return BitConverter.ToUInt64(buffer.GetUnderlying(), buffer.Start);
                    default: throw new DataFormatException("UInt64 values can be 0, 1, 2, 4 or 8 bytes.");
                }
            };
        }

        private Func<string, ScatterTreeBuffer> GenerateStringEncoder() {
            return (value) => {
                if (null == value) {
                    return ScatterTreeBuffer.Empty;
                } else {
                    return new ScatterTreeBuffer(Encoding.UTF8.GetBytes(value));
                }
            };
        }
        private Func<Buffer<byte>, string> GenerateStringDecoder() {
            return (buffer) => {
                return Encoding.UTF8.GetString(buffer.GetUnderlying(), buffer.Start, buffer.Readable);
            };
        }

        private Func<Array, ScatterTreeBuffer> GenerateArrayEncoder<T>() {
            // Get serilizer for sub items
            var valueEncoder = GetEncoder(typeof(T).GetElementType());

            return (value) => {
                // Handle nulls
                if (null == value) {
                    return ScatterTreeBuffer.Empty;
                }

                // Serialize elements
                var pos = 0;
                var result = new ScatterTreeBuffer[value.Length];
                foreach (var element in value) {
                    result[pos++] = (ScatterTreeBuffer)valueEncoder.DynamicInvoke(element);
                }

                return new ScatterTreeBuffer(result);
            };
        }
        private Func<Buffer<byte>, Array> GenerateArrayDecoder<T>() {
            // Get deserilizer for sub items
            var valueDecoder = GetDecoder(typeof(T).GetElementType());

            return (buffer) => {
                // Instantiate list
                var container = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(typeof(T).GetElementType()));

                // Deserialize until we reach length limit
                while (buffer.IsReadable) {
                    // Extract length
                    var length = (int)VLQ.DecompressUnsigned(buffer);

                    // Extract subbuffer
                    var subBuffer = buffer.DequeueBuffer(length);

                    // Deserialize element
                    var element = valueDecoder.DynamicInvoke(subBuffer);

                    // Add to output
                    container.Add(element);
                }

                // Convert to array and return
                var output = Array.CreateInstance(typeof(T).GetElementType(), container.Count);
                container.CopyTo(output, 0);

                return output;
            };
        }

        private Func<IList, ScatterTreeBuffer> GenerateListEncoder<T>() {
            // Get serilizer for sub items
            var valueEncoder = GetEncoder(typeof(T).GenericTypeArguments[0]);

            return (value) => {
                // Handle nulls
                if (null == value) {
                    return ScatterTreeBuffer.Empty;
                }

                // Serialize elements
                var pos = 0;
                var result = new ScatterTreeBuffer[value.Count];
                foreach (var element in value) {
                    result[pos++] = (ScatterTreeBuffer)valueEncoder.DynamicInvoke(element);
                }

                return new ScatterTreeBuffer(result);
            };
        }
        private Func<Buffer<byte>, T> GenerateListDecoder<T>() {
            // Get deserilizer for sub items
            var valueDecoder = GetDecoder(typeof(T).GenericTypeArguments[0]);

            return (buffer) => {
                // Instantiate list
                var output = (IList)Activator.CreateInstance(typeof(T));//typeof(List<>).MakeGenericType(type.GenericTypeArguments)

                // Deserialize until we reach length limit
                while (buffer.IsReadable) {
                    // Extract length
                    var length = (int)VLQ.DecompressUnsigned(buffer);

                    // Extract subbuffer
                    var subBuffer = buffer.DequeueBuffer(length);

                    // Deserialize element
                    var element = valueDecoder.DynamicInvoke(subBuffer);

                    // Add to output
                    output.Add(element);
                }

                return (T)output;
            };
        }

        private Func<IDictionary, ScatterTreeBuffer> GenerateDictionaryEncoder<T>() {
            // Get serilizer for sub items
            var keyEncoder = GetEncoder(typeof(T).GenericTypeArguments[0]);
            var valueEncoder = GetEncoder(typeof(T).GenericTypeArguments[1]);

            return (value) => {
                // Handle nulls
                if (null == value) {
                    return ScatterTreeBuffer.Empty;
                }

                // Serialize elements   
                var pos = 0;
                var result = new ScatterTreeBuffer[value.Count * 2];
                var e = value.GetEnumerator();
                while (e.MoveNext()) {
                    result[pos++] = (ScatterTreeBuffer)keyEncoder.DynamicInvoke(e.Key);
                    result[pos++] = (ScatterTreeBuffer)valueEncoder.DynamicInvoke(e.Value);
                }

                return new ScatterTreeBuffer(result);
            };
        }
        private Func<Buffer<byte>, IDictionary> GenerateDictionaryDecoder<T>() {
            // Get deserilizer for sub items
            var keyDecoder = GetDecoder(typeof(T).GenericTypeArguments[0]);
            var valueDecoder = GetDecoder(typeof(T).GenericTypeArguments[1]);

            return (buffer) => {
                // Instantiate dictionary
                var output = (IDictionary)Activator.CreateInstance(typeof(T));

                // Loop through input buffer until depleated
                while (buffer.IsReadable) {
                    // Deserialize key
                    var keyLength = (int)VLQ.DecompressUnsigned(buffer);
                    var keyBuffer = buffer.DequeueBuffer(keyLength);
                    var keyValue = keyDecoder.DynamicInvoke(keyBuffer);

                    // Deserialize value
                    var valueLength = (int)VLQ.DecompressUnsigned(buffer);
                    var valueBuffer = buffer.DequeueBuffer(valueLength);
                    var valueValue = valueDecoder.DynamicInvoke(valueBuffer);

                    // Add to output
                    output[keyValue] = valueValue;
                }

                return output;
            };
        }

        private Func<T, ScatterTreeBuffer> GeneratePOCOEncoder<T>() {
            // Create method
            var name = typeof(T).Name + "_Serilizer";
            var newType = DynamicModule.DefineType(name, TypeAttributes.Public);
            var newMethod = newType.DefineMethod(name, MethodAttributes.Static | MethodAttributes.Public, typeof(ScatterTreeBuffer), new Type[] { typeof(T) });

            // Add  IL
            var il = newMethod.GetILGenerator();

            il.DeclareLocal(typeof(int)); //index
            il.DeclareLocal(typeof(long)); //pre-length

            var notNullLabel = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_0); // value
            il.Emit(OpCodes.Brtrue, notNullLabel);
            il.Emit(OpCodes.Ldarg_1); // output
            il.Emit(OpCodes.Ldc_I4_S, 0x80);
            //il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.AddRaw), new Type[] { typeof(byte) }));
            il.Emit(OpCodes.Ret);

            il.MarkLabel(notNullLabel);
            il.Emit(OpCodes.Ldarg_1); // output
            //il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.Allocate), new Type[] { }));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_1); // output
            //il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeProperty(nameof(SerializationOutput.Length)).GetMethod);
            il.Emit(OpCodes.Stloc_1);

            var properties = new Dictionary<byte, FieldInfo>();
            foreach (var itm in typeof(T).GetRuntimeFields()) {
                // Get property attribute which tells us the properties' index
                var attribute = (LightWeightPropertyAttribute)itm.GetCustomAttribute(typeof(LightWeightPropertyAttribute), false);
                if (null == attribute) {
                    // No attribute found, skip
                    continue;
                }

                properties.Add(attribute.Index, itm);

            }
            if (properties.Count > 0) {
                var maxval = properties.Keys.Max();
                for (byte i = 0; i <= maxval; i++) {
                    if (properties.TryGetValue(i, out var itm)) {
                        // Get the serializer for the subitem
                        var subType = GetEncoder(itm.FieldType);

                        // Get it's method info
                        var subMethodInfo = subType.GetMethodInfo();

                        il.Emit(OpCodes.Ldarg_0); // value
                        il.Emit(OpCodes.Ldfld, itm);
                        il.Emit(OpCodes.Ldarg_1); // output
                        il.Emit(OpCodes.Call, subMethodInfo);
                    } else {
                        //output.AddRaw(0x80);
                        il.Emit(OpCodes.Ldarg_1);           // P1: output
                        il.Emit(OpCodes.Ldc_I4_S, 0x80);    // P2: 0x80
                        //il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.AddRaw), new Type[] { typeof(byte) }));
                    }
                }
            }

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldarg_1);
            //il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeProperty(nameof(SerializationOutput.Length)).GetMethod);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Sub);
            //il.Emit(OpCodes.Call, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.SetVLQ), new Type[] { typeof(int), typeof(ulong) }));
            il.Emit(OpCodes.Ret);

            // Add to serilizers
            var methodInfo = newType.CreateTypeInfo().GetMethod(name);
            return (Func<T, ScatterTreeBuffer>)methodInfo.CreateDelegate(typeof(Func<T, ScatterTreeBuffer>));

        }
        private Func<Buffer<byte>, T> GeneratePOCODecoder<T>() {
            // Build vector of property types
            var a = new FieldInfo[byte.MaxValue];
            foreach (var field in typeof(T).GetRuntimeFields()) { // TODO: Add property support
                                                                  // Get property attribute which tells us the properties' index
                var attribute = (LightWeightPropertyAttribute)field.GetCustomAttribute(typeof(LightWeightPropertyAttribute));

                // Skip if not found, or index doesn't match
                if (null != attribute) {
                    // TODO: check for dupes
                    a[attribute.Index] = field;
                    continue;
                }
            }

            return (buffer) => {
                // Instantiate output
                var output = (T)Activator.CreateInstance(typeof(T));

                // Prepare for object deserialization
                var index = -1;

                // Attempt to read field length, if we've reached the end of the payload, abort
                while (buffer.IsReadable) {
                    // Get the length in a usable format
                    var length = (int)VLQ.DecompressUnsigned(buffer);
                    var subBuffer = buffer.DequeueBuffer(length);

                    // Increment the index
                    index++;

                    if (a[index] != null) {
                        // Get deserilizer
                        var deserializer = GetDecoder(a[index].FieldType);

                        // Deserialize value
                        var value = deserializer.DynamicInvoke(subBuffer);

                        // Set it on property
                        a[index].SetValue(output, value);
                    }
                }

                return output;
            };
        }


        public static byte[] Serialize<T>(T value) {
            var buffer = new Buffer<byte>(0);
            var lw = new LightWeight();
            lw.Encode(value, buffer);
            return buffer.ToArray();
        }
        public static T Deserialize<T>(byte[] payload) {
            var buffer = new Buffer<byte>(payload);
            var lw = new LightWeight();
            return lw.Decode<T>(buffer);
        }
    }
}
