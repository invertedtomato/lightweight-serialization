using System;
using System.IO;
using System.Text;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class StringCoderGenerator  : ICoderGenerator{
		private static readonly Node Null = new Node(Vlq.Encode(0));
		
		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(String);
		}

		public Delegate GenerateEncoder(Type type, Func<Type,Delegate> recurse){
			return new Func<String, Node>(value => {
				// Handle nulls
				if (null == value) {
					return Null;
				}

				// Encode value
				var node = new Node(Encoding.UTF8.GetBytes(value));
				
				// Prepend length
				node.Prepend(Vlq.Encode((UInt64)node.TotalLength+1)); // Offset by one, since 0 is NULL

				return node;
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type,Delegate> recurse) {
			return new Func<Stream, String>((input) => {
				// Decode header
				var header = Vlq.Decode(input);
				
				// Handle nulls
				if (header == 0) {
					return null;
				}

				// Determine length
				var length = (Int32)header - 1;
				
				// Decode value
				return Encoding.UTF8.GetString(input.Read(length), 0, length);
			});
		}
	}
}