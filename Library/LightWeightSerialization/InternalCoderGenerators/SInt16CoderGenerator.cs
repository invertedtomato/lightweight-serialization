using System;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class SInt16CoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			var type = typeof(T);
			var typeInfo = type.GetTypeInfo();
			return type == typeof(Int16) || // Standard value
			       (typeInfo.IsEnum && typeInfo.GetEnumUnderlyingType() == typeof(Int16)); // Enum value
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Int16, Node>(value => { return new Node(SignedVlq.Encode(value)); });
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, Int16>(input => { return (Int16) SignedVlq.Decode(input); });
		}
	}
}