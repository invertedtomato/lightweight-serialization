using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public interface ICoderGenerator {
		Boolean IsCompatibleWith<T>();
		Delegate GenerateEncoder(Type type, Func<Type,Delegate> recurse); // Func<?, Node>
		Delegate GenerateDecoder(Type type, Func<Type,Delegate> recurse); // Func<Stream, Int32, ?>
	}
}