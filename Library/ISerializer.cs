using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Serialization {
    public interface ISerializer {
        void Serialize<T>(T value, Buffer<byte> buffer);
        T Deserialize<T>(Buffer<byte> input);
    }
}
