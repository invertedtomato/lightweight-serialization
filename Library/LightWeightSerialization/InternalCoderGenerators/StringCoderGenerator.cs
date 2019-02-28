using System;
using System.IO;
using System.Text;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class StringCoderGenerator : ICoderGenerator {
		private static readonly Node Null = new Node(UnsignedVlq.Encode(0));

		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(String);
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<String, Node>(value => {
				// Handle nulls
				if (null == value) {
					return Null;
				}

				// Encode value
				var output = new Node(2);
				output.Append(new ArraySegment<Byte>(  Encoding.UTF8.GetBytes(value)));

				// Prepend length
				output.SetFirst(UnsignedVlq.Encode((UInt64) output.TotalLength + 1)); // Offset by one, since 0 is NULL

				return output;
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, String>(input => {
				// Decode header
				var header = UnsignedVlq.Decode(input);

				// Handle nulls
				if (header == 0) {
					return null;
				}

				// Determine length
				var length = (Int32) header - 1;

				// Decode value
				return Encoding.UTF8.GetString(input.Read(length), 0, length);
			});
		}
	}
}