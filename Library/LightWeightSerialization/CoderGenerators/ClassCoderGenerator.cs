using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.CoderGenerators
{
    public class ClassCoderGenerator : ICoderGenerator
    {
        // Precompute values for performance
        private static readonly EncodeBuffer Null = new EncodeBuffer(UnsignedVlq.Encode(0));
        private static readonly EncodeBuffer One = new EncodeBuffer(UnsignedVlq.Encode(1));

        public Boolean IsCompatibleWith<T>()
        {
            // This explicitly does not support lists or dictionaries
            if (typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T)) ||
                typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T)))
            {
                // TODO: Instead only accept classes with a specific attribute
                return false;
            }

            return typeof(T).GetTypeInfo().IsClass;
        }

        public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse)
        {
            // Find all properties decorated with LightWeightProperty attribute
            var fields = new FieldInfo[Byte.MaxValue]; // Index => Field
            var coders = new Delegate[Byte.MaxValue]; // Index => Encoder/Decoder

            var fieldCount = -1;
            foreach (var property in type.GetRuntimeFields())
            {
                // Get property attribute which tells us the properties' index
                var attribute = (LightWeightPropertyAttribute)property.GetCustomAttribute(typeof(LightWeightPropertyAttribute), false);
                if (null == attribute)
                {
                    // No attribute found, skip
                    continue;
                }

                // Check for duplicate index
                if (null != fields[attribute.Index])
                {
                    throw new DuplicateIndexException($"The index {fields[attribute.Index]} is already used and cannot be reused.");
                }

                // Note the max index used
                if (attribute.Index > fieldCount)
                {
                    fieldCount = attribute.Index;
                }

                // Find/create encoder
                var encoder = recurse(property.FieldType);

                // Store property in lookup
                fields[attribute.Index] = property;
                coders[attribute.Index] = encoder;
            }

            // If no properties, shortcut the whole thing and return a blank
            if (fieldCount == -1)
            {
                return new Func<Object, EncodeBuffer>(value =>
                {
                    // Handle nulls
                    if (null == value)
                    {
                        return Null;
                    }

                    return One;
                });
            }

            // Check that no indexes have been missed
            for (var i = 0; i < fieldCount; i++)
            {
                if (null == fields[i])
                {
                    throw new MissingIndexException($"Indexes must not be skipped, however missing index {i}."); // TODO: Make so indexes can be skipped for easier versioning
                }
            }

            return new Func<Object, EncodeBuffer>(value =>
            {
                // Handle nulls
                if (null == value)
                {
                    return Null;
                }

                var output = new EncodeBuffer();

                for (Byte i = 0; i <= fieldCount; i++)
                {
                    var field = fields[i];
                    var encoder = coders[i];

                    // Get the serializer for the sub-item
                    var subType = recurse(field.FieldType);

                    // Get it's method info
                    var subMethodInfo = subType.GetMethodInfo();

                    // Extract value
                    var v = field.GetValue(value);

                    // Add to output
                    output.Append((EncodeBuffer)encoder.DynamicInvokeTransparent(v));
                }

                // Encode length
                output.SetFirst(UnsignedVlq.Encode((UInt64)output.TotalLength + 1)); // Number of bytes

                return output;
            });
        }

        public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse)
        {
            // Find all properties decorated with LightWeightProperty attribute
            var fields = new FieldInfo[Byte.MaxValue]; // Index => Field
            var coders = new Delegate[Byte.MaxValue]; // Index => Encoder/Decoder

            var maxIndex = -1;
            foreach (var property in type.GetRuntimeFields())
            {
                // Get property attribute which tells us the properties' index
                var attribute = (LightWeightPropertyAttribute)property.GetCustomAttribute(typeof(LightWeightPropertyAttribute), false);
                if (null == attribute)
                {
                    // No attribute found, skip
                    continue;
                }

                // Check for duplicate index
                if (null != fields[attribute.Index])
                {
                    throw new DuplicateIndexException($"The index {fields[attribute.Index]} is already used and cannot be reused.");
                }

                // Note the max index used
                if (attribute.Index > maxIndex)
                {
                    maxIndex = attribute.Index;
                }

                // Find/create encoder
                var decoder = recurse(property.FieldType);

                // Store property in lookup
                fields[attribute.Index] = property;
                coders[attribute.Index] = decoder;
            }

            // Check that no indexes have been missed
            for (var i = 0; i < maxIndex; i++)
            {
                if (null == fields[i])
                {
                    throw new MissingIndexException($"Indexes must not be skipped, however missing index {i}.");
                }
            }

            return new Func<DecodeBuffer, Object>(input =>
            {
                // Read the length header
                var header = (Int32)UnsignedVlq.Decode(input);

                // Handle nulls
                if (header == 0)
                {
                    return null;
                }

                // Determine length
                var length = header - 1;

                // Instantiate output
                var output = Activator.CreateInstance(type);

                // Extract an inner buffer so that if fields are added to the class in the future we ignore them, being backwards compatible
                var innerInput = input.Extract(length);

                // Isolate bytes for body
                for (var i = 0; i <= maxIndex; i++)
                {
                    var field = fields[i];

                    // Deserialize value
                    var value = coders[i].DynamicInvokeTransparent(innerInput);

                    // Set it on property
                    field.SetValue(output, value);
                }

                return output;
            });
        }
    }
}