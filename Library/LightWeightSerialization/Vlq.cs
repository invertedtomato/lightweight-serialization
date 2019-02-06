using System;
using System.IO;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public static class Vlq {
		private static readonly VLQCodec VLQ = new VLQCodec();

		public static Byte[] Encode(UInt64 value) {
			using (var stream = new MemoryStream()) {
				VLQ.CompressUnsigned(stream, value);
				stream.Seek(0, SeekOrigin.Begin);
				return stream.ToArray();
			}
		}

		public static UInt64 Decode(Stream input) {
			VLQ.DecompressUnsigned(input, out var output);
			return output;
		}
	}
}