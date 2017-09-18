using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class UInt16Coder {
        public static void Serialize(ushort value, SerializationOutput output) {
            if (value <= byte.MaxValue) {
                UInt8Coder.Serialize((byte)value, output);
            } else {
                output.AddRaw(VLQCodec.Nil + 2);
                output.AddRawArray(BitConverter.GetBytes(value));
            }
        }

        public object Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                case 2: return BitConverter.ToUInt16(buffer.DequeueBuffer(2).ToArray(), 0);
                default: throw new DataFormatException("UInt64 values can be 0, 1 or 2 bytes.");
            }
        }
    }
}
