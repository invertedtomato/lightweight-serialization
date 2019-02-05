using System;
using System.IO;
using InvertedTomato.Compression.Integers;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class UInt8CoderGenerator  : ICoderGenerator{
		private readonly VLQCodec VLQ = new VLQCodec();

		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(Byte);
		}

		public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Byte, Node>(value => {
				if (value == 0) {
					return LightWeight.EmptyNode;
				} else  {
					return Node.Leaf(VLQCodec.One, BitConverter.GetBytes(value));
				}
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse) {
			return new Func<Stream, Int32, Byte>((buffer, count) => {
				if (count == 0) {
					return 0;
				}else if (count == 1) {
					return (Byte) buffer.ReadByte();
				} else {
					throw new DataFormatException($"UInt8 values can be 0 or 1 bytes, but {count} found.");
				}
			});
		}
	}
}