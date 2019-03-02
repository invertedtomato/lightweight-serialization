using System;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class SingleCoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(Single);
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Single, EncodeBuffer>(value => {
				return new EncodeBuffer(new ArraySegment<Byte>(BitConverter.GetBytes(value)));
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<DecodeBuffer, Single>(input => {
				return BitConverter.ToSingle(input.Underlying, input.GetIncrementOffset(4));
			});
		}
	}
}