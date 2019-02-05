using System;
using System.Collections;
using System.IO;
using System.Reflection;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class IDictionaryCoderGenerator  : ICoderGenerator{
		private readonly VLQCodec VLQ = new VLQCodec();

		public Boolean IsCompatibleWith<T>() {
			return typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T));
		}

		public Delegate GenerateEncoder(Type type, Func<Type,Delegate> recurse) {
			// Get serializer for sub items
			var keyEncoder = recurse(type.GenericTypeArguments[0]);
			var valueEncoder = recurse(type.GenericTypeArguments[1]);

			return new Func<IDictionary, Node>(value => {
				// Handle nulls
				if (null == value) {
					return LightWeight.EmptyNode;
				}

				// Serialize elements   
				var childNodes = new NodeSet(value.Count * 2);
				var e = value.GetEnumerator();
				while (e.MoveNext()) {
					childNodes.Add((Node) keyEncoder.DynamicInvoke(e.Key));
					childNodes.Add((Node) valueEncoder.DynamicInvoke(e.Value));
				}

				// Encode length
				var encodedLength = VLQ.CompressUnsigned((UInt64)childNodes.TotalLength).ToArray();

				return Node.NonLeaf(encodedLength, childNodes);
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type,Delegate> recurse) {
			// Get deserializer for sub items
			var keyDecoder = recurse(type.GenericTypeArguments[0]);
			var valueDecoder = recurse(type.GenericTypeArguments[1]);

			return new Func<Stream, Int32, IDictionary>((buffer, count) => {
				// Instantiate dictionary
				var output = (IDictionary) Activator.CreateInstance(type);

				// Loop through input buffer until depleted
				while (count > 0) {
					// Deserialize key
					count -= VLQ.DecompressUnsigned(buffer, out var keyLength);
					var keyValue = keyDecoder.DynamicInvoke(buffer, (Int32)keyLength);
					count -= (Int32)keyLength;

					// Deserialize value
					count -= VLQ.DecompressUnsigned(buffer, out var valueLength);
					var valueValue = valueDecoder.DynamicInvoke(buffer, (Int32)valueLength);
					count -= (Int32)valueLength;

					// Add to output
					output[keyValue] = valueValue;
				}

				return output; // (T)
			});
		}
	}
}