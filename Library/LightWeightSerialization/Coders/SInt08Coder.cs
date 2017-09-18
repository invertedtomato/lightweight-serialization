using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class SInt8Coder {
        private static VLQCodec VLQ = new VLQCodec();

        public static void Serialize(sbyte value, SerializationOutput output) {
            if (value == 0) {
                output.AddRaw(VLQCodec.Nil);
            } else {
                output.AddRawArray(new byte[] { VLQCodec.Nil+1, (byte)value });
            }
        }

        public static sbyte Deserialize(Buffer<byte> buffer) {
            var length = (int)VLQ.DecompressUnsigned(buffer);
            var subBuffer = buffer.DequeueBuffer(length);

            switch (length) {
                case 0: return 0;
                case 1: return (sbyte)subBuffer.Dequeue();
                default: throw new DataFormatException("SInt64 values can be 0 or 1 bytes.");
            }
        }
    }
}
