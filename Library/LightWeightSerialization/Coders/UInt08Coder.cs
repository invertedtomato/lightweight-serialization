using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class UInt8Coder {
        public static void Serialize(byte value, SerializationOutput output) {
            if (value == 0) {
                output.AddRaw(VLQCodec.Nil);
            } else {
                output.AddRawArray(new byte[] { VLQCodec.Nil + 1, value });
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
