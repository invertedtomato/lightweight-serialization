using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class SInt16Coder {
        private static VLQCodec VLQ = new VLQCodec();

        public static void Serialize(short value, SerializationOutput output) {
            if (value <= sbyte.MaxValue && value >= sbyte.MinValue) {
                SInt8Coder.Serialize((sbyte)value, output);
            } else {
                output.AddRaw(VLQCodec.Nil + 2);
                output.AddRawArray(BitConverter.GetBytes(value));
            }
        }

        public static short Deserialize(Buffer<byte> buffer) {
            var length = (int)VLQ.DecompressUnsigned(buffer);
            var subBuffer = buffer.DequeueBuffer(length);

            switch (length) {
                case 0: return 0;
                case 1: return subBuffer.Dequeue();
                case 2: return BitConverter.ToInt16(subBuffer.GetUnderlying(), subBuffer.Start);
                default: throw new DataFormatException("SInt64 values can be 0, 1 or 2 bytes.");
            }
        }
    }
}
