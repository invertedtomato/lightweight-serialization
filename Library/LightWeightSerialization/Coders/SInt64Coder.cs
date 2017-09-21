using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class SInt64Coder {
        public static ScatterTreeBuffer Serialize(long value) {
            if (value <= int.MaxValue && value >= int.MinValue) {
                return SInt32Coder.Serialize((int)value);
            } else { // TODO: 5, 6, 7 byte encoding
                return new ScatterTreeBuffer(BitConverter.GetBytes(value));
            }
        }

        public static long Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                case 2: return BitConverter.ToInt16(buffer.GetUnderlying(), buffer.Start);
                // TODO: 3
                case 4: return BitConverter.ToInt32(buffer.GetUnderlying(), buffer.Start);
                // TODO 5,6,7
                case 8: return BitConverter.ToInt64(buffer.GetUnderlying(), buffer.Start);
                default: throw new DataFormatException("SInt64 values can be 0, 1, 2, 4 or 8 bytes.");
            }
        }
    }
}
