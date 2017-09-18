using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Text;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class StringCoder {
        private static VLQCodec VLQ = new VLQCodec();

        public static void Serialize(string value, SerializationOutput output) {
            if (null == value) {
                output.AddRaw(VLQCodec.Nil);
            } else {
                output.AddArray(Encoding.UTF8.GetBytes(value));
            }
        }

        public static string Deserialize(Buffer<byte> buffer) {
            var length = (int)VLQ.DecompressUnsigned(buffer);
            var subBuffer = buffer.DequeueBuffer(length);
            
            return Encoding.UTF8.GetString(subBuffer.GetUnderlying(), subBuffer.Start, subBuffer.Readable);
        }
    }
}
