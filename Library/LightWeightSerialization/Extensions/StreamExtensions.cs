using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization.Extensions {
	public static class StreamExtensions {
		public static Byte[] Read(this Stream target, Int32 count) {
			var buffer = new Byte[count];

			target.Read(buffer, 0, count);

			return buffer;
		}

		public static void Write(this Stream target, Byte[] input) {
			target.Write(input, 0, input.Length);
		}
	}
}