using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class ArrayCoderGenerator : ICoderGenerator {
		private static readonly Node Null = new Node(Vlq.Encode(0));

		public Boolean IsCompatibleWith<T>() {
			return typeof(T).IsArray;
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			// Get serializer for sub items
			var valueEncoder = recurse(type.GetElementType());

			return new Func<Array, Node>(value => {
				// Handle nulls
				if (null == value) {
					return Null;
				}

				// Serialize elements
				var output = new Node();
				foreach (var subValue in value) {
					output.Append((Node) valueEncoder.DynamicInvoke(subValue));
				}

				// Encode length
				output.Prepend(Vlq.Encode((UInt64) value.Length)); // Number of elements, not number of bytes

				return output;
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			// Get deserializer for sub items
			var valueDecoder = recurse(type.GetElementType());

			return new Func<Stream, Array>((input) => {
				var header = Vlq.Decode(input);

				if (header == 0) {
					return null;
				}

				// Determine length
				var length = (Int32) header - 1;

				// Instantiate list
				var container = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GetElementType()));

				// Deserialize until we reach length limit
				for (var i = 0; i < length; i++) {
					// Deserialize element
					var element = valueDecoder.DynamicInvoke(input);

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