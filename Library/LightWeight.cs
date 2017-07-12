using InvertedTomato.IO.Buffers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using InvertedTomato.Compression.Integers;

/* TODO:
 - abort if type isn't serializable
 - add deserialise
 - add array/enumerable support
 - datetime
 - timespan
 -- convert to Stream
 - test edge conditions
 - performance compare to protobuff+JSON
 - readme
*/

namespace InvertedTomato.LightWeightSerialization {
    public static class LightWeight {
        private const byte MSB = 0x80;
        private static readonly Type PropertyAttribute = typeof(LightWeightPropertyAttribute);

        public static byte[] Serialize<T>(T input) {
            return Serialize(input, input.GetType());
        }

        public static byte[] Serialize(object input, Type type) {
            if (null == type) {
                throw new ArgumentNullException("type");
            }

            // Serialize input to byte array
            if (input == null) { // Null
                return new byte[] { };
            }

            if (type == typeof(bool)) { // Bool
                return SerializeBool((bool)input);
            }

            if (type == typeof(sbyte)) { // SInt8
                return SerializeSInt8((sbyte)input);
            }
            if (type == typeof(short)) { // SInt16
                return SerializeSInt16((short)input);
            }
            if (type == typeof(int)) { // SInt32
                return SerializeSInt32((int)input);
            }
            if (type == typeof(long)) { // SInt64
                return SerializeSInt64((long)input);
            }

            if (type == typeof(byte)) { // UInt8
                return SerializeUInt8((byte)input);
            }
            if (type == typeof(ushort)) { // UInt16
                return SerializeUInt16((ushort)input);
            }
            if (type == typeof(uint)) { // UInt32
                return SerializeUInt32((uint)input);
            }
            if (type == typeof(ulong)) { // UInt64
                return SerializeUInt64((ulong)input);
            }

            if (type == typeof(string)) { // String
                return SerializeString((string)input);
            }



            if (type == typeof(bool[])) { // Bool array
                return SerializeBoolArray((bool[])input);
            }

            if (type == typeof(sbyte[])) { // SInt8 array
                return SerializeSInt8Array((sbyte[])input);
            }
            if (type == typeof(short[])) { // SInt16 array
                return SerializeSInt16Array((short[])input);
            }
            if (type == typeof(int[])) { // SInt32 array
                return SerializeSInt32Array((int[])input);
            }
            if (type == typeof(long[])) { // SInt64 array
                return SerializeSInt64Array((long[])input);
            }

            if (type == typeof(byte[])) { // UInt8 array
                return SerializeUInt8Array((byte[])input);
            }
            if (type == typeof(ushort[])) { // UInt16 array
                return SerializeUInt16Array((ushort[])input);
            }
            if (type == typeof(uint[])) { // UInt32 array
                return SerializeUInt32Array((uint[])input);
            }
            if (type == typeof(ulong[])) { // UInt64 array
                return SerializeUInt64Array((ulong[])input);
            }

            if (type == typeof(string[])) { // String array
                return SerializeStringArray((string[])input);
            }


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

                // Get value
                var value = property.GetValue(input, null);


                // Serialize property
                propertiesSerialized[lightWeightProperty.Index] = Serialize(value);

                // Adjust max used index if needed
                if (lightWeightProperty.Index > maxPropertyIndex) {
                    maxPropertyIndex = lightWeightProperty.Index;
                }

                // Take an educated guess how much buffer is required
                outputBufferSize += propertiesSerialized[lightWeightProperty.Index].Length + 4; // NOTE: this '4' is an arbitary number of spare bytes to fit the length header
            }

            // TODO: truncate unused fields? 

            // Allocate output buffer
            var buffer = new Buffer<byte>(outputBufferSize);
            // TODO: buffer overrun
            // Iterate through each properties serialized data to merge into one output array
            var codec = new VLQCodec();
            for (var i = 0; i <= maxPropertyIndex; i++) {
                var propertySerialized = propertiesSerialized[i];

                // If index was missed...
                if (null == propertySerialized) {
                    // Increase buffer size if needed
                    if (buffer.Available < 1) {
                        buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Used + 1));
                    }

