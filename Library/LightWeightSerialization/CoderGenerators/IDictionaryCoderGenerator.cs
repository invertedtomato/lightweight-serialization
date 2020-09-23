using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.CoderGenerators
{
    public class IDictionaryCoderGenerator : ICoderGenerator
    {
        // Precompute null value for performance
        private static readonly EncodeBuffer Null = new EncodeBuffer(UnsignedVlq.Encode(0));

        public Boolean IsCompatibleWith<T>()
        {
            return typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T));
        }

        public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse)
        {
            // Get serializer for sub items
            var keyEncoder = recurse(type.GenericTypeArguments[0]);
            var valueEncoder = recurse(type.GenericTypeArguments[1]);

            return new Func<IDictionary, EncodeBuffer>(value =>
            {
                // Handle nulls
                if (null == value)
                {
                    return Null;
                }

                // Serialize elements   
                var output = new EncodeBuffer();
                var e = value.GetEnumerator();
                UInt64 count = 0;
                while (e.MoveNext())
                {
                    output.Append((EncodeBuffer)keyEncoder.DynamicInvokeTransparent(e.Key));
                    output.Append((EncodeBuffer)valueEncoder.DynamicInvokeTransparent(e.Value));
                    count++;
                }

                // Encode length
                output.SetFirst(UnsignedVlq.Encode(count + 1));

                return output;
            });
        }

        public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse)
        {
            // Get deserializer for sub items
            var keyDecoder = recurse(type.GenericTypeArguments[0]);
            var valueDecoder = recurse(type.GenericTypeArguments[1]);

            return new Func<DecodeBuffer, IDictionary>(input =>
            {
                // Read header
                var header = UnsignedVlq.Decode(input);

                if (header == 0)
                {
                    return null;
                }

                // Get count
                var count = (Int32)header - 1;

                // Instantiate dictionary
                var output = (IDictionary)Activator.CreateInstance(type);

                // Loop through input buffer until depleted
                for (var i = 0; i < count; i++)
                {
                    // Deserialize key
                    var keyValue = keyDecoder.DynamicInvokeTransparent(input);

                    // Deserialize value
                    var valueValue = valueDecoder.DynamicInvokeTransparent(input);

                    // Add to output
                    output[keyValue] = valueValue;
                }

                return output;
            });
        }
    }
}