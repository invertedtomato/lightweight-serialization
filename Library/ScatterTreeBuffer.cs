using System;

namespace InvertedTomato.IO.Buffers {
    public class ScatterTreeBuffer {
        public byte[] Payload { get; private set; }
        public ScatterTreeBuffer[] Children { get; private set; }

        public ScatterTreeBuffer(byte[] payload) {
#if DEBUG
            if (null == payload) {
                throw new ArgumentNullException(nameof(payload));
            }
#endif

            Payload = payload;
        }
        public ScatterTreeBuffer(ScatterTreeBuffer[] children) {
#if DEBUG
            if (null == children) {
                throw new ArgumentNullException(nameof(children));
            }
#endif

            Children = children;
        }
    }
}
