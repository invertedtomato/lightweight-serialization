using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public static class SignedVlq {
		public static Byte[] Encode(Int64 value) {
			return UnsignedVlq.Encode((UInt64) ((value << 1) ^ (value >> 63)));
		}

		public static Int64 Decode(Stream input) {
			var casted = (Int64) UnsignedVlq.Decode(input);
			return (casted >> 1) ^ -(casted & 1);
		}
	}
}