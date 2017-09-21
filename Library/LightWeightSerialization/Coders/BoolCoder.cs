using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public static class BoolCoder {
        public static ScatterTreeBuffer Serialize(bool value) {
            if (value) {
                return new ScatterTreeBuffer(new byte[] { 0x00 });
            } else {
                return new ScatterTreeBuffer(new byte[] { });
            }
        }

        public static bool Deserialize(Buffer<byte> buffer) {
            if (buffer.Readable == 0) {
                return false;
            }
#if DEBUG
            if (buffer.Readable > 1) {
                throw new DataFormatException("Boolean values can be no more than 1 byte long.");
            }
#endif
            if (buffer.Dequeue() != 0x00) {
                throw new DataFormatException("Boolean values cannot be anything other than 0x00.");
            }

            return true;
        }

    }
}
