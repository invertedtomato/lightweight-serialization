using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class UInt32Coder {
        private static VLQCodec VLQ = new VLQCodec();

        public static ScatterTreeBuffer Serialize(uint value) {
            if (value <= ushort.MaxValue) {
               return  UInt16Coder.Serialize((ushort)value);
            } else {// TODO: 3 byte encoding
                return new ScatterTreeBuffer(BitConverter.GetBytes(value));
            }
        }

        public static uint Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                case 2: return BitConverter.ToUInt16(buffer.GetUnderlying(), buffer.Start);
                // TODO: 3
                case 4: return BitConverter.ToUInt32(buffer.GetUnderlying(), buffer.Start);
                default: throw new DataFormatException("UInt32 values can be 0, 1, 2 or 4 bytes.");
            }
        }
    }
}
