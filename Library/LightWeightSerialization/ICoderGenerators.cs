using System;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public interface ICoderGenerator {
		Boolean IsCompatibleWith<T>();
		Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse); // Func<?, Node output>
		Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse); // Func<Stream input, ?>
	}
}