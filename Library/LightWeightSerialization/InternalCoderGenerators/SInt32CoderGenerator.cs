using System;
using System.IO;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class SInt32CoderGenerator  : ICoderGenerator{
		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(Int32);
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Int32, Node>(value => {
				return new Node(Vlq.Encode(ZigZag.Encode(value)));
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, Int32>((input) => {
				return (Int32) ZigZag.Decode(Vlq.Decode(input));
			});
		}
	}
}