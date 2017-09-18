using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class UInt64Coder {
        public static void Serialize(ulong value, SerializationOutput output) {
            if (value <= uint.MaxValue) {
                UInt32Coder.Serialize((uint)value, output);
            } else { // TODO: 5, 6, 7 byte encoding
                output.AddRaw(VLQCodec.Nil + 8);
                output.AddRawArray(BitConverter.GetBytes(value));
            }
        }

        public object Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                case 2: return BitConverter.ToUInt16(buffer.DequeueBuffer(2).ToArray(), 0);
                // TODO: 3
                case 4: return BitConverter.ToUInt32(buffer.DequeueBuffer(4).ToArray(), 0);
                // TODO 5,6,7
                case 8: return BitConverter.ToUInt64(buffer.DequeueBuffer(8).ToArray(), 0);
                default: throw new DataFormatException("UInt64 values can be 0, 1, 2, 4 or 8 bytes.");
            }
        }
    }
}
