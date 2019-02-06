using System;
using System.IO;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class UInt16CoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(UInt16);
		}

		public Delegate GenerateEncoder(Type type, Func<Type,Delegate> recurse){
			return new Func<UInt16, Node>(value => {
				return new Node(Vlq.Encode(value));
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type,Delegate> recurse){
			return new Func<Stream, UInt16>((input) => {
				return (UInt16)Vlq.Decode(input);
			});
		}
	}
}