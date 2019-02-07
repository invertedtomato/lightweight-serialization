using System;
using System.Linq;

namespace Tests {
	public static class Extensions {
		public static Byte[] ParseAsHex(this String target) {
			return Enumerable.Range(0, target.Length)
				.Where(x => x % 2 == 0)
				.Select(x => Convert.ToByte(target.Substring(x, 2), 16))
				.ToArray();
		}
	}
}