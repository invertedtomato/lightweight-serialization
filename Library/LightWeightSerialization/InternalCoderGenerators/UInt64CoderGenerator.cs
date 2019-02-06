using System;
using System.IO;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class UInt64CoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(UInt64);
		}
		
		public Delegate GenerateEncoder(Type type, Func<Type,Delegate> recurse){
			return new Func<UInt64, Node>(value => {
				return new Node(Vlq.Encode(value));
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type,Delegate> recurse){
			return new Func<Stream, UInt64>((input) => {
				return (UInt64)Vlq.Decode(input);
			});
		}
	}
}