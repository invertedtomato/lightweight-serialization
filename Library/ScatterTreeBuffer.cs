using System;
using System.Linq;

namespace InvertedTomato.IO.Buffers {
    public class ScatterTreeBuffer {
        public static ScatterTreeBuffer Empty = new ScatterTreeBuffer(new byte[] { });

        public byte[] Payload { get; private set; }
        public ScatterTreeBuffer[] Children { get; private set; }
        public int Count { get; private set; }
        public int Length { get; private set; }

        public ScatterTreeBuffer(byte[] payload) {
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
    }
}
