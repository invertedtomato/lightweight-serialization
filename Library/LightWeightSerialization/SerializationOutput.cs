using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Collections.Generic;
using System.Text;

namespace InvertedTomato.Serialization.LightWeightSerialization {
    public class SerializationOutput {
        private static readonly VLQCodec VLQ = new VLQCodec();
        private readonly List<byte[]> Elements = new List<byte[]>();

        public int Length { get; private set; }

        public int Allocate() {
            Elements.Add(null);
            return Elements.Count - 1;
        }

        public void SetVLQ(int allocationId, ulong data) {
#if DEBUG
            if (Elements[allocationId] != null) {
                throw new ArgumentException("Invalid allocationId provided.", nameof(allocationId));
            }
#endif
            var array = VLQ.CompressUnsigned(data).ToArray();
            Elements[allocationId] = array;
            Length += array.Length;
        }

        public void AddArray(byte[] value) {
#if DEBUG
            if (null == value) {
                throw new ArgumentNullException(nameof(value));
            }
#endif

            var length = VLQ.CompressUnsigned(value.Length).ToArray();
            AddRawArray(length);
            AddRawArray(value);
        }

        public void AddRawArray(byte[] value) {
#if DEBUG
            if (null == value) {
                throw new ArgumentNullException(nameof(value));
            }
#endif

            Elements.Add(value);
            Length += value.Length;
        }

        public void AddRaw(byte value) {
            Elements.Add(new byte[] { value });
            Length++;
        }

        public void Generate(Buffer<byte> buffer) {
#if DEBUG
            if (null == buffer) {
                throw new ArgumentNullException(nameof(buffer));
            }
#endif

            // Grow buffer if needed
            var spaceNeeded = Length - buffer.Writable;
            if (spaceNeeded > 0) {
                buffer.Grow(spaceNeeded);
            }

            // Add all elements, skipping the first VLQ-encoded length
            var isHeadGone = false;
            foreach (var element in Elements) {
#if DEBUG
                if (null == element) {
                    throw new InvalidOperationException("Cannot generate while their is uncompleted allocations");
                }
#endif
                if (isHeadGone) {
                    buffer.EnqueueArray(element);
                } else {
                    for (var i = 0; i < element.Length; i++) {
                        if ((element[i] & VLQCodec.Nil) > 0) {
                            buffer.EnqueueArray(element, i + 1);
                            isHeadGone = true;
                            break;
                        }
                    }
                }
            }
        }

        public byte[] ToArray() { // Used for testing
            var output = new byte[Length];
            var pos = 0;

            foreach (var element in Elements) {
#if DEBUG
                if (null == element) {
                    throw new InvalidOperationException("Cannot generate while their is uncompleted allocations");
                }
#endif
                Buffer.BlockCopy(element, 0, output, pos, element.Length);
                pos += element.Length;
            }

            return output;
        }
    }
}
