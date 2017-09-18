using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class UInt16Coder {
        private static VLQCodec VLQ = new VLQCodec();

        public static void Serialize(ushort value, SerializationOutput output) {
            if (value <= byte.MaxValue) {
                UInt8Coder.Serialize((byte)value, output);
            } else {
                output.AddRaw(VLQCodec.Nil + 2);
                output.AddRawArray(BitConverter.GetBytes(value));
            }
        }

        public static ushort Deserialize(Buffer<byte> buffer) {
            var length = (int)VLQ.DecompressUnsigned(buffer);
            var subBuffer = buffer.DequeueBuffer(length);

            switch (length) {
                case 0: return 0;
                case 1: return subBuffer.Dequeue();
                case 2: return BitConverter.ToUInt16(subBuffer.GetUnderlying(), subBuffer.Start);
                default: throw new DataFormatException("UInt64 values can be 0, 1 or 2 bytes.");
            }
        }
    }
}
