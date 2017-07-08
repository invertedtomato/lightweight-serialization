using InvertedTomato.IO.Buffers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.LightWeightSerialization {
    public static class LightWeight {
        private const byte MSB = 0x80;
        private static readonly byte[] Blank = new byte[] { };
        private static readonly Type PropertyAttribute = typeof(LightWeightPropertyAttribute);

        public static byte[] Serialize(bool input) {
            if (!input) {
                return new byte[] { };
            }

            return new byte[] { byte.MaxValue };
        }

        public static byte[] Serialize(sbyte input) {
            if (input == 0) {
                return new byte[] { };
            }

            return new byte[] { (byte)input };
        }
        public static byte[] Serialize(short input) {
            if (input <= sbyte.MaxValue && input >= sbyte.MinValue) {
                return Serialize((sbyte)input);
            }

            return BitConverter.GetBytes(input);
        }
        public static byte[] Serialize(int input) {
            if (input <= short.MaxValue && input >= short.MinValue) {
                return Serialize((short)input);
            }

            return BitConverter.GetBytes(input);
        }
        public static byte[] Serialize(long input) {
            if (input <= int.MaxValue && input >= int.MinValue) {
                return Serialize((int)input);
            }

            return BitConverter.GetBytes(input);
        }

        public static byte[] Serialize(byte input) {
            if (input == 0) {
                return new byte[] { };
            }

            return new byte[] { input };
        }
        public static byte[] Serialize(ushort input) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                return Serialize((byte)input);
            }

            return BitConverter.GetBytes(input);
        }
        public static byte[] Serialize(uint input) {
            if (input <= byte.MaxValue && input >= byte.MinValue) {
                return Serialize((byte)input);
            }
            if (input <= ushort.MaxValue && input >= ushort.MinValue) {
                return Serialize((ushort)input);
            }

            return BitConverter.GetBytes(input);
        }
        public static byte[] Serialize(ulong input) {
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

        public static byte[] Serialize(string input) {
            if (null == input) {
                return new byte[] { };
            }

            return Encoding.UTF8.GetBytes(input);
        }

        public static byte[] Serialize<T>(T input) {
            // Serialize input to byte array
            if (input == null) { // Null
                return new byte[] { };
            }

            if (input is bool) { // Boolean
                return Serialize((bool)(object)input);
            }

            if (input is sbyte) { // SInt8
                return Serialize((sbyte)(object)input);
            }
            if (input is short) { // SInt16
                return Serialize((short)(object)input);
            }
            if (input is int) { // SInt32
                return Serialize((int)(object)input);
            }
            if (input is long) { // SInt64
                return Serialize((long)(object)input);
            }

            if (input is byte) { // UInt8
                return Serialize((byte)(object)input);
            }
            if (input is ushort) { // UInt16
                return Serialize((ushort)(object)input);
            }
            if (input is uint) { // UInt32
                return Serialize((uint)(object)input);
            }
            if (input is ulong) { // UInt64
                return Serialize((ulong)(object)input);
            }

            if (input is string) { // String
                return Serialize((string)(object)input);
            }


            var type = input.GetType();

            // TODO: abort if type isn't serializable?

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

        public static T Deserialize<T>(byte[] raw) {
            if (null == raw) {
                throw new ArgumentNullException("input");
            }

            throw new NotImplementedException();
        }

    }
}
