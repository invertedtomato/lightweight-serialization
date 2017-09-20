using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class SInt32Coder {
        private static VLQCodec VLQ = new VLQCodec();

        public static void Serialize(int value, SerializationOutput output) {
            if (value <= short.MaxValue && value >= short.MinValue) {
                SInt16Coder.Serialize((short)value, output);
            } else { // TODO: 3-byte encoding
                output.AddRaw(VLQCodec.Nil + 4);
                output.AddRawArray(BitConverter.GetBytes(value));
            }
        }

        public static int Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                case 2: return BitConverter.ToInt16(buffer.GetUnderlying(), buffer.Start);
                // TODO: 3
                case 4: return BitConverter.ToInt32(buffer.GetUnderlying(), buffer.Start);
                default: throw new DataFormatException("SInt32 values can be 0, 1, 2 or 4 bytes.");
            }
        }
    }
}
