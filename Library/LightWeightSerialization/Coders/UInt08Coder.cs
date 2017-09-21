using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class UInt8Coder {
        private static VLQCodec VLQ = new VLQCodec();

        public static ScatterTreeBuffer Serialize(byte value) {
            if (value == 0) {
                return ScatterTreeBuffer.Empty;
            } else {
                return new ScatterTreeBuffer(new byte[] { value });
            }
        }

        public static byte Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                default: throw new DataFormatException("UInt64 values can be 0 or 1 bytes.");
            }
        }
    }
}
