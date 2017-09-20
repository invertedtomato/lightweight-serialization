﻿using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using InvertedTomato.Serialization.LightWeightSerialization.Coders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace InvertedTomato.Serialization.LightWeightSerialization {
    public class LightWeight : ISerializer {
        /// <summary>
        /// Serialize a value to a byte array.
        /// </summary>
        public static byte[] Serialize<T>(T value) {
            return Serialize(value, new LightWeightOptions());
        }

        /// <summary>
        /// Serialize a value to a byte array, with options.
        /// </summary>
        public static byte[] Serialize<T>(T value, LightWeightOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException(nameof(options));
            }
#endif

            var buffer = new Buffer<byte>(options.DefaultInitialBufferSize);
            var lw = new LightWeight(options);
            lw.Serialize(value, buffer);
            return buffer.ToArray();

        }

        /// <summary>
        /// Deserialize a value from a byte array.
        /// </summary>
        public static T Deserialize<T>(byte[] payload) {
            return Deserialize<T>(payload, new LightWeightOptions());
        }

        /// <summary>
        /// Deserialize a value from a byte array, with options.
        /// </summary>
        public static T Deserialize<T>(byte[] payload, LightWeightOptions options) {
#if DEBUG
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
#endif

            var lw = new LightWeight(options);
            return lw.Deserialize<T>(new Buffer<byte>(payload));
        }


        // TODO serilized buffer start in wrong position

        private readonly TypeInfo IListTypeInfo = typeof(IList).GetTypeInfo();
        private readonly TypeInfo IDictionaryTypeInfo = typeof(IDictionary).GetTypeInfo();
        private readonly LightWeightOptions Options;
        private readonly Dictionary<Type, Delegate> Serializers = new Dictionary<Type, Delegate>();
        private readonly Dictionary<Type, Delegate> Deserializers = new Dictionary<Type, Delegate>();
        private readonly ModuleBuilder DynamicModule;
        private readonly VLQCodec VLQ = new VLQCodec();

        public LightWeight(LightWeightOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            Options = options;

            // Load primative serilizers
            Serializers.Add(typeof(bool), (Action<bool, SerializationOutput>)BoolCoder.Serialize);
            Serializers.Add(typeof(sbyte), (Action<sbyte, SerializationOutput>)SInt8Coder.Serialize);
            Serializers.Add(typeof(short), (Action<short, SerializationOutput>)SInt16Coder.Serialize);
            Serializers.Add(typeof(int), (Action<int, SerializationOutput>)SInt32Coder.Serialize);
            Serializers.Add(typeof(long), (Action<long, SerializationOutput>)SInt64Coder.Serialize);
            Serializers.Add(typeof(byte), (Action<byte, SerializationOutput>)UInt8Coder.Serialize);
            Serializers.Add(typeof(ushort), (Action<ushort, SerializationOutput>)UInt16Coder.Serialize);
            Serializers.Add(typeof(uint), (Action<uint, SerializationOutput>)UInt32Coder.Serialize);
            Serializers.Add(typeof(ulong), (Action<ulong, SerializationOutput>)UInt64Coder.Serialize);
            Serializers.Add(typeof(string), (Action<string, SerializationOutput>)StringCoder.Serialize);

            // Load primative deserializers
            Deserializers.Add(typeof(bool), (Func<Buffer<byte>, bool>)BoolCoder.Deserialize);
            Deserializers.Add(typeof(sbyte), (Func<Buffer<byte>, sbyte>)SInt8Coder.Deserialize);
            Deserializers.Add(typeof(short), (Func<Buffer<byte>, short>)SInt16Coder.Deserialize);
            Deserializers.Add(typeof(int), (Func<Buffer<byte>, int>)SInt32Coder.Deserialize);
            Deserializers.Add(typeof(long), (Func<Buffer<byte>, long>)SInt64Coder.Deserialize);
            Deserializers.Add(typeof(byte), (Func<Buffer<byte>, byte>)UInt8Coder.Deserialize);
            Deserializers.Add(typeof(ushort), (Func<Buffer<byte>, ushort>)UInt16Coder.Deserialize);
            Deserializers.Add(typeof(uint), (Func<Buffer<byte>, uint>)UInt32Coder.Deserialize);
            Deserializers.Add(typeof(ulong), (Func<Buffer<byte>, ulong>)UInt64Coder.Deserialize);
            Deserializers.Add(typeof(string), (Func<Buffer<byte>, string>)StringCoder.Deserialize);

            // Prepare assembly for dynamic serilizers/deserilizers
            var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicCoders"), AssemblyBuilderAccess.Run);
            DynamicModule = dynamicAssembly.DefineDynamicModule("DynamicCoders");
        }

        /// <summary>
        /// Serialise a value to a given buffer.
        /// </summary>
        public void Serialize<T>(T value, Buffer<byte> buffer) {
#if DEBUG
            if (null == buffer) {
                throw new ArgumentNullException(nameof(buffer));
            }
#endif

            // Create output
            var output = new SerializationOutput();

            // Get root serilizer
            var rootSerilizer = (Action<T, SerializationOutput>)GetSerializer<T>();

            // Invoke root serilizer
            rootSerilizer(value, output);

            // Write to provided buffer
            output.Generate(buffer);
        }

        private Delegate GetSerializerBlind(Type type) {
            var method = typeof(LightWeight).GetRuntimeMethods().Single(a => a.Name == nameof(GetSerializer)).MakeGenericMethod(type); // "typeof(LightWeight).GetRuntimeMethod(nameof(EnsureSerializer), new Type[] { })" returns NULL for generics - why?
            return (Delegate)method.Invoke(this, null);
        }
        private Delegate GetSerializer<T>() {
            // If there's no coder, build one
            if (!Serializers.TryGetValue(typeof(T), out var serilizer)) {
                PrepareFor<T>();
                serilizer = Serializers[typeof(T)];
            }

            // Return coder
            return serilizer;
        }

        /// <summary>
        /// Deserialize a value from a buffer.
        /// </summary>
        public T Deserialize<T>(Buffer<byte> input) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException(nameof(input));
            }
#endif

            // Get root serilizer
            var rootDeserilizer = GetDeserializer<T>();

            // Invoke root serilizer
            return (T)rootDeserilizer.DynamicInvoke(input);
        }

        private Delegate GetDeserializerBlind(Type type) {
            var method = typeof(LightWeight).GetRuntimeMethods().Single(a => a.Name == nameof(GetDeserializer)).MakeGenericMethod(type); // "typeof(LightWeight).GetRuntimeMethod(nameof(EnsureSerializer), new Type[] { })" returns NULL for generics - why?
            return (Delegate)method.Invoke(this, null);
        }
        private Delegate GetDeserializer<T>() {
            // If there's no coder, build one
            if (!Deserializers.TryGetValue(typeof(T), out var deserilizer)) {
                PrepareFor<T>();
                deserilizer = Deserializers[typeof(T)];
            }

            // Return coder
            return deserilizer;
        }



        /// <summary>
        /// Have the system compile serializer/deserializers for the given type.
        /// </summary>
        /// <remarks>
        /// Useful to run in advance if you don't want first-call delays.
        /// </remarks>
        public void PrepareFor<T>() {
            if (typeof(T).IsArray) {
                PrepareForArray<T>();
                return;
            }
            var t = typeof(T).GetTypeInfo();
            if (IListTypeInfo.IsAssignableFrom(t)) {
                PrepareForList<T>();
                return;
            }
            if (IDictionaryTypeInfo.IsAssignableFrom(t)) {
                PrepareForDictionary<T>();
                return;
            }

            PrepareForPOCO<T>();
        }

        private void PrepareForArray<T>() {
            // Get serilizer for sub items
            var innerSerializer = GetSerializerBlind(typeof(T).GetElementType());

            Action<Array, SerializationOutput> serilizer = (value, output) => {
                if (null == value) {
                    output.AddRaw(VLQCodec.Nil);
                    return;
                }

                // Allocate space for a length header
                var allocateId = output.Allocate();
                var initialLength = output.Length;

                // Serialize elements
                foreach (var element in value) {
                    innerSerializer.DynamicInvoke(element, output);
                }

                // Set length header
                output.SetVLQ(allocateId, (ulong)(output.Length - initialLength));
            };
            Serializers[typeof(T)] = serilizer;

            // Get deserilizer for sub items
            var innerDeserializer = GetDeserializerBlind(typeof(T).GetElementType());

            Func<Buffer<byte>, Array> deserilizer = (buffer) => {
                // Instantiate list
                var container = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(typeof(T).GetElementType()));

                // Deserialize until we reach length limit
                while (buffer.IsReadable) {
                    // Extract length
                    var length = (int)VLQ.DecompressUnsigned(buffer);

                    // Extract subbuffer
                    var subBuffer = buffer.DequeueBuffer(length);

                    // Deserialize element
                    var element = innerDeserializer.DynamicInvoke(subBuffer);

                    // Add to output
                    container.Add(element);
                }

                // Convert to array and return
                var output = Array.CreateInstance(typeof(T).GetElementType(), container.Count);
                container.CopyTo(output, 0);

                return output;
            };
            Deserializers[typeof(T)] = deserilizer;
        }

        private void PrepareForList<T>() {
            // Get serilizer for sub items
            var innerSerializer = GetSerializerBlind(typeof(T).GenericTypeArguments[0]);

            Action<IList, SerializationOutput> serilizer = (value, output) => {
                // Allocate space for a length header
                var allocateId = output.Allocate();
                var initialLength = output.Length;

                // Serialize elements
                foreach (var element in value) {
                    innerSerializer.DynamicInvoke(element, output);
                }

                // Set length header
                output.SetVLQ(allocateId, (ulong)(output.Length - initialLength));
            };
            Serializers[typeof(T)] = serilizer;

            // Get deserilizer for sub items
            var innerDeserializer = GetDeserializerBlind(typeof(T).GenericTypeArguments[0]);

            Func<Buffer<byte>, T> deserilizer = (buffer) => {
                // Instantiate list
                var output = (IList)Activator.CreateInstance(typeof(T));//typeof(List<>).MakeGenericType(type.GenericTypeArguments)

                // Deserialize until we reach length limit
                while (buffer.IsReadable) {
                    // Extract length
                    var length = (int)VLQ.DecompressUnsigned(buffer);

                    // Extract subbuffer
                    var subBuffer = buffer.DequeueBuffer(length);

                    // Deserialize element
                    var element = innerDeserializer.DynamicInvoke(subBuffer);

                    // Add to output
                    output.Add(element);
                }

                return (T)output;
            };
            Deserializers[typeof(T)] = deserilizer;
        }

        private void PrepareForDictionary<T>() {
            // Get serilizer for sub items
            var innerKeySerializer = GetSerializerBlind(typeof(T).GenericTypeArguments[0]);
            var innerValueSerializer = GetSerializerBlind(typeof(T).GenericTypeArguments[1]);

            Action<IDictionary, SerializationOutput> serilizer = (value, output) => {
                // Allocate space for a length header
                var allocateId = output.Allocate();
                var initialLength = output.Length;

                // Serialize elements
                var e = value.GetEnumerator();
                while (e.MoveNext()) {
                    innerKeySerializer.DynamicInvoke(e.Key, output);
                    innerValueSerializer.DynamicInvoke(e.Value, output);
                }

                // Set length header
                output.SetVLQ(allocateId, (ulong)(output.Length - initialLength));
            };
            Serializers[typeof(T)] = serilizer;

            // Get deserilizer for sub items
            var innerKeyDeserializer = GetDeserializerBlind(typeof(T).GenericTypeArguments[0]);
            var innerValueDeserializer = GetDeserializerBlind(typeof(T).GenericTypeArguments[1]);

            Func<Buffer<byte>, IDictionary> deserilizer = (buffer) => {
                // Instantiate dictionary
                var output = (IDictionary)Activator.CreateInstance(typeof(T));

                // Loop through input buffer until depleated
                while (buffer.IsReadable) {
                    // Deserialize key
                    var keyLength = (int)VLQ.DecompressUnsigned(buffer);
                    var keyBuffer = buffer.DequeueBuffer(keyLength);
                    var keyValue = innerKeyDeserializer.DynamicInvoke(keyBuffer);

                    // Deserialize value
                    var valueLength = (int)VLQ.DecompressUnsigned(buffer);
                    var valueBuffer = buffer.DequeueBuffer(valueLength);
                    var valueValue = innerValueDeserializer.DynamicInvoke(valueBuffer);

                    // Add to output
                    output[keyValue] = valueValue;
                }

                return output;
            };
            Deserializers[typeof(T)] = deserilizer;
        }

        private void PrepareForPOCO<T>() {
            // Create method
            var name = typeof(T).Name + "_Serilizer";
            var newType = DynamicModule.DefineType(name, TypeAttributes.Public);
            var newMethod = newType.DefineMethod(name, MethodAttributes.Static | MethodAttributes.Public, null, new Type[] { typeof(T), typeof(SerializationOutput) });

            // Add  IL
            var il = newMethod.GetILGenerator();

            il.DeclareLocal(typeof(int)); //index
            il.DeclareLocal(typeof(long)); //pre-length

            var notNullLabel = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_0); // value
            il.Emit(OpCodes.Brtrue, notNullLabel);
            il.Emit(OpCodes.Ldarg_1); // output
            il.Emit(OpCodes.Ldc_I4_S, 0x80);
            il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.AddRaw), new Type[] { typeof(byte) }));
            il.Emit(OpCodes.Ret);

            il.MarkLabel(notNullLabel);
            il.Emit(OpCodes.Ldarg_1); // output
            il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.Allocate), new Type[] { }));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_1); // output
            il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeProperty(nameof(SerializationOutput.Length)).GetMethod);
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
                        var subType = GetSerializerBlind(itm.FieldType);

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
                        il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.AddRaw), new Type[] { typeof(byte) }));
                    }
                }
            }

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeProperty(nameof(SerializationOutput.Length)).GetMethod);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Sub);
            il.Emit(OpCodes.Call, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.SetVLQ), new Type[] { typeof(int), typeof(ulong) }));
            il.Emit(OpCodes.Ret);

            // Add to serilizers
            var methodInfo = newType.CreateTypeInfo().GetMethod(name);
            Serializers[typeof(T)] = (Action<T, SerializationOutput>)methodInfo.CreateDelegate(typeof(Action<T, SerializationOutput>));

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

            Func<Buffer<byte>, T> deserilizer = (buffer) => {
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
                        var deserializer = GetDeserializerBlind(a[index].FieldType);

                        // Deserialize value
                        var value = deserializer.DynamicInvoke(subBuffer);

                        // Set it on property
                        a[index].SetValue(output, value);
                    }
                }

                return output;
            };
            Deserializers[typeof(T)] = deserilizer;
        }
    }
}
