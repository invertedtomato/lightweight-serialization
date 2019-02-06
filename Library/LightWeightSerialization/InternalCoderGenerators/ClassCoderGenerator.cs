using System;
using System.Collections;
using System.IO;
using System.Reflection;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class ClassCoderGenerator : ICoderGenerator {
		private readonly VLQCodec VLQ = new VLQCodec();

		public Boolean IsCompatibleWith<T>() {
			// This explicitly does not support lists or dictionaries
			if (typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T)) ||
			    typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T))) {
				// TODO: Find a better way to match POCOS
				return false;
			}

			return typeof(T).GetTypeInfo().IsClass;
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			// Find all properties decorated with LightWeightProperty attribute

			var fields = new FieldInfo[Byte.MaxValue]; // Index => Field
			var encoders = new Delegate[Byte.MaxValue]; // Index => Encoder
			var maxIndex = -1;
			foreach (var property in type.GetRuntimeFields()) {
				// Get property attribute which tells us the properties' index
				var attribute = (LightWeightPropertyAttribute) property.GetCustomAttribute(typeof(LightWeightPropertyAttribute), false);
				if (null == attribute) {
					// No attribute found, skip
					continue;
				}

				// Check for duplicate index
				if (null != fields[attribute.Index]) {
					throw new DuplicateIndexException($"The index {fields[attribute.Index]} is already used and cannot be reused.");
				}

				// Note the max index used
				if (attribute.Index > maxIndex) {
					maxIndex = attribute.Index;
				}

				// Find/create encoder
				var encoder = recurse(property.FieldType);

				// Store property in lookup
				fields[attribute.Index] = property;
				encoders[attribute.Index] = encoder;
			}

			// If no properties, shortcut the whole thing and return a blank
			if (maxIndex == -1) {
				return new Func<Object, Node>(value => { return LightWeight.EmptyNode; });
			}

			return new Func<Object, Node>(value => {
				// Handle nulls
				if (null == value) {
					return LightWeight.EmptyNode;
				}

				var childNodes = new NodeSet(maxIndex + 1);

				for (Byte i = 0; i <= maxIndex; i++) {
					var field = fields[i];
					var encoder = encoders[i];

					if (null == field) {
						childNodes.Add(LightWeight.EmptyNode);
					} else {
						// Get the serializer for the sub-item
						var subType = recurse(field.FieldType);

						// Get it's method info
						var subMethodInfo = subType.GetMethodInfo();

						// Extract value
						var v = field.GetValue(value);

						// Add to output
						childNodes.Add((Node) encoder.DynamicInvoke(v));
					}
				}

				// Encode length
				var encodedLength = VLQ.CompressUnsigned((UInt64) childNodes.TotalLength).ToArray();

				return Node.NonLeaf(encodedLength, childNodes);
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			// Build vector of property types
			var fields = new FieldInfo[Byte.MaxValue];
			var decoders = new Delegate [Byte.MaxValue];
			foreach (var field in type.GetRuntimeFields()) {
				// TODO: Add property support
				// Get property attribute which tells us the properties' index
				var attribute = (LightWeightPropertyAttribute) field.GetCustomAttribute(typeof(LightWeightPropertyAttribute));

				// Skip if not found, or index doesn't match
				if (null == attribute) {
					// No attribute found, skip
					continue;
				}

				// Check for duplicate index
				if (null != fields[attribute.Index]) {
					throw new DuplicateIndexException($"The index {fields[attribute.Index]} is already used and cannot be reused.");
				}

				// Find/generate decoder
				var decoder = recurse(field.FieldType);

				// Store values in lookup
				fields[attribute.Index] = field;
				decoders[attribute.Index] = decoder;
			}

			return new Func<Stream, Int32, Object>((buffer, count) => {
				// Instantiate output
				var output = Activator.CreateInstance(type);

				// Prepare for object deserialization
				var index = -1;

				// Attempt to read field length, if we've reached the end of the payload, abort
				while (count > 0) {
					// Get the length in a usable format
					count -= VLQ.DecompressUnsigned(buffer, out var length);

					// Increment the index
					index++;

					// Get field, and skip if it doesn't exist (it'll be ignored)
					var field = fields[index];
					if (null == field) {
						continue;
					}

					// Deserialize value
					var value = decoders[index].DynamicInvoke(buffer, (Int32) length);
					count -= (Int32) length;

					// Set it on property
					field.SetValue(output, value);
				}

				return output; // (T)
			});
		}
	}
}