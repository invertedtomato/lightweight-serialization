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
            // Serialize input to byte array
            if (input == null) { // Null
                return new byte[] { };
            }

            if (input is bool) { // Boolean
                return SerializeBoolean((bool)(object)input);
            }

            if (input is sbyte) { // SInt8
                return SerializeSignedInteger((sbyte)(object)input);
            }
            if (input is short) { // SInt16
                return SerializeSignedInteger((short)(object)input);
            }
            if (input is int) { // SInt32
                return SerializeSignedInteger((int)(object)input);
            }
            if (input is long) { // SInt64
                return SerializeSignedInteger((long)(object)input);
            }

            if (input is byte) { // UInt8
                return SerializeUnsignedInteger((byte)(object)input);
            }
            if (input is ushort) { // UInt16
                return SerializeUnsignedInteger((ushort)(object)input);
            }
            if (input is uint) { // UInt32
                return SerializeUnsignedInteger((uint)(object)input);
            }
            if (input is ulong) { // UInt64
                return SerializeUnsignedInteger((ulong)(object)input);
            }

            if (input is string) { // String
                return SerializeString((string)(object)input);
            }


            var type = input.GetType();


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

        internal static byte[] SerializeBoolean(bool input) {
            if (!input) {
                return new byte[] { };
            }

            return new byte[] { byte.MaxValue };
        }

        internal static byte[] SerializeSignedInteger(sbyte input) {
            if (input == 0) {
                return new byte[] { };
            }

            return new byte[] { (byte)input };
        }
        internal static byte[] SerializeSignedInteger(short input) {
            if (input <= sbyte.MaxValue && input >= sbyte.MinValue) {
                return Serialize((sbyte)input);
            }

            return BitConverter.GetBytes(input);
        }
        internal static byte[] SerializeSignedInteger(int input) {
            if (input <= short.MaxValue && input >= short.MinValue) {
                return Serialize((short)input);
            }

            return BitConverter.GetBytes(input);
        }
        internal static byte[] SerializeSignedInteger(long input) {
            if (input <= int.MaxValue && input >= int.MinValue) {
                return Serialize((int)input);
            }

            return BitConverter.GetBytes(input);
        }

        internal static byte[] SerializeUnsignedInteger(byte input) {
            if (input == 0) {
                return new byte[] { };
            }

            return new byte[] { input };
        }
        internal static byte[] SerializeUnsignedInteger(ushort input) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                return Serialize((byte)input);
            }

            return BitConverter.GetBytes(input);
        }
        internal static byte[] SerializeUnsignedInteger(uint input) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                return Serialize((byte)input);
            }
            if (input <= ushort.MaxValue && input >= ushort.MinValue) {
                return Serialize((ushort)input);
            }

            return BitConverter.GetBytes(input);
        }
        internal static byte[] SerializeUnsignedInteger(ulong input) {
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


        public static T Deserialize<T>(byte[] raw) {
            if (null == raw) {
                throw new ArgumentNullException("input");
            }
            var t = typeof(T);

            if (t == typeof(bool)) {
                return (T)(object)DeserializeAsBoolean(raw);
            }

            if (t == typeof(sbyte)) {
                return (T)(object)DeserializeAsSInt8(raw);
            }
            if (t == typeof(short)) {
                return (T)(object)DeserializeAsSInt16(raw);
            }
            if (t == typeof(int)) {
                return (T)(object)DeserializeAsSInt32(raw);
            }
            if (t == typeof(long)) {
                return (T)(object)DeserializeAsSInt64(raw);
            }

            if(t == typeof(byte)) {
                return (T)(object)DeserializeAsUInt8(raw);
            }
            if (t == typeof(ushort)) {
                return (T)(object)DeserializeAsUInt16(raw);
            }
            if (t == typeof(uint)) {
                return (T)(object)DeserializeAsUInt32(raw);
            }
            if (t == typeof(ulong)) {
                return (T)(object)DeserializeAsUInt64(raw);
            }

            if (t == typeof(string)) {
                return (T)(object)DeserializeString(raw);
            }
            /*
            var output = default(T);

            var type = typeof(T);


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
            }*/
            throw new NotImplementedException();

        }

        internal static bool DeserializeAsBoolean(byte[] input) {
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

        internal static sbyte DeserializeAsSInt8(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            switch (input.Length) {
                case 0: return 0;
                case 1: return (sbyte)input[0];
                default: throw new DataFormatException("SInt8 values can be 0 or 1 bytes.");
            }
        }
        internal static short DeserializeAsSInt16(byte[] input) {
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
        internal static int DeserializeAsSInt32(byte[] input) {
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
        internal static long DeserializeAsSInt64(byte[] input) {
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

        internal static byte DeserializeAsUInt8(byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            switch (input.Length) {
                case 0: return 0;
                case 1: return input[0];
                default: throw new DataFormatException("UInt8 values can be 0 or 1 bytes.");
            }
        }
        internal static ushort DeserializeAsUInt16(byte[] input) {
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
        internal static uint DeserializeAsUInt32(byte[] input) {
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
        internal static ulong DeserializeAsUInt64(byte[] input) {
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
