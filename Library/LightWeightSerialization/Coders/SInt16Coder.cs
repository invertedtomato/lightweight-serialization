using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class SInt16Coder {
        public static ScatterTreeBuffer Serialize(short value) {
            if (value <= sbyte.MaxValue && value >= sbyte.MinValue) {
                return SInt8Coder.Serialize((sbyte)value);
            } else {
                return new ScatterTreeBuffer(BitConverter.GetBytes(value));
            }
        }

        public static short Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                case 2: return BitConverter.ToInt16(buffer.GetUnderlying(), buffer.Start);
                default: throw new DataFormatException("SInt64 values can be 0, 1 or 2 bytes.");
            }
        }
    }
}