                    // Append stub byte
                    buffer.Enqueue(MSB);
                } else {
                    // Increase buffer size if needed
                    if (buffer.Available < propertySerialized.Length + 10) {
                        buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Used + propertiesSerialized.Length + 10));
                    }

                    // Append VLQ-encoded length
                    codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)propertySerialized.Length }), buffer);

                    // Append encoded bytes
                    buffer.EnqueueArray(propertySerialized);
                }
            }

            return buffer.ToArray();
            // TODO: arrays?
        }

        internal static byte[] SerializeBool(bool input) {
            if (!input) {
                return new byte[] { };
            }

            return new byte[] { byte.MaxValue };
        }

        internal static byte[] SerializeSInt8(sbyte input) {
            if (input == 0) {
                return new byte[] { };
            }

            return new byte[] { (byte)input };
        }
        internal static byte[] SerializeSInt16(short input) {
            if (input <= sbyte.MaxValue && input >= sbyte.MinValue) {
                return Serialize((sbyte)input);
            }

            return BitConverter.GetBytes(input);
        }
        internal static byte[] SerializeSInt32(int input) {
            if (input <= short.MaxValue && input >= short.MinValue) {
                return Serialize((short)input);
            }

            return BitConverter.GetBytes(input);
        }
        internal static byte[] SerializeSInt64(long input) {
            if (input <= int.MaxValue && input >= int.MinValue) {
                return Serialize((int)input);
            }

            return BitConverter.GetBytes(input);
        }

        internal static byte[] SerializeUInt8(byte input) {
            if (input == 0) {
                return new byte[] { };
            }

            return new byte[] { input };
        }
        internal static byte[] SerializeUInt16(ushort input) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                return Serialize((byte)input);
            }

            return BitConverter.GetBytes(input);
        }
        internal static byte[] SerializeUInt32(uint input) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                return Serialize((byte)input);
            }
            if (input <= ushort.MaxValue && input >= ushort.MinValue) {
                return Serialize((ushort)input);
            }

            return BitConverter.GetBytes(input);
        }
        internal static byte[] SerializeUInt64(ulong input) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                return Serialize((byte)input);
            }
            if (input <= ushort.MaxValue && input >= ushort.MinValue) {
                return Serialize((ushort)input);
            }
            if (input <= uint.MaxValue && input >= uint.MinValue) {
                return Serialize((uint)input);
            }

            return BitConverter.GetBytes(input);
        }

        internal static byte[] SerializeString(string input) {
            if (null == input) {
                return new byte[] { };
            }

            return Encoding.UTF8.GetBytes(input);
        }

        internal static byte[] SerializeBoolArray(bool[] input) {
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(input.Length * 2); // TRUE is the largest possible value, which encodes as 2 bytes

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);
                /*
                // Increase buffer size if needed
                if (buffer.Available < serialized.Length + 10) {
                    buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Used + serialized.Length + 10)); // 10 is arbitary
                }
                */
                // Append VLQ-encoded length
                codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }

        internal static byte[] SerializeSInt8Array(sbyte[] input) {
            throw new NotImplementedException();
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(input.Length * 1);

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);
                /*
                // Increase buffer size if needed
                if (buffer.Available < serialized.Length + 10) {
                    buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Used + serialized.Length + 10)); // 10 is arbitary
                }
                */
                // Append VLQ-encoded length
                codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        internal static byte[] SerializeSInt16Array(short[] input) {
            throw new NotImplementedException();
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(input.Length * 1);

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);
                /*
                // Increase buffer size if needed
                if (buffer.Available < serialized.Length + 10) {
                    buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Used + serialized.Length + 10)); // 10 is arbitary
                }
                */
                // Append VLQ-encoded length
                codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        internal static byte[] SerializeSInt32Array(int[] input) {
            throw new NotImplementedException();
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(input.Length * 1);

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);
                /*
                // Increase buffer size if needed
                if (buffer.Available < serialized.Length + 10) {
                    buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Used + serialized.Length + 10)); // 10 is arbitary
                }
                */
                // Append VLQ-encoded length
                codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        internal static byte[] SerializeSInt64Array(long[] input) {
            throw new NotImplementedException();
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(input.Length * 1);

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);
                /*
                // Increase buffer size if needed
                if (buffer.Available < serialized.Length + 10) {
                    buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Used + serialized.Length + 10)); // 10 is arbitary
                }
                */
                // Append VLQ-encoded length
                codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }

        internal static byte[] SerializeUInt8Array(byte[] input) {
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(input.Length * 2); // Longest possible value is 2 bytes (length + paylaod)

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        internal static byte[] SerializeUInt16Array(ushort[] input) {
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(input.Length * 3); // Longest possible value is 3 bytes

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        internal static byte[] SerializeUInt32Array(uint[] input) {
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(input.Length * 5); // Longest possible value is 5 bytes

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        internal static byte[] SerializeUInt64Array(ulong[] input) {
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(input.Length * 9); // Longest possible value is 9 bytes

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }

        internal static byte[] SerializeStringArray(string[] input) {
            throw new NotImplementedException();
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(input.Length * 1);

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);
                /*
                // Increase buffer size if needed
                if (buffer.Available < serialized.Length + 10) {
                    buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Used + serialized.Length + 10)); // 10 is arbitary
                }
                */
                // Append VLQ-encoded length
                codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }


        public static T Deserialize<T>(byte[] payload) {
            return (T)Deserialize(payload, typeof(T));
        }
        public static object Deserialize(byte[] payload, Type type) {
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





            if (type == typeof(bool[])) { // Bool array
                return DeserializeBoolArray(payload);
            }

            if (type == typeof(sbyte[])) { // SInt8 array
                return DeserializeSInt8Array(payload);
            }
            if (type == typeof(short[])) { // SInt16 array
                return DeserializeSInt16Array(payload);
            }
            if (type == typeof(int[])) { // SInt32 array
                return DeserializeSInt32Array(payload);
            }
            if (type == typeof(long[])) { // SInt64 array
                return DeserializeSInt64Array(payload);
            }

            if (type == typeof(byte[])) { // UInt8 array
                return DeserializeUInt8Array(payload);
            }
            if (type == typeof(ushort[])) { // UInt16 array
                return DeserializeUInt16Array(payload);
            }
            if (type == typeof(uint[])) { // UInt32 array
                return DeserializeUInt32Array(payload);
            }
            if (type == typeof(ulong[])) { // UInt64 array
                return DeserializeUInt64Array(payload);
            }

            if (type == typeof(string[])) { // String array
                return DeserializeStringArray(payload);
            }

            // Instantiate output object
            var output = Activator.CreateInstance(type);

            // Prepare for object deserialization
            var codec = new VLQCodec();
            var buffer = new Buffer<byte>(payload);
            var lengthBuffer = new Buffer<ulong>(1);
            var index = -1;

            // Attempt to read field length, if we've reached the end of the payload, abort
            while (!buffer.IsEmpty) {
                // Get the length in a usable format
                codec.Decompress(buffer, lengthBuffer);
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
                    var value = Deserialize(buffer.DequeueBuffer(length).ToArray(), property.PropertyType);

                    // Set it on property
                    property.SetValue(output, value);
                }
            }

            return output;
        }

        private static object DeserializeStringArray(byte[] payload) {
            throw new NotImplementedException();
        }

        private static object DeserializeUInt64Array(byte[] payload) {
            throw new NotImplementedException();
        }

        private static object DeserializeUInt32Array(byte[] payload) {
            throw new NotImplementedException();
        }

        private static object DeserializeUInt16Array(byte[] payload) {
            throw new NotImplementedException();
        }

        private static object DeserializeUInt8Array(byte[] payload) {
            throw new NotImplementedException();
        }

        private static object DeserializeSInt64Array(byte[] payload) {
            throw new NotImplementedException();
        }

        private static object DeserializeSInt32Array(byte[] payload) {
            throw new NotImplementedException();
        }

        private static object DeserializeSInt16Array(byte[] payload) {
            throw new NotImplementedException();
        }

        private static object DeserializeSInt8Array(byte[] payload) {
            throw new NotImplementedException();
        }

        private static object DeserializeBoolArray(byte[] payload) {
            throw new NotImplementedException();
        }

        internal static bool DeserializeBool(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            if (input.Length == 0) {
                return false;
            }
            if (input.Length > 1) {
                throw new DataFormatException("Boolean values can be no more than 1 byte long.");
            }
            if (input[0] != byte.MaxValue) {
                throw new DataFormatException("Boolean values cannot be anything other than 0xFF.");
            }

            return true;
        }

        internal static sbyte DeserializeSInt8(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            switch (input.Length) {
                case 0: return 0;
                case 1: return (sbyte)input[0];
                default: throw new DataFormatException("SInt8 values can be 0 or 1 bytes.");
            }
        }
        internal static short DeserializeSInt16(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            switch (input.Length) {
                case 0: return 0;
                case 1: return input[0];
                case 2: return BitConverter.ToInt16(input, 0);
                default: throw new DataFormatException("SInt16 values can be 0, 1, or 2 bytes.");
            }
        }
        internal static int DeserializeSInt32(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            switch (input.Length) {
                case 0: return 0;
                case 1: return input[0];
                case 2: return BitConverter.ToInt16(input, 0);
                case 4: return BitConverter.ToInt32(input, 0);
                default: throw new DataFormatException("SInt32 values can be 0, 1, 2 or 4 bytes.");
            }
        }
        internal static long DeserializeSInt64(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            switch (input.Length) {
                case 0: return 0;
                case 1: return input[0];
                case 2: return BitConverter.ToInt16(input, 0);
                case 4: return BitConverter.ToInt32(input, 0);
                case 8: return BitConverter.ToInt64(input, 0);
                default: throw new DataFormatException("SInt64 values can be 0, 1, 2, 4 or 8 bytes.");
            }
        }

        internal static byte DeserializeUInt8(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            switch (input.Length) {
                case 0: return 0;
                case 1: return input[0];
                default: throw new DataFormatException("UInt8 values can be 0 or 1 bytes.");
            }
        }
        internal static ushort DeserializeUInt16(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            switch (input.Length) {
                case 0: return 0;
                case 1: return input[0];
                case 2: return BitConverter.ToUInt16(input, 0);
                default: throw new DataFormatException("UInt16 values can be 0, 1, or 2 bytes.");
            }
        }
        internal static uint DeserializeUInt32(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            switch (input.Length) {
                case 0: return 0;
                case 1: return input[0];
                case 2: return BitConverter.ToUInt16(input, 0);
                case 4: return BitConverter.ToUInt32(input, 0);
                default: throw new DataFormatException("UInt32 values can be 0, 1, 2 or 4 bytes.");
            }
        }
        internal static ulong DeserializeUInt64(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            switch (input.Length) {
                case 0: return 0;
                case 1: return input[0];
                case 2: return BitConverter.ToUInt16(input, 0);
                case 4: return BitConverter.ToUInt32(input, 0);
                case 8: return BitConverter.ToUInt64(input, 0);
                default: throw new DataFormatException("UInt64 values can be 0, 1, 2, 4 or 8 bytes.");
            }
        }

        internal static string DeserializeString(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            return Encoding.UTF8.GetString(input, 0, input.Length);
        }
    }
}
