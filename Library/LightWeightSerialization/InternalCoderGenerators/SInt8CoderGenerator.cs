using System;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class SInt8CoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			var type = typeof(T);
			var typeInfo = type.GetTypeInfo();
			return type == typeof(SByte) || // Standard value
			       (typeInfo.IsEnum && typeInfo.GetEnumUnderlyingType() == typeof(SByte)); // Enum value
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<SByte, Node>(value => { return new Node(new ArraySegment<Byte>(new[] {(Byte) value})); });
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, SByte>(input => {
				var v = input.ReadByte();

				return (SByte) v;
			});
		}
	}
}