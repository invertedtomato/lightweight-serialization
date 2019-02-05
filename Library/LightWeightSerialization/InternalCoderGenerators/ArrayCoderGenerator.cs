using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class ArrayCoderGenerator : ICoderGenerator {
		private readonly VLQCodec VLQ = new VLQCodec();

		public Boolean IsCompatibleWith<T>() {
			return typeof(T).IsArray;
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			// Get serializer for sub items
			var valueEncoder = recurse(type.GetElementType());

			return new Func<Array, Node>(value => {
				// Handle nulls
				if (null == value) {
					return LightWeight.EmptyNode;
				}

				// Serialize elements
				var childNodes = new NodeSet(value.Length);
				foreach (var subValue in value) {
					childNodes.Add((Node) valueEncoder.DynamicInvoke(subValue));
				}

				// Encode length
				var encodedLength = VLQ.CompressUnsigned((UInt64) childNodes.TotalLength).ToArray();

				return Node.NonLeaf(encodedLength, childNodes);
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			// Get deserializer for sub items
			var valueDecoder = recurse(type.GetElementType());

			return new Func<Stream, Int32, Array>((buffer, count) => {
				// Instantiate list
				var container = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(type.GetElementType()));

				// Deserialize until we reach length limit
				while (count > 0) {
					// Extract length
					count -= VLQ.DecompressUnsigned(buffer, out var length);

					// Deserialize element
					var element = valueDecoder.DynamicInvoke(buffer, (Int32) length);
					count -= (Int32) length;

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