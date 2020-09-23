using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.CoderGenerators
{
    public class IListCoderGenerator : ICoderGenerator
    {
        // Precompute null value for performance
        private static readonly EncodeBuffer Null = new EncodeBuffer(UnsignedVlq.Encode(0));

        public Boolean IsCompatibleWith<T>()
        {
            // This explicitly does not support arrays (otherwise they could get matched with the below check)
            if (typeof(T).IsArray)
            {
                return false;
            }

            return typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T));
        }

        public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse)
        {
            // Get serializer for sub items
            var valueEncoder = recurse(type.GenericTypeArguments[0]);

            return new Func<IList, EncodeBuffer>(value =>
            {
                // Handle nulls
                if (null == value)
                {
                    return Null;
                }

                // Serialize elements
                var output = new EncodeBuffer();
                foreach (var element in value)
                {
                    output.Append((EncodeBuffer)valueEncoder.DynamicInvokeTransparent(element));
                }

                // Encode length
                output.SetFirst(UnsignedVlq.Encode((UInt64)value.Count + 1));

                return output;
            });
        }

        public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse)
        {
            // Get deserializer for sub items
            var valueDecoder = recurse(type.GenericTypeArguments[0]);

            return new Func<DecodeBuffer, IList>(input =>
            {
                // Read header
                var header = UnsignedVlq.Decode(input);

                // Handle nulls
                if (header == 0)
                {
                    return null;
                }

                // Determine length
                var count = (Int32)header - 1;

                // Instantiate list
                var output = (IList)Activator.CreateInstance(type); //typeof(List<>).MakeGenericType(type.GenericTypeArguments)

                // Deserialize until we reach length limit
                for (var i = 0; i < count; i++)
                {
                    // Deserialize element
                    var element = valueDecoder.DynamicInvokeTransparent(input);

                    // Add to output
                    output.Add(element);
                }

                return output;
            });
        }
    }
}