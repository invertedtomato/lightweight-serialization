using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Serialization {
    public interface ISerializer {
        Buffer<byte> Serialize(object value, Type type, Buffer<byte> output);
        object Deserialize(Buffer<byte> input, Type type);
    }
}
