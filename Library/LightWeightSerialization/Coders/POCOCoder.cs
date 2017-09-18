using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Collections;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.Coders {
    public class POCOCoder {
        private readonly VLQCodec VLQ = new VLQCodec();
        private readonly byte[] MSB = new byte[] { 0x80 };
        private static readonly Type Attribute = typeof(LightWeightPropertyAttribute);

        public Type Target { get { return null; } }

        public void Serialize(object value, Buffer<byte> buffer) {
            var values = Class1.GetAllFields(value);
            foreach (var value in values) {
                // If index was missed...
                if (null == value) {
                    // Append stub byte
                    _EnqueueBuffer(MSB);
                } else {
                    // Serialize value
                    var v = Serialize(value);
                    _EnqueueLength(v.Length);
                    _EnqueueBuffer(v);
                }
            }

            /*
            // Iterate properties
            var properties = PropertyCache.Get(value.GetType());
            foreach (var property in properties) {
                // If index was missed...
                if (null == property) {
                    // Append stub byte
                    _EnqueueBuffer(MSB);
                } else {
                    // Serialize value
                    var v = Serialize(property.GetValue(value, null));
                    _EnqueueLength(v.Length);
                    _EnqueueBuffer(v);
                }
            }*/
        }

        public object Deserialize(Buffer<byte> buffer) {
            // Instantiate output
            var output = Activator.CreateInstance(type);

            // Prepare for object deserialization
            var index = -1;

            // Attempt to read field length, if we've reached the end of the payload, abort
            while (payload.IsReadable) {
                // Get the length in a usable format
                var l = (int)Codec.DecompressUnsigned(payload);
                var b = payload.DequeueBuffer(l);

                // Increment the index
                index++;

                // Iterate through each property looking for one that matches index
                foreach (var property in type.GetRuntimeProperties()) {
                    // Get property attribute which tells us the properties' index
                    var attribute = (LightWeightPropertyAttribute)property.GetCustomAttribute(PropertyAttribute);

                    // Skip if not found, or index doesn't match
                    if (null == attribute || attribute.Index != index) {
                        // No attribute found, skip
                        continue;
                    }

                    // Deserialize value
                    var v = Deserialize(b, property.PropertyType);

                    // Set it on property
                    property.SetValue(output, v);
                }
            }

            return output;
        }

    }
}
