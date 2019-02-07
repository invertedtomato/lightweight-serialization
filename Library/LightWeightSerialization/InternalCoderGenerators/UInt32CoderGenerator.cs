using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class UInt32CoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(UInt32);
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<UInt32, Node>(value => { return new Node(UnsignedVlq.Encode(value)); });
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, UInt32>(input => { return (UInt32) UnsignedVlq.Decode(input); });
		}
	}
}