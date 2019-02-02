using System;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Serialization {
	public interface ISerializer {
		void Encode<T>(T value, Buffer<Byte> buffer);
		T Decode<T>(Buffer<Byte> input);
	}
}