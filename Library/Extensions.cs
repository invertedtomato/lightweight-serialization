using InvertedTomato.IO.Buffers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace InvertedTomato.LightWeightSerialization {
    internal static class Extensions {

        /// <summary>
        /// Enqueue a set of values, resizing the buffer if required.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="values"></param>
        /// <returns>Resized buffer</returns>
        public static Buffer<byte> EnqueueArrayWithResize(this Buffer<byte> target, params byte[][] values) {
#if DEBUG
            if (null == target) {
                throw new ArgumentNullException("target");
            }
#endif

            // Calculate total size required
            var totalSize = values.Sum(a => a.Length);

            // Increase buffer size if needed
            if (target.Writable < totalSize) {
                target = target.Resize(Math.Max(target.Capacity * 2, target.Readable + totalSize));
            }

            // Write all values
            foreach (var value in values) {
                target.EnqueueArray(value);
            }

            // Return potentially new buffer
            return target;
        }
    }
}
