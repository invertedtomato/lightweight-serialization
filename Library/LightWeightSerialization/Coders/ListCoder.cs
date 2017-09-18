using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class ListCoder {
        public static void Serialize(IList value, SerializationOutput output) {
            /*
            var allocateId = output.Allocate();
            var initialLength = output.Length;

            // Iterate through each element
            foreach (var element in value) {
                // Serialize value

                output.AddRawArray(Serialize(element));
            }
            output.SetVLQ(allocateId, (ulong)(output.Length - initialLength));*/
            throw new NotImplementedException();
        }

        public object Deserialize(Type type, Buffer<byte> buffer) {
            throw new NotImplementedException();
            /*

            // Instantiate list
            var output = (IList)Activator.CreateInstance(type);//typeof(List<>).MakeGenericType(type.GenericTypeArguments)

            // Deserialize items
            while (buffer.IsReadable) {
                // Deserialize value
                var l = (int)VLQ.DecompressUnsigned(buffer);
                var b = buffer.DequeueBuffer(l);
                var v = Deserialize(b, type.GenericTypeArguments[0]);

                // Add to output
                output.Add(v);
            }

            return output;*/
        }

    }
}
