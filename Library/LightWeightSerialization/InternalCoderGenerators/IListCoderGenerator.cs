using System;
using System.Collections;
using System.IO;
using System.Reflection;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class IListCoderGenerator  : ICoderGenerator{
		private readonly VLQCodec VLQ = new VLQCodec();

		public Boolean IsCompatibleWith<T>() {
			// This explicitly does not support arrays
			if (typeof(T).IsArray) {
				return false;
			}

			return typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T));
		}

		public Delegate GenerateEncoder(Type type, Func<Type,Delegate> recurse) {
			// Get serializer for sub items
			var valueEncoder = recurse(type.GenericTypeArguments[0]);

			return new Func<IList, Node>(value => {
				// Handle nulls
				if (null == value) {
					return LightWeight.EmptyNode;
				}

				// Serialize elements
				var childNodes = new NodeSet(value.Count);
				foreach (var element in value) {
					childNodes.Add((Node) valueEncoder.DynamicInvoke(element));
				}

				// Encode length
				var encodedLength = VLQ.CompressUnsigned((UInt64)childNodes.TotalLength).ToArray();

				return Node.NonLeaf(encodedLength, childNodes);
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type,Delegate> recurse) {
			// Get deserializer for sub items
			var valueDecoder = recurse(type.GenericTypeArguments[0]);

			return new Func<Stream, Int32, IList>((buffer, count) => {
				// Instantiate list
				var output = (IList) Activator.CreateInstance(type); //typeof(List<>).MakeGenericType(type.GenericTypeArguments)

				// Deserialize until we reach length limit
				while (count > 0) {
					// Extract length
					count -= VLQ.DecompressUnsigned(buffer, out var length);

					// Deserialize element
					var element = valueDecoder.DynamicInvoke(buffer, (Int32) length);
					count -= (Int32)length;

					// Add to output
					output.Add(element);
				}

				return output; // (T)
			});
		}
	}
}