using System.IO;

namespace InvertedTomato.Serialization {
	public interface ISerializer {
		void Encode<T>(T value, Stream output);
		T Decode<T>(Stream input);
	}
}