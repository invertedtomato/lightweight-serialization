using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Serialization {
    public interface ISerializer {
        void Encode<T>(T value, Buffer<byte> buffer);
        T Decode<T>(Buffer<byte> input);
    }
}
