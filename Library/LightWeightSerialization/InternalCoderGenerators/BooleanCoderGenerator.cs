using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class BooleanCoderGenerator : ICoderGenerator {
		private const Byte False = 0x00;
		private const Byte True = 0x01;
		private static readonly EncodeBuffer FalseBuffer = new EncodeBuffer(new ArraySegment<Byte>(new[] {False}));
		private static readonly EncodeBuffer TrueBuffer = new EncodeBuffer(new ArraySegment<Byte>(new[] {True}));

		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(Boolean);
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Boolean, EncodeBuffer>(value => {
				if (value) {
					return TrueBuffer;
				}

				return FalseBuffer;
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<DecodeBuffer, Boolean>(stream => {
				var v = stream.ReadByte();
				switch (v) {
					case True: return true;
					case False: return false;
					default: throw new SchemaMismatchException($"Boolean value must be either {True} or {False}, but {v} given.");
				}
			});
		}
	}
}