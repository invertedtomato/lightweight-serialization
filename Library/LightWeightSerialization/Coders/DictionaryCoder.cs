using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class DictionaryCoder {
        private readonly VLQCodec VLQ = new VLQCodec();

        public Type Target { get { return typeof(IDictionary); } }

        public void Serialize(object value, Buffer<byte> buffer) {
            var cast = value as IDictionary;

            // Enumerate all values
            var e = cast.GetEnumerator();
            while (e.MoveNext()) {
                // Serialize key
                var k = Serialize(e.Key);
                VLQ.CompressUnsigned((ulong)k.Length, buffer);
                buffer.EnqueueArray(k);

                // Serialize value
                var v = Serialize(e.Value);
                VLQ.CompressUnsigned((ulong)v.Length, buffer);
                buffer.EnqueueArray(v);
            }

            return;
        }

        public object Deserialize(Type type, Buffer<byte> buffer) {
            // Instantiate dictionary
            var output = (IDictionary)Activator.CreateInstance(type); // typeof(Dictionary<,>).MakeGenericType(type.GenericTypeArguments)

            // Loop through input buffer until depleated
            while (buffer.IsReadable) {
                // Deserialize key
                var kl = (int)VLQ.DecompressUnsigned(buffer);
                var kb = buffer.DequeueBuffer(kl);
                var k = Deserialize(kb, type.GenericTypeArguments[0]);

                // Deserialize value
                var vl = (int)VLQ.DecompressUnsigned(buffer);
                var vb = buffer.DequeueBuffer(vl);
                var v = Deserialize(vb, type.GenericTypeArguments[1]);

                // Add to output
                output[k] = v;
            }

            return output;
        }

    }
}
