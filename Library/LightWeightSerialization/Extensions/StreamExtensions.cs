using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization.Extensions {
	public static class StreamExtensions {
		public static Byte[] Read(this Stream stream, Int32 count) {
			var buffer = new Byte[count];

			stream.Read(buffer, 0, count);

			return buffer;
		}
	}
}