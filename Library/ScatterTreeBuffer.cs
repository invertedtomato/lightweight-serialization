using System;
using System.Linq;

namespace InvertedTomato.IO.Buffers {
	public class ScatterTreeBuffer {
		public static ScatterTreeBuffer Empty = new ScatterTreeBuffer(new Byte[] { });

		public ScatterTreeBuffer(Byte[] payload) {
#if DEBUG
			if (null == payload) {
				throw new ArgumentNullException(nameof(payload));
			}
#endif

			Payload = payload;
			Length = payload.Length;
			Count = 1;
		}

		public ScatterTreeBuffer(ScatterTreeBuffer[] children) {
#if DEBUG
			if (null == children) {
				throw new ArgumentNullException(nameof(children));
			}
#endif

			Children = children;
			Length = children.Sum(a => a.Length);
			Count = children.Sum(a => a.Count);
		}

		public Byte[] Payload { get; }
		public ScatterTreeBuffer[] Children { get; }
		public Int32 Count { get; }
		public Int32 Length { get; }
	}
}