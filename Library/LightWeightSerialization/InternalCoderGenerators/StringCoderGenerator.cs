using System;
using System.IO;
using System.Text;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class StringCoderGenerator  : ICoderGenerator{
		private readonly VLQCodec VLQ = new VLQCodec();

		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(String);
		}

		public Delegate GenerateEncoder(Type type, Func<Type,Delegate> recurse){
			return new Func<String, Node>(value => {
				if (null == value) {
					return LightWeight.EmptyNode;
				}

				var encodedValue = Encoding.UTF8.GetBytes(value);
				var encodedLength = VLQ.CompressUnsigned((UInt64)encodedValue.Length).ToArray();
				return Node.Leaf(encodedLength, encodedValue);
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type,Delegate> recurse) {
			return new Func<Stream, Int32, String>((buffer, count) => {
				return Encoding.UTF8.GetString(buffer.Read(count), 0, count);
			});
		}
	}
}