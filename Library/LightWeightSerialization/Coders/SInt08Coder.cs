using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class SInt8Coder {
        private static VLQCodec VLQ = new VLQCodec();

        public static ScatterTreeBuffer Serialize(sbyte value) {
            if (value == 0) {
                return ScatterTreeBuffer.Empty;
            } else {
                return new ScatterTreeBuffer(new byte[] { (byte)value });
            }
        }

        public static sbyte Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return (sbyte)buffer.Dequeue();
                default: throw new DataFormatException("SInt64 values can be 0 or 1 bytes.");
            }
        }
    }
}
