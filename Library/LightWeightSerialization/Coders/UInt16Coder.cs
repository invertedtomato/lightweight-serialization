using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class UInt16Coder {
        private static VLQCodec VLQ = new VLQCodec();

        public static ScatterTreeBuffer Serialize(ushort value) {
            if (value <= byte.MaxValue) {
                return UInt8Coder.Serialize((byte)value);
            } else {
                return new ScatterTreeBuffer(BitConverter.GetBytes(value));
            }
        }

        public static ushort Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                case 2: return BitConverter.ToUInt16(buffer.GetUnderlying(), buffer.Start);
                default: throw new DataFormatException("UInt64 values can be 0, 1 or 2 bytes.");
            }
        }
    }
}
