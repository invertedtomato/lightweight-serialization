using System;
using System.IO;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class BooleanCoderGenerator : ICoderGenerator {
		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(Boolean);
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Boolean, Node>((value) => {
				if (value) {
					return Node.Leaf(VLQCodec.One, new Byte[] {0x00});
				} else {
					return LightWeight.EmptyNode;
				}
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, Int32, Boolean>((stream, count) => {
				if (count == 0) {
					return false;
				}
#if DEBUG
				if (count != 1) {
					throw new DataFormatException($"Boolean values can be no more than 1 byte long, but {count} found..");
				}
#endif
				if (stream.ReadByte() != 0x00) {
					throw new DataFormatException("Boolean values cannot be anything other than 0x00.");
				}

				return true;
			});
		}
	}
}