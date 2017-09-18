using InvertedTomato.IO.Buffers;
using System;
using System.Text;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class StringCoder {
        public static void Serialize(string value, SerializationOutput output) {
            output.AddArray(Encoding.UTF8.GetBytes(value));
        }

        public object Deserialize(Buffer<byte> buffer) {
            return Encoding.UTF8.GetString(buffer.GetUnderlying(), buffer.Start, buffer.Readable);
        }
    }
}
