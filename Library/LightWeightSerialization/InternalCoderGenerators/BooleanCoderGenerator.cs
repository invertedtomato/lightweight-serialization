using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class BooleanCoderGenerator : ICoderGenerator {
		private const Byte False = 0x00;
		private const Byte True = 0x01;
		private static readonly Node FalseNode = new Node(new ArraySegment<Byte>(new[] {False}));
		private static readonly Node TrueNode = new Node(new ArraySegment<Byte>(new[] {True}));

		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(Boolean);
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Boolean, Node>(value => {
				if (value) {
					return TrueNode;
				}

				return FalseNode;
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, Boolean>(stream => {
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