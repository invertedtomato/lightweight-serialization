using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class SInt16Coder {
        public static void Serialize(short value, SerializationOutput output) {
            if (value <= sbyte.MaxValue && value >= sbyte.MinValue) {
                SInt8Coder.Serialize((sbyte)value, output);
            } else {
                output.AddRaw(VLQCodec.Nil + 2);
                output.AddRawArray(BitConverter.GetBytes(value));
            }
        }

        public object Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                case 2: return BitConverter.ToInt16(buffer.DequeueBuffer(2).ToArray(), 0);
                default: throw new DataFormatException("SInt64 values can be 0, 1 or 2 bytes.");
            }
        }
    }
}
