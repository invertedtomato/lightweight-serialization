using System;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class UInt64CoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			var type = typeof(T);
			var typeInfo = type.GetTypeInfo();
			return type == typeof(UInt64) || // Standard value
			       (typeInfo.IsEnum && typeInfo.GetEnumUnderlyingType() == typeof(UInt64)); // Enum value
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<UInt64, Node>(value => { return new Node(UnsignedVlq.Encode(value)); });
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, UInt64>(input => { return UnsignedVlq.Decode(input); });
		}
	}
}