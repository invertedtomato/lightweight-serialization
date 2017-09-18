using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class UInt32Coder {
        public static void Serialize(uint value, SerializationOutput output) {
            if (value <= ushort.MaxValue) {
                UInt16Coder.Serialize((ushort)value, output);
            } else {// TODO: 3 byte encoding
                output.AddRaw(VLQCodec.Nil + 4);
                output.AddRawArray(BitConverter.GetBytes(value));
            }
        }

        public object Deserialize(Buffer<byte> buffer) {
            switch (buffer.Readable) {
                case 0: return 0;
                case 1: return buffer.Dequeue();
                case 2: return BitConverter.ToUInt16(buffer.DequeueBuffer(2).ToArray(), 0);
                // TODO: 3
                case 4: return BitConverter.ToUInt32(buffer.DequeueBuffer(4).ToArray(), 0);
                default: throw new DataFormatException("UInt32 values can be 0, 1, 2 or 4 bytes.");
            }
        }
    }
}
