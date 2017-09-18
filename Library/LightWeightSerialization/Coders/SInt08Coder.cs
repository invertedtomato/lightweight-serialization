using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class SInt8Coder {
        public static void Serialize(sbyte value, SerializationOutput output) {
            if (value == 0) {
                output.AddRaw(VLQCodec.Nil);
            } else {
                output.AddRawArray(new byte[] { VLQCodec.Nil+1, (byte)value });
            }
        }

        public object Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                default: throw new DataFormatException("SInt64 values can be 0 or 1 bytes.");
            }
        }
    }
}
