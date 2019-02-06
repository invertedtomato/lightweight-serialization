using System;
using System.Collections;
using System.IO;
using System.Reflection;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class ClassCoderGenerator : ICoderGenerator {
		private static readonly Node EmptyNode = new Node(Vlq.Encode(0));

		public Boolean IsCompatibleWith<T>() {
			// This explicitly does not support lists or dictionaries
			if (typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T)) ||
			    typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T))) {
				// TODO: Instead only accept classes with a specific attribute
				return false;
			}

			return typeof(T).GetTypeInfo().IsClass;
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			// Find all properties decorated with LightWeightProperty attribute
			var fields = new FieldInfo[Byte.MaxValue]; // Index => Field
			var coders = new Delegate[Byte.MaxValue]; // Index => Encoder/Decoder

			var fieldCount = -1;
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
				if (attribute.Index > fieldCount) {
					fieldCount = attribute.Index;
				}

				// Find/create encoder
				var encoder = recurse(property.FieldType);

				// Store property in lookup
				fields[attribute.Index] = property;
				coders[attribute.Index] = encoder;
			}

			// If no properties, shortcut the whole thing and return a blank
			if (fieldCount == -1) {
				return new Func<Object, Node>(value => { return EmptyNode; });
			}

			// Check that no indexes have been missed
			for (var i = 0; i < fieldCount; i++) {
				if (null == fields[i]) {
					throw new MissingIndexException($"Indexes must not be skipped, however missing index {i}.");
				}
			}

			return new Func<Object, Node>(value => {
				// Handle nulls
				if (null == value) {
					return EmptyNode; // TODO: Not the correct way to handle nulls. Indistinguishable from empty
				}

				var output = new Node();

				for (Byte i = 0; i <= fieldCount; i++) {
					var field = fields[i];
					var encoder = coders[i];

					// Get the serializer for the sub-item
					var subType = recurse(field.FieldType);

					// Get it's method info
					var subMethodInfo = subType.GetMethodInfo();

					// Extract value
					var v = field.GetValue(value);

					// Add to output
					output.Append((Node) encoder.DynamicInvoke(v));
				}
				
				// Encode length
				output.Prepend(Vlq.Encode((UInt64) output.TotalLength)); // Number of bytes
				
				return output;
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			// Find all properties decorated with LightWeightProperty attribute
			var fields = new FieldInfo[Byte.MaxValue]; // Index => Field
			var coders = new Delegate[Byte.MaxValue]; // Index => Encoder/Decoder

			var fieldCount = -1;
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
				if (attribute.Index > fieldCount) {
					fieldCount = attribute.Index;
				}

				// Find/create encoder
				var decoder = recurse(property.FieldType);

				// Store property in lookup
				fields[attribute.Index] = property;
				coders[attribute.Index] = decoder;
			}

			// If no properties, shortcut the whole thing and return a blank
			if (fieldCount == -1) {
				return new Func<Object, Node>(value => { return EmptyNode; });
			}

			// Check that no indexes have been missed
			for (var i = 0; i < fieldCount; i++) {
				if (null == fields[i]) {
					throw new MissingIndexException($"Indexes must not be skipped, however missing index {i}.");
				}
			}

			return new Func<Stream, Object>((input) => {
				// Instantiate output
				var output = Activator.CreateInstance(type);

				// Read the length header
				var length = (Int32)Vlq.Decode(input);

				// Isolate bytes for body
				using (var innerInput = new MemoryStream(input.Read(length))) {  // TODO: Inefficient because it copies a potentially large chunk of memory. Better approach?
					for (var i = 0; i < fieldCount; i++) {
						var field = fields[i];

						// Deserialize value
						var value = coders[i].DynamicInvoke(innerInput);

						// Set it on property
						field.SetValue(output, value);
					}
				}

				return output;
			});
		}
	}
}