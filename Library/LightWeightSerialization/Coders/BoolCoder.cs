using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public static class BoolCoder {
        public static void Serialize(bool value, SerializationOutput output) {
            if (value) {
                output.AddRawArray(new byte[] { VLQCodec.Nil + 1, 0xff });
            } else {
                output.AddRaw(VLQCodec.Nil);
            }
        }

        public static bool Deserialize(Type type, Buffer<byte> buffer) {
            if (buffer.Readable == 0) {
                return false;
            }
#if DEBUG
            if (buffer.Readable > 1) {
                throw new DataFormatException("Boolean values can be no more than 1 byte long.");
            }
#endif
            if (buffer.Dequeue() != byte.MaxValue) {
                throw new DataFormatException("Boolean values cannot be anything other than 0xFF.");
            }

            return true;
        }

    }
}
