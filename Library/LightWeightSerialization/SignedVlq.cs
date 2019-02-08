using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public static class SignedVlq {
		public const Int64 MinValue = Int64.MinValue + 1;
		public const Int64 MaxValue = Int64.MaxValue ;

		public static Byte[] Encode(Int64 value) {
			return UnsignedVlq.Encode((UInt64) ((value << 1) ^ (value >> 63)));
		}

		public static Int64 Decode(Byte[] input) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}
#endif
			using (var stream = new MemoryStream(input)) {
				return Decode(stream);
			}
		}

		public static Int64 Decode(Stream input) {
			var casted = (Int64) UnsignedVlq.Decode(input);
			return (casted >> 1) ^ -(casted & 1);
		}
	}
}