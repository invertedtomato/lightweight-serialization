using InvertedTomato.IO.Buffers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using InvertedTomato.Compression.Integers;
using System.Collections;

namespace InvertedTomato.LightWeightSerialization {
    public class LightWeight {
        private static readonly byte[] MSB = new byte[] { 0x80 };
        private static readonly Type PropertyAttribute = typeof(LightWeightPropertyAttribute);

        public static byte[] Serialize<T>(T value) {
            return Serialize(value, new LightWeightOptions());
        }
        public static byte[] Serialize<T>(T value, LightWeightOptions options) {
            if (null == value) {
                return new byte[] { };
            }

            var lw = new LightWeight(options);
            var buffer = new Buffer<byte>(options.SerializeBufferInitialSize);
            buffer = lw.Serialize(value, value.GetType(), buffer);
            return buffer.ToArray();
        }

        public static T Deserialize<T>(byte[] payload) {
            return Deserialize<T>(payload, new LightWeightOptions());
        }
        public static T Deserialize<T>(byte[] payload, LightWeightOptions options) {
#if DEBUG
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
#endif

            var lw = new LightWeight(options);
            return (T)lw.Deserialize(new Buffer<byte>(payload), typeof(T));
        }


        private readonly VLQCodec Codec = new VLQCodec();
        private readonly LightWeightOptions Options;
        private Buffer<byte> Buffer;

        public LightWeight(LightWeightOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            Options = options;
        }
        public Buffer<byte> Serialize(object value, Type type, Buffer<byte> buffer) {
#if DEBUG
            if (null == type) {
                throw new ArgumentNullException("type");
            }
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }
#endif
            Buffer = buffer;

            Serialize(value, type);

            return Buffer;
        }
        private void Serialize(object value, Type type) {
            if (value == null) { // Null
                SerializeNull();
            } else if (type == typeof(bool)) { // Bool
                SerializeBool((bool)value);
            } else if (type == typeof(sbyte)) { // SInt8
                SerializeSInt8((sbyte)value);
            } else if (type == typeof(short)) { // SInt16
                SerializeSInt16((short)value);
            } else if (type == typeof(int)) { // SInt32
                SerializeSInt32((int)value);
            } else if (type == typeof(long)) { // SInt64
                SerializeSInt64((long)value);
            } else if (type == typeof(byte)) { // UInt8
                SerializeUInt8((byte)value);
            } else if (type == typeof(ushort)) { // UInt16
                SerializeUInt16((ushort)value);
            } else if (type == typeof(uint)) { // UInt32
                SerializeUInt32((uint)value);
            } else if (type == typeof(ulong)) { // UInt64
                SerializeUInt64((ulong)value);
            } else if (type == typeof(string)) { // String
                SerializeString((string)value);
            } else {
                var dict = value as IDictionary; // Dict
                if (null != dict) {
                    SerializeDictionary(dict);
                } else {
                    var list = value as IList; // List
                    if (null != list) {
                        SerializeList(list);
                    } else {
                        SerializePOCO(value, type);
                    }
                }
            }
        }

        private void SerializeNull() {
            // Nothing to do here, move along
        }
        private void SerializeBool(bool input) {
            if (!input) {
                return;
            } else {
                _EnqueueBuffer(new byte[] { byte.MaxValue });
            }
        }

        private void SerializeSInt8(sbyte input) {
            if (input == 0) {
                return;
            } else {
                _EnqueueBuffer(new byte[] { (byte)input });
            }
        }
        private void SerializeSInt16(short input) {
            if (input <= sbyte.MaxValue && input >= sbyte.MinValue) {
                SerializeSInt8((sbyte)input);
            } else {
                _EnqueueBuffer(BitConverter.GetBytes(input));
            }
        }
        private void SerializeSInt32(int input) {
            if (input <= short.MaxValue && input >= short.MinValue) {
                SerializeSInt16((short)input);
            } else {
                _EnqueueBuffer(BitConverter.GetBytes(input));
            }
        }
        private void SerializeSInt64(long input) {
            if (input <= int.MaxValue && input >= int.MinValue) {
                SerializeSInt32((int)input);
            } else {
                _EnqueueBuffer(BitConverter.GetBytes(input));
            }
        }

        private void SerializeUInt8(byte input) {
            if (input == 0) {
                return;
            } else {
                _EnqueueBuffer(new byte[] { input });
            }
        }
        private void SerializeUInt16(ushort input) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                SerializeUInt8((byte)input);
            } else {
                _EnqueueBuffer(BitConverter.GetBytes(input));
            }
        }
        private void SerializeUInt32(uint input) {
            if (input <= ushort.MaxValue && input >= ushort.MinValue) {
                SerializeUInt16((ushort)input);
            } else {
                _EnqueueBuffer(BitConverter.GetBytes(input));
            }
        }
        private void SerializeUInt64(ulong input) {
            if (input <= uint.MaxValue && input >= uint.MinValue) {
                SerializeUInt32((uint)input);
            } else {
                _EnqueueBuffer(BitConverter.GetBytes(input));
            }
        }

        private void SerializeString(string input) {
            if (null == input) {
                return;
            } else {
                _EnqueueBuffer(Encoding.UTF8.GetBytes(input));
            }
        }

        private void SerializeList(IList value) {
            // Iterate through each element
            foreach (var subinput in value) {
                // Serialize element
                var v = Serialize(subinput);

                // Append VLQ-encoded length
                _EnqueueLength(v.Length);

                // Append serialized bytes
                _EnqueueBuffer(v);
            }
        }
        private void SerializeDictionary(IDictionary value) {
            // Enumerate all values
            var e = value.GetEnumerator();
            while (e.MoveNext()) {
                // Serialize key
                var k = Serialize(e.Key);
                _EnqueueLength(k.Length);
                _EnqueueBuffer(k);

                // Serialize value
                var v = Serialize(e.Value);
                _EnqueueLength(v.Length);
                _EnqueueBuffer(v);
            }

            return;
        }
        private void SerializePOCO(object value, Type type) {
            // Iterate properties
            var properties = _GetProperties(type);
            foreach (var property in properties) {
                // If index was missed...
                if (null == property) {
                    // Append stub byte
                    _EnqueueBuffer(MSB);
                } else {
                    // Serialize value
                    var v = Serialize(property.GetValue(value, null));
                    _EnqueueLength(v.Length);
                    _EnqueueBuffer(v);
                }
            }
        }

        private static readonly object PropertyCacheSync = new object();
        private static readonly Dictionary<Type, PropertyInfo[]> PropertyCache = new Dictionary<Type, PropertyInfo[]>();

        private PropertyInfo[] _GetProperties(Type type) {
            lock (PropertyCacheSync) {
                PropertyInfo[] output;
                if (!PropertyCache.TryGetValue(type, out output)) {
                    var a = new Dictionary<byte, PropertyInfo>();

                    foreach (var property in type.GetRuntimeProperties()) {
                        // Get property attribute which tells us the properties' index
                        var attribute = (LightWeightPropertyAttribute)property.GetCustomAttribute(PropertyAttribute, false);
                        if (null == attribute) {
                            // No attribute found, skip
                            continue;
                        }

#if DEBUG
                        // Check for duplicate index and abort if found
                        if (a.ContainsKey(attribute.Index)) {
                            throw new InvalidOperationException("Duplicate key");
                        }
#endif

                        a[attribute.Index] = property;
                    }

                    output = PropertyCache[type] = a.OrderBy(b => b.Key)
                                                    .Select(b => b.Value)
                                                    .ToArray();
                }

                return output;
            }
        }

        private void _EnqueueLength(long length) {
            // Increase buffer size if needed
            if (Buffer.Writable < 10) { // 10 is the max length of a VLQ compressed number
                var newSize = Math.Max(Buffer.Capacity + Options.SerializeBufferGrowthSize, Buffer.Readable + 10);
                Buffer = Buffer.Resize(newSize);
            }

            // Write encoded value
            Codec.CompressUnsigned((ulong)length, Buffer);
        }
        private void _EnqueueBuffer(byte[] value) {
            // Increase buffer size if needed
            if (Buffer.Writable < value.Length) {
                var newSize = Math.Max(Buffer.Capacity + Options.SerializeBufferGrowthSize, Buffer.Readable + value.Length);
                Buffer = Buffer.Resize(newSize);
            }

            // Write all value
            Buffer.EnqueueArray(value);
        }

        public object Deserialize(Buffer<byte> payload, Type type) {
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
            if (null == type) {
                throw new ArgumentNullException("type");
            }

            if (type == typeof(bool)) {
                return DeserializeBool(payload);
            }

            if (type == typeof(sbyte)) {
                return DeserializeSInt8(payload);
            }
            if (type == typeof(short)) {
                return DeserializeSInt16(payload);
            }
            if (type == typeof(int)) {
                return DeserializeSInt32(payload);
            }
            if (type == typeof(long)) {
                return DeserializeSInt64(payload);
            }

            if (type == typeof(byte)) {
                return DeserializeUInt8(payload);
            }
            if (type == typeof(ushort)) {
                return DeserializeUInt16(payload);
            }
            if (type == typeof(uint)) {
                return DeserializeUInt32(payload);
            }
            if (type == typeof(ulong)) {
                return DeserializeUInt64(payload);
            }

            if (type == typeof(string)) {
                return DeserializeString(payload);
            }


            if (type.IsArray) {
                return DeserializeArray(type.GetElementType(), payload);
            }
            if (typeof(IList).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())) {
                return DeserializeList(type.GenericTypeArguments[0], payload);
            }
            if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())) {
                return DeserializeDictionary(type.GenericTypeArguments[0], type.GenericTypeArguments[1], payload);
            }

            return DeserializePOCO(type, payload);

        }

        private bool DeserializeBool(Buffer<byte> payload) {
            if (payload.Readable == 0) {
                return false;
            }
#if DEBUG
            if (payload.Readable > 1) {
                throw new DataFormatException("Boolean values can be no more than 1 byte long.");
            }
            if (payload.Dequeue() != byte.MaxValue) {
                throw new DataFormatException("Boolean values cannot be anything other than 0xFF.");
            }
#endif

            return true;
        }

        private sbyte DeserializeSInt8(Buffer<byte> payload) {
            switch (payload.Readable) {
                case 0: return 0;
                case 1: return (sbyte)payload.Dequeue();
                default: throw new DataFormatException("SInt8 values can be 0 or 1 bytes.");
            }
        }
        private short DeserializeSInt16(Buffer<byte> payload) {
            switch (payload.Readable) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToInt16(payload.DequeueBuffer(2).ToArray(), 0);
                default: throw new DataFormatException("SInt16 values can be 0, 1, or 2 bytes.");
            }
        }
        private int DeserializeSInt32(Buffer<byte> payload) {
            switch (payload.Readable) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToInt16(payload.DequeueBuffer(2).ToArray(), 0); // Possible to optimise
                case 4: return BitConverter.ToInt32(payload.DequeueBuffer(4).ToArray(), 0);
                default: throw new DataFormatException("SInt32 values can be 0, 1, 2 or 4 bytes.");
            }
        }
        private long DeserializeSInt64(Buffer<byte> payload) {
            switch (payload.Readable) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToInt16(payload.DequeueBuffer(2).ToArray(), 0);
                case 4: return BitConverter.ToInt32(payload.DequeueBuffer(4).ToArray(), 0);
                case 8: return BitConverter.ToInt64(payload.DequeueBuffer(8).ToArray(), 0);
                default: throw new DataFormatException("SInt64 values can be 0, 1, 2, 4 or 8 bytes.");
            }
        }

        private byte DeserializeUInt8(Buffer<byte> payload) {
            switch (payload.Readable) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                default: throw new DataFormatException("UInt8 values can be 0 or 1 bytes.");
            }
        }
        private ushort DeserializeUInt16(Buffer<byte> payload) {
            switch (payload.Readable) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToUInt16(payload.DequeueBuffer(2).ToArray(), 0);
                default: throw new DataFormatException("UInt16 values can be 0, 1, or 2 bytes.");
            }
        }
        private uint DeserializeUInt32(Buffer<byte> payload) {
            switch (payload.Readable) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToUInt16(payload.DequeueBuffer(2).ToArray(), 0);
                case 4: return BitConverter.ToUInt32(payload.DequeueBuffer(4).ToArray(), 0);
                default: throw new DataFormatException("UInt32 values can be 0, 1, 2 or 4 bytes.");
            }
        }
        private ulong DeserializeUInt64(Buffer<byte> payload) {
            switch (payload.Readable) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToUInt16(payload.DequeueBuffer(2).ToArray(), 0);
                case 4: return BitConverter.ToUInt32(payload.DequeueBuffer(4).ToArray(), 0);
                case 8: return BitConverter.ToUInt64(payload.DequeueBuffer(8).ToArray(), 0);
                default: throw new DataFormatException("UInt64 values can be 0, 1, 2, 4 or 8 bytes.");
            }
        }

        private string DeserializeString(Buffer<byte> payload) {
            // Get sub buffer
            var b = payload.DequeueBuffer(payload.Readable); // Possible to optimise

            // Decode using UTF8
            return Encoding.UTF8.GetString(b.ToArray(), 0, b.Readable);
        }

        private Array DeserializeArray(Type innerType, Buffer<byte> payload) {
            // Deserialize temporarily as list
            var container = DeserializeList(innerType, payload);

            // Convert to array and return
            var output = Array.CreateInstance(innerType, container.Count);
            container.CopyTo(output, 0);

            return output;
        }
        private IList DeserializeList(Type innerType, Buffer<byte> payload) {
            // Instantiate list
            var output = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(innerType));

            // Deserialize items
            while (payload.IsReadable) {
                // Deserialize value
                var l = (int)Codec.DecompressUnsigned(payload);
                var b = payload.DequeueBuffer(l);
                var v = Deserialize(b, innerType);

                // Add to output
                output.Add(v);
            }

            return output;
        }
        private IDictionary DeserializeDictionary(Type keyType, Type valueType, Buffer<byte> payload) {
            // Instantiate dictionary
            var output = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));

            // Loop through input buffer until depleated
            while (payload.IsReadable) {
                // Deserialize key
                var kl = (int)Codec.DecompressUnsigned(payload);
                var kb = payload.DequeueBuffer(kl);
                var k = Deserialize(kb, keyType);

                // Deserialize value
                var vl = (int)Codec.DecompressUnsigned(payload);
                var vb = payload.DequeueBuffer(vl);
                var v = Deserialize(vb, valueType);

                // Add to output
                output[k] = v;
            }

            return output;
        }
        private object DeserializePOCO(Type type, Buffer<byte> payload) {
            // Instantiate output
            var output = Activator.CreateInstance(type);

            // Prepare for object deserialization
            var index = -1;

            // Attempt to read field length, if we've reached the end of the payload, abort
            while (payload.IsReadable) {
                // Get the length in a usable format
                var l = (int)Codec.DecompressUnsigned(payload);
                var b = payload.DequeueBuffer(l);

                // Increment the index
                index++;

                // Iterate through each property looking for one that matches index
                foreach (var property in type.GetRuntimeProperties()) {
                    // Get property attribute which tells us the properties' index
                    var attribute = (LightWeightPropertyAttribute)property.GetCustomAttribute(PropertyAttribute);

                    // Skip if not found, or index doesn't match
                    if (null == attribute || attribute.Index != index) {
                        // No attribute found, skip
                        continue;
                    }

                    // Deserialize value
                    var v = Deserialize(b, property.PropertyType);

                    // Set it on property
                    property.SetValue(output, v);
                }
            }

            return output;
        }
    }
}
