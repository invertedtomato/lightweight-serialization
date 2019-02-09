using System;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class UInt32CoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			var type = typeof(T);
			var typeInfo = type.GetTypeInfo();
			return type == typeof(UInt32) || // Standard value
			       (typeInfo.IsEnum && typeInfo.GetEnumUnderlyingType() == typeof(UInt32)); // Enum value
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<UInt32, Node>(value => { return new Node(UnsignedVlq.Encode(value)); });
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, UInt32>(input => { return (UInt32) UnsignedVlq.Decode(input); });
		}
	}
}