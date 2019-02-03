using System;
using System.IO;

namespace InvertedTomato.Serialization {
	public interface ISerializer {
		Int32 Encode<T>(T value, Stream output);
		T Decode<T>(Stream input, Int32 count);
	}
}