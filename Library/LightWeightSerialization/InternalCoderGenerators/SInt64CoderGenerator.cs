using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class SInt64CoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(Int64);
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Int64, Node>(value => { return new Node(SignedVlq.Encode(value)); });
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, Int64>(input => { return SignedVlq.Decode(input); });
		}
	}
}