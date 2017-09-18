using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class ArrayCoder {
        private readonly VLQCodec VLQ = new VLQCodec();

        public Type Target { get { return typeof(IList); } }

        public static void Serialize(IList value, SerilizationOutput output) {
            var cast = (IList)value;

            var coder = CoderRegistry.Get(type.GenericTypeArguments[0]);

            // Iterate through each element
            foreach (var element in cast) {

                // Serialize value
                var v = coder.Serialize(type.GenericTypeArguments[0], element);
                VLQ.CompressUnsigned((ulong)v.Length, buffer);
                buffer.EnqueueArray(v);
            }
        }

        public object Deserialize(Type type, Buffer<byte> buffer) {
            // Deserialize temporarily as list
            var container = DeserializeList(innerType, payload);

            // Convert to array and return
            var output = Array.CreateInstance(type.GetElementType(), container.Count);
            container.CopyTo(output, 0);

            return output;
        }
    }
}
