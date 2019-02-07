using System;
using System.Collections;
using System.IO;
using System.Reflection;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class IDictionaryCoderGenerator : ICoderGenerator {
		private static readonly Node Null = new Node(Vlq.Encode(0));

		public Boolean IsCompatibleWith<T>() {
			return typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T));
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			// Get serializer for sub items
			var keyEncoder = recurse(type.GenericTypeArguments[0]);
			var valueEncoder = recurse(type.GenericTypeArguments[1]);

			return new Func<IDictionary, Node>(value => {
				// Handle nulls
				if (null == value) {
					return Null;
				}

				// Serialize elements   
				var output = new Node();
				var e = value.GetEnumerator();
				UInt64 count = 0;
				while (e.MoveNext()) {
					output.Append((Node) keyEncoder.DynamicInvoke(e.Key));
					output.Append((Node) valueEncoder.DynamicInvoke(e.Value));
					count++;
				}

				// Encode length
				output.Prepend(Vlq.Encode(count + 1));

				return output;
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			// Get deserializer for sub items
			var keyDecoder = recurse(type.GenericTypeArguments[0]);
			var valueDecoder = recurse(type.GenericTypeArguments[1]);

			return new Func<Stream, IDictionary>((input) => {
				// Read header
				var header = Vlq.Decode(input);

				if (header == 0) {
					return null;
				}

				// Get count
				var count = (Int32) header - 1;

				// Instantiate dictionary
				var output = (IDictionary) Activator.CreateInstance(type);

				// Loop through input buffer until depleted
				for (var i = 0; i < count; i++) {
					// Deserialize key
					var keyValue = keyDecoder.DynamicInvoke(input);

					// Deserialize value
					var valueValue = valueDecoder.DynamicInvoke(input);

					// Add to output
					output[keyValue] = valueValue;
				}

				return output;
			});
		}
	}
}