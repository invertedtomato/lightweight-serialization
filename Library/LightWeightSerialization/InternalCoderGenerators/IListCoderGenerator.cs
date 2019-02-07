using System;
using System.Collections;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class IListCoderGenerator : ICoderGenerator {
		private static readonly Node Null = new Node(UnsignedVlq.Encode(0));

		public Boolean IsCompatibleWith<T>() {
			// This explicitly does not support arrays (otherwise they could get matched with the below check)
			if (typeof(T).IsArray) {
				return false;
			}

			return typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T));
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			// Get serializer for sub items
			var valueEncoder = recurse(type.GenericTypeArguments[0]);

			return new Func<IList, Node>(value => {
				// Handle nulls
				if (null == value) {
					return Null;
				}

				// Serialize elements
				var output = new Node();
				foreach (var element in value) {
					output.Append((Node) valueEncoder.DynamicInvoke(element));
				}

				// Encode length
				output.Prepend(UnsignedVlq.Encode((UInt64) value.Count + 1));

				return output;
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			// Get deserializer for sub items
			var valueDecoder = recurse(type.GenericTypeArguments[0]);

			return new Func<Stream, IList>(input => {
				// Read header
				var header = UnsignedVlq.Decode(input);

				// Handle nulls
				if (header == 0) {
					return null;
				}

				// Determine length
				var ount = (Int32) header - 1;

				// Instantiate list
				var output = (IList) Activator.CreateInstance(type); //typeof(List<>).MakeGenericType(type.GenericTypeArguments)

				// Deserialize until we reach length limit
				for (var i = 0; i < ount; i++) {
					// Deserialize element
					var element = valueDecoder.DynamicInvoke(input);

					// Add to output
					output.Add(element);
				}

				return output;
			});
		}
	}
}