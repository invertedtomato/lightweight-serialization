using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class SInt16CoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(Int16);
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Int16, Node>(value => { return new Node(SignedVlq.Encode(value)); });
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, Int16>(input => { return (Int16) SignedVlq.Decode(input); });
		}
	}
}