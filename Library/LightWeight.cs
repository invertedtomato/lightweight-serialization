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
        private const byte MSB = 0x80;
        private static readonly Type PropertyAttribute = typeof(LightWeightPropertyAttribute);

        public static byte[] Serialize<T>(T value) {
            if (null == value) {
                return new byte[] { };
            }

            var lw = new LightWeight();
            var buffer = new Buffer<byte>(8);
            buffer = lw.Serialize(value, value.GetType(), buffer);
            return buffer.ToArray();
        }
        public static T Deserialize<T>(byte[] payload) {
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }

            var lw = new LightWeight();
            return (T)lw.Deserialize(new Buffer<byte>(payload), typeof(T));
        }


        private readonly VLQCodec Codec = new VLQCodec();
        private readonly Buffer<ulong> LengthBuffer = new Buffer<ulong>(1);

        public Buffer<byte> Serialize(object value, Type type, Buffer<byte> buffer) {
            // Serialize input to byte array
            if (value == null) { // Null
                return SerializeNull(buffer);
            }

            if (type == typeof(bool)) { // Bool
                return SerializeBool((bool)value, buffer);
            }

            if (type == typeof(sbyte)) { // SInt8
                return SerializeSInt8((sbyte)value, buffer);
            }
            if (type == typeof(short)) { // SInt16
                return SerializeSInt16((short)value, buffer);
            }
            if (type == typeof(int)) { // SInt32
                return SerializeSInt32((int)value, buffer);
            }
            if (type == typeof(long)) { // SInt64
                return SerializeSInt64((long)value, buffer);
            }

            if (type == typeof(byte)) { // UInt8
                return SerializeUInt8((byte)value, buffer);
            }
            if (type == typeof(ushort)) { // UInt16
                return SerializeUInt16((ushort)value, buffer);
            }
            if (type == typeof(uint)) { // UInt32
                return SerializeUInt32((uint)value, buffer);
            }
            if (type == typeof(ulong)) { // UInt64
                return SerializeUInt64((ulong)value, buffer);
            }

            if (type == typeof(string)) { // String
                return SerializeString((string)value, buffer);
            }

            var dict = value as IDictionary;
            if (null != dict) {
                return SerializeDictionary(dict, buffer);
            }

            var list = value as IList;
            if (null != list) {
                return SerializeList(list, buffer);
            }

            return SerializePOCO(value, type, buffer);
        }

        private Buffer<byte> SerializeNull(Buffer<byte> buffer) {
            return buffer;
        }
        private Buffer<byte> SerializeBool(bool input, Buffer<byte> buffer) {
            if (!input) {
                return buffer;
            }

            return buffer.EnqueueArrayWithResize(new byte[] { byte.MaxValue });
        }

        private Buffer<byte> SerializeSInt8(sbyte input, Buffer<byte> buffer) {
            if (input == 0) {
                return buffer;
            }

            return buffer.EnqueueArrayWithResize(new byte[] { (byte)input });
        }
        private Buffer<byte> SerializeSInt16(short input, Buffer<byte> buffer) {
            if (input <= sbyte.MaxValue && input >= sbyte.MinValue) {
                return SerializeSInt8((sbyte)input, buffer);
            } else {
                return buffer.EnqueueArrayWithResize(BitConverter.GetBytes(input));
            }
        }
        private Buffer<byte> SerializeSInt32(int input, Buffer<byte> buffer) {
            if (input <= short.MaxValue && input >= short.MinValue) {
                return SerializeSInt16((short)input, buffer);
            }

            return buffer.EnqueueArrayWithResize(BitConverter.GetBytes(input));
        }
        private Buffer<byte> SerializeSInt64(long input, Buffer<byte> buffer) {
            if (input <= int.MaxValue && input >= int.MinValue) {
                return SerializeSInt32((int)input, buffer);
            }

            return buffer.EnqueueArrayWithResize(BitConverter.GetBytes(input));
        }

        private Buffer<byte> SerializeUInt8(byte input, Buffer<byte> buffer) {
            if (input == 0) {
                return buffer;
            }

            return buffer.EnqueueArrayWithResize(new byte[] { input });
        }
        private Buffer<byte> SerializeUInt16(ushort input, Buffer<byte> buffer) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                return SerializeUInt8((byte)input, buffer);
            }

            return buffer.EnqueueArrayWithResize(BitConverter.GetBytes(input));
        }
        private Buffer<byte> SerializeUInt32(uint input, Buffer<byte> buffer) {
            if (input <= ushort.MaxValue && input >= ushort.MinValue) {
                return SerializeUInt16((ushort)input, buffer);
            }

            return buffer.EnqueueArrayWithResize(BitConverter.GetBytes(input));
        }
        private Buffer<byte> SerializeUInt64(ulong input, Buffer<byte> buffer) {
            if (input <= uint.MaxValue && input >= uint.MinValue) {
                return SerializeUInt32((uint)input, buffer);
            }

            return buffer.EnqueueArrayWithResize(BitConverter.GetBytes(input));
        }

        private Buffer<byte> SerializeString(string input, Buffer<byte> buffer) {
            if (null == input) {
                return buffer;
            }

            return buffer.EnqueueArrayWithResize(Encoding.UTF8.GetBytes(input));
        }

        private Buffer<byte> SerializeList(IList value, Buffer<byte> buffer) {
            // Iterate through each element
            foreach (var subinput in value) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Resize buffer if required
                var space = 10 + serialized.Length; // 10 is for length header
                if (buffer.Writable < space) {
                    buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Readable + space));
                }

                // Append VLQ-encoded length
                Codec.CompressUnsignedBuffer(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append serialized bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer;
        }
        private Buffer<byte> SerializeDictionary(IDictionary value, Buffer<byte> buffer) {
            // Enumerate all values
            var e = value.GetEnumerator();
            while (e.MoveNext()) {
                // Serialize elements
                var k = Serialize(e.Key);
                var v = Serialize(e.Value);

                // Resize buffer if required
                var space = 10 + k.Length + v.Length; // 10 is for length header
                if (buffer.Writable < space) {
                    buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Readable + space));
                }

                // Append VLQ-encoded length
                Codec.CompressUnsignedBuffer(new Buffer<ulong>(new ulong[] { (ulong)k.Length + (ulong)v.Length }), buffer);

                // Append serialized data
                buffer.EnqueueArray(k);
                buffer.EnqueueArray(v);
            }

            return buffer;
        }
        private Buffer<byte> SerializePOCO(object value, Type type, Buffer<byte> buffer) {

            // Iterate through each property,
            var propertiesSerialized = new byte[byte.MaxValue][];
            var maxPropertyIndex = -1;
            var outputBufferSize = 0;
            foreach (var property in type.GetRuntimeProperties()) {
                // Get property attribute which tells us the properties' index
                var lightWeightProperty = (LightWeightPropertyAttribute)property.GetCustomAttribute(PropertyAttribute);
                if (null == lightWeightProperty) {
                    // No attribute found, skip
                    continue;
                }

                // Check for duplicate index and abort if found
                if (null != propertiesSerialized[lightWeightProperty.Index]) {
                    throw new InvalidOperationException("Duplicate key");
                }

                // Serialize property
                propertiesSerialized[lightWeightProperty.Index] = Serialize(property.GetValue(value, null));

                // Adjust max used index if needed
                if (lightWeightProperty.Index > maxPropertyIndex) {
                    maxPropertyIndex = lightWeightProperty.Index;
                }

                // Take an educated guess how much buffer is required
                outputBufferSize += propertiesSerialized[lightWeightProperty.Index].Length + 4; // NOTE: this '4' is an arbitary number of spare bytes to fit the length header
            }

            // TODO: truncate unused fields? 

            // Resize buffer if required
            if (buffer.Writable < outputBufferSize) {
                buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Readable + outputBufferSize));
            }

            // Iterate through each properties serialized data to merge into one output array
            for (var i = 0; i <= maxPropertyIndex; i++) {
                var propertySerialized = propertiesSerialized[i];

                // If index was missed...
                if (null == propertySerialized) {
                    // Increase buffer size if needed
                    if (buffer.Writable < 1) {
                        buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Readable + 1));
                    }

                    // Append stub byte
                    buffer.Enqueue(MSB);
                } else {
                    // Increase buffer size if needed
                    if (buffer.Writable < propertySerialized.Length + 10) {
                        buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Readable + propertiesSerialized.Length + 10));
                    }

                    // Append VLQ-encoded length
                    Codec.CompressUnsignedBuffer(new Buffer<ulong>(new ulong[] { (ulong)propertySerialized.Length }), buffer);

                    // Append encoded bytes
                    buffer.EnqueueArray(propertySerialized);
                }
            }

            return buffer;
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
            // Get raw bytes
            var raw = payload.DequeueBuffer(payload.Readable).ToArray(); // Possible to optimise

            // Decode using UTF8
            return Encoding.UTF8.GetString(raw, 0, raw.Length);
        }

        private Array DeserializeArray(Type innerType, Buffer<byte> payload) {
            // Create temporary container list while elements are being deserialized
            var container = DeserializeList(innerType, payload);

            // Create output array and populate with items
            var output = Array.CreateInstance(innerType, container.Count);
            container.CopyTo(output, 0);
            return output;
        }
        private IList DeserializeList(Type innerType, Buffer<byte> payload) {
            // Instantiate list
            var output = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(innerType));

            // Deserialize items
            while (payload.IsReadable) {
                // Get the length in a usable format
                Codec.DecompressUnsignedBuffer(payload, LengthBuffer);
                var length = (int)LengthBuffer.Dequeue();
                LengthBuffer.Reset();

                // Deserialize element
                output.Add(Deserialize(payload.DequeueBuffer(length), innerType));
            }

            return output;
        }
        private IDictionary DeserializeDictionary(Type keyType, Type valueType, Buffer<byte> payload) {
            throw new NotImplementedException();
        }
        private object DeserializePOCO(Type type, Buffer<byte> payload) {
            // Instantiate output object
            var output = Activator.CreateInstance(type);

            // Prepare for object deserialization
            var lengthBuffer = new Buffer<ulong>(1);
            var index = -1;

            // Attempt to read field length, if we've reached the end of the payload, abort
            while (payload.IsReadable) {
                // Get the length in a usable format
                Codec.DecompressUnsignedBuffer(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Increment the index
                index++;

                // Iterate through each property looking for one that matches index
                foreach (var property in type.GetRuntimeProperties()) {
                    // Get property attribute which tells us the properties' index
                    var lightWeightProperty = (LightWeightPropertyAttribute)property.GetCustomAttribute(PropertyAttribute);

                    // Skip if not found, or index doesn't match
                    if (null == lightWeightProperty || lightWeightProperty.Index != index) {
                        // No attribute found, skip
                        continue;
                    }

                    // Decode value
                    var value = Deserialize(payload.DequeueBuffer(length), property.PropertyType);

                    // Set it on property
                    property.SetValue(output, value);
                }
            }

            return output;
        }
    }
}
