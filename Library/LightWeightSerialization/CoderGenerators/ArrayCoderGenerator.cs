using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization.CoderGenerators
{
    public class ArrayCoderGenerator : ICoderGenerator
    {
        // Precompute null value for performance
        private static readonly EncodeBuffer Null = new EncodeBuffer(UnsignedVlq.Encode(0));

        public Boolean IsCompatibleWith<T>()
        {
            return typeof(T).IsArray;
        }

        public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse)
        {
            // Get serializer for sub items
            var valueEncoder = recurse(type.GetElementType());

            return new Func<Array, EncodeBuffer>(value =>
            {
                // Handle nulls
                if (null == value)
                {
                    return Null;
                }

                // Serialize elements
                var output = new EncodeBuffer();
                foreach (var subValue in value)
                {
                    output.Append((EncodeBuffer)valueEncoder.DynamicInvokeTransparent(subValue));
                }

                // Encode length
                output.SetFirst(UnsignedVlq.Encode((UInt64)value.Length + 1)); // Number of elements, not number of bytes

                return output;
            });
        }

        public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse)
        {
            // Get deserializer for sub items
            var valueDecoder = recurse(type.GetElementType());

            return new Func<DecodeBuffer, Array>(input =>
            {
                var header = UnsignedVlq.Decode(input);

                if (header == 0)
                {
                    return null;
                }

                // Determine length
                var length = (Int32)header - 1;

                // Instantiate list
                var container = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GetElementType()));

                // Deserialize until we reach length limit
                for (var i = 0; i < length; i++)
                {
                    // Deserialize element
                    var element = valueDecoder.DynamicInvokeTransparent(input);

                    // Add to output
                    container.Add(element);
                }

                // Convert to array and return
                var output = Array.CreateInstance(type.GetElementType(), container.Count);
                container.CopyTo(output, 0);

                return output;
            });
        }
    }
}