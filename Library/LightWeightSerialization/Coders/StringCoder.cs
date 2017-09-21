using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Text;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class StringCoder {
        public static ScatterTreeBuffer Serialize(string value) {
            if (null == value) {
                return ScatterTreeBuffer.Empty;
            } else {
                return new ScatterTreeBuffer(Encoding.UTF8.GetBytes(value));
            }
        }

        public static string Deserialize(Buffer<byte> buffer) {
            return Encoding.UTF8.GetString(buffer.GetUnderlying(), buffer.Start, buffer.Readable);
        }
    }
}
