using InvertedTomato.IO.Buffers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using InvertedTomato.Compression.Integers;

/* TODO:
 - abort if type isn't serializable
 - add enumerable support?
 - datetime
 - timespan
 - convert to Stream?
 - test edge conditions
 - performance compare to protobuff+JSON
 - Use Buffer<byte> on serialize?
 - readme
*/

namespace InvertedTomato.LightWeightSerialization {
    public class LightWeight {
        private const byte MSB = 0x80;
        private static readonly Type PropertyAttribute = typeof(LightWeightPropertyAttribute);
        private static readonly VLQCodec Codec = new VLQCodec();

        public byte[] Serialize<T>(T input) {
            return Serialize(input, input.GetType());
        }

        public byte[] Serialize(object input, Type type) {
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
                    Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)propertySerialized.Length }), buffer);

                    // Append encoded bytes
                    buffer.EnqueueArray(propertySerialized);
                }
            }

            return buffer.ToArray();
        }

        private byte[] SerializeBool(bool input) {
            if (!input) {
                return new byte[] { };
            }

            return new byte[] { byte.MaxValue };
        }

        private byte[] SerializeSInt8(sbyte input) {
            if (input == 0) {
                return new byte[] { };
            }

            return new byte[] { (byte)input };
        }
        private byte[] SerializeSInt16(short input) {
            if (input <= sbyte.MaxValue && input >= sbyte.MinValue) {
                return Serialize((sbyte)input);
            }

            return BitConverter.GetBytes(input);
        }
        private byte[] SerializeSInt32(int input) {
            if (input <= short.MaxValue && input >= short.MinValue) {
                return Serialize((short)input);
            }

            return BitConverter.GetBytes(input);
        }
        private byte[] SerializeSInt64(long input) {
            if (input <= int.MaxValue && input >= int.MinValue) {
                return Serialize((int)input);
            }

            return BitConverter.GetBytes(input);
        }

        private byte[] SerializeUInt8(byte input) {
            if (input == 0) {
                return new byte[] { };
            }

            return new byte[] { input };
        }
        private byte[] SerializeUInt16(ushort input) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                return Serialize((byte)input);
            }

            return BitConverter.GetBytes(input);
        }
        private byte[] SerializeUInt32(uint input) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                return Serialize((byte)input);
            }
            if (input <= ushort.MaxValue && input >= ushort.MinValue) {
                return Serialize((ushort)input);
            }

            return BitConverter.GetBytes(input);
        }
        private byte[] SerializeUInt64(ulong input) {
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

        private byte[] SerializeString(string input) {
            if (null == input) {
                return new byte[] { };
            }

            return Encoding.UTF8.GetBytes(input);
        }

        private byte[] SerializeBoolArray(bool[] input) {

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
                Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }

        private byte[] SerializeSInt8Array(sbyte[] input) {

            var buffer = new Buffer<byte>(input.Length * 2);

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        private byte[] SerializeSInt16Array(short[] input) {

            var buffer = new Buffer<byte>(input.Length * 3);

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        private byte[] SerializeSInt32Array(int[] input) {

            var buffer = new Buffer<byte>(input.Length * 5);

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        private byte[] SerializeSInt64Array(long[] input) {

            var buffer = new Buffer<byte>(input.Length * 9);

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }

        private byte[] SerializeUInt8Array(byte[] input) {

            var buffer = new Buffer<byte>(input.Length * 2); // Longest possible value is 2 bytes (length + paylaod)

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        private byte[] SerializeUInt16Array(ushort[] input) {

            var buffer = new Buffer<byte>(input.Length * 3); // Longest possible value is 3 bytes

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        private byte[] SerializeUInt32Array(uint[] input) {

            var buffer = new Buffer<byte>(input.Length * 5); // Longest possible value is 5 bytes

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }
        private byte[] SerializeUInt64Array(ulong[] input) {

            var buffer = new Buffer<byte>(input.Length * 9); // Longest possible value is 9 bytes

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Append VLQ-encoded length
                Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }

        private byte[] SerializeStringArray(string[] input) {

            var buffer = new Buffer<byte>(input.Length * 8); // Arbitary guess at average string length

            // Iterate through each element
            foreach (var subinput in input) {
                // Serialize element
                var serialized = Serialize(subinput);

                // Increase buffer size if needed
                if (buffer.Available < serialized.Length + 10) {
                    buffer = buffer.Resize(Math.Max(buffer.Capacity * 2, buffer.Used + serialized.Length + 10)); // 10 is arbitary
                }

                // Append VLQ-encoded length
                Codec.Compress(new Buffer<ulong>(new ulong[] { (ulong)serialized.Length }), buffer);

                // Append encoded bytes
                buffer.EnqueueArray(serialized);
            }

            return buffer.ToArray();
        }


        public T Deserialize<T>(byte[] payload) {
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }

            return (T)Deserialize(new Buffer<byte>(payload), typeof(T));
        }
        public T Deserialize<T>(Buffer<byte> payload) {
            return (T)Deserialize(payload, typeof(T));
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
            var lengthBuffer = new Buffer<ulong>(1);
            var index = -1;

            // Attempt to read field length, if we've reached the end of the payload, abort
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
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


        private bool DeserializeBool(Buffer<byte> payload) {
            if (null == payload) {
                throw new ArgumentNullException("input");
            }

            if (payload.Used == 0) {
                return false;
            }
            if (payload.Used > 1) {
                throw new DataFormatException("Boolean values can be no more than 1 byte long.");
            }
            if (payload.Dequeue() != byte.MaxValue) {
                throw new DataFormatException("Boolean values cannot be anything other than 0xFF.");
            }

            return true;
        }

        private sbyte DeserializeSInt8(Buffer<byte> payload) {
            if (null == payload) {
                throw new ArgumentNullException("input");
            }

            switch (payload.Used) {
                case 0: return 0;
                case 1: return (sbyte)payload.Dequeue();
                default: throw new DataFormatException("SInt8 values can be 0 or 1 bytes.");
            }
        }
        private short DeserializeSInt16(Buffer<byte> payload) {
            if (null == payload) {
                throw new ArgumentNullException("input");
            }

            switch (payload.Used) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToInt16(payload.DequeueBuffer(2).ToArray(), 0);
                default: throw new DataFormatException("SInt16 values can be 0, 1, or 2 bytes.");
            }
        }
        private int DeserializeSInt32(Buffer<byte> payload) {
            if (null == payload) {
                throw new ArgumentNullException("input");
            }

            switch (payload.Used) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToInt16(payload.DequeueBuffer(2).ToArray(), 0); // Possible to optimise
                case 4: return BitConverter.ToInt32(payload.DequeueBuffer(4).ToArray(), 0);
                default: throw new DataFormatException("SInt32 values can be 0, 1, 2 or 4 bytes.");
            }
        }
        private long DeserializeSInt64(Buffer<byte> payload) {
            if (null == payload) {
                throw new ArgumentNullException("input");
            }

            switch (payload.Used) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToInt16(payload.DequeueBuffer(2).ToArray(), 0);
                case 4: return BitConverter.ToInt32(payload.DequeueBuffer(4).ToArray(), 0);
                case 8: return BitConverter.ToInt64(payload.DequeueBuffer(8).ToArray(), 0);
                default: throw new DataFormatException("SInt64 values can be 0, 1, 2, 4 or 8 bytes.");
            }
        }

        private byte DeserializeUInt8(Buffer<byte> payload) {
            if (null == payload) {
                throw new ArgumentNullException("input");
            }

            switch (payload.Used) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                default: throw new DataFormatException("UInt8 values can be 0 or 1 bytes.");
            }
        }
        private ushort DeserializeUInt16(Buffer<byte> payload) {
            if (null == payload) {
                throw new ArgumentNullException("input");
            }

            switch (payload.Used) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToUInt16(payload.DequeueBuffer(2).ToArray(), 0);
                default: throw new DataFormatException("UInt16 values can be 0, 1, or 2 bytes.");
            }
        }
        private uint DeserializeUInt32(Buffer<byte> payload) {
            if (null == payload) {
                throw new ArgumentNullException("input");
            }

            switch (payload.Used) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToUInt16(payload.DequeueBuffer(2).ToArray(), 0);
                case 4: return BitConverter.ToUInt32(payload.DequeueBuffer(4).ToArray(), 0);
                default: throw new DataFormatException("UInt32 values can be 0, 1, 2 or 4 bytes.");
            }
        }
        private ulong DeserializeUInt64(Buffer<byte> payload) {
            if (null == payload) {
                throw new ArgumentNullException("input");
            }

            switch (payload.Used) {
                case 0: return 0;
                case 1: return payload.Dequeue();
                case 2: return BitConverter.ToUInt16(payload.DequeueBuffer(2).ToArray(), 0);
                case 4: return BitConverter.ToUInt32(payload.DequeueBuffer(4).ToArray(), 0);
                case 8: return BitConverter.ToUInt64(payload.DequeueBuffer(8).ToArray(), 0);
                default: throw new DataFormatException("UInt64 values can be 0, 1, 2, 4 or 8 bytes.");
            }
        }

        private string DeserializeString(Buffer<byte> payload) {
            if (null == payload) {
                throw new ArgumentNullException("input");
            }

            // Get raw bytes
            var raw = payload.DequeueBuffer(payload.Used).ToArray(); // Possible to optimise

            // Decode using UTF8
            return Encoding.UTF8.GetString(raw, 0, raw.Length);
        }


        private bool[] DeserializeBoolArray(Buffer<byte> payload) {
            var output = new List<bool>();
            var lengthBuffer = new Buffer<ulong>(1);
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Deserialize element
                output.Add(DeserializeBool(payload.DequeueBuffer(length)));
            }

            return output.ToArray();
        }

        private sbyte[] DeserializeSInt8Array(Buffer<byte> payload) {
            var output = new List<sbyte>();
            var lengthBuffer = new Buffer<ulong>(1);
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Deserialize element
                output.Add(DeserializeSInt8(payload.DequeueBuffer(length)));
            }

            return output.ToArray();
        }
        private short[] DeserializeSInt16Array(Buffer<byte> payload) {
            var output = new List<short>();
            var lengthBuffer = new Buffer<ulong>(1);
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Deserialize element
                output.Add(DeserializeSInt16(payload.DequeueBuffer(length)));
            }

            return output.ToArray();
        }
        private int[] DeserializeSInt32Array(Buffer<byte> payload) {
            var output = new List<int>();
            var lengthBuffer = new Buffer<ulong>(1);
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Deserialize element
                output.Add(DeserializeSInt32(payload.DequeueBuffer(length)));
            }

            return output.ToArray();
        }
        private long[] DeserializeSInt64Array(Buffer<byte> payload) {
            var output = new List<long>();
            var lengthBuffer = new Buffer<ulong>(1);
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Deserialize element
                output.Add(DeserializeSInt64(payload.DequeueBuffer(length)));
            }

            return output.ToArray();
        }

        private byte[] DeserializeUInt8Array(Buffer<byte> payload) {
            var output = new List<byte>();
            var lengthBuffer = new Buffer<ulong>(1);
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Deserialize element
                output.Add(DeserializeUInt8(payload.DequeueBuffer(length)));
            }

            return output.ToArray();
        }
        private ushort[] DeserializeUInt16Array(Buffer<byte> payload) {
            var output = new List<ushort>();
            var lengthBuffer = new Buffer<ulong>(1);
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Deserialize element
                output.Add(DeserializeUInt16(payload.DequeueBuffer(length)));
            }

            return output.ToArray();
        }
        private uint[] DeserializeUInt32Array(Buffer<byte> payload) {
            var output = new List<uint>();
            var lengthBuffer = new Buffer<ulong>(1);
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Deserialize element
                output.Add(DeserializeUInt32(payload.DequeueBuffer(length)));
            }

            return output.ToArray();
        }
        private ulong[] DeserializeUInt64Array(Buffer<byte> payload) {
            var output = new List<ulong>();
            var lengthBuffer = new Buffer<ulong>(1);
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Deserialize element
                output.Add(DeserializeUInt64(payload.DequeueBuffer(length)));
            }

            return output.ToArray();
        }

        private string[] DeserializeStringArray(Buffer<byte> payload) {
            var output = new List<string>();
            var lengthBuffer = new Buffer<ulong>(1);
            while (!payload.IsEmpty) {
                // Get the length in a usable format
                Codec.Decompress(payload, lengthBuffer);
                var length = (int)lengthBuffer.Dequeue();
                lengthBuffer.Reset();

                // Deserialize element
                output.Add(DeserializeString(payload.DequeueBuffer(length)));
            }

            return output.ToArray();
        }
    }
}
