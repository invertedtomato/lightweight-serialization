using System;
using System.IO;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class UInt16CoderGenerator : ICoderGenerator {
		private readonly VLQCodec VLQ = new VLQCodec();

		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(UInt16);
		}

		public Delegate GenerateEncoder(Type type, Func<Type,Delegate> recurse){
			var smaller = recurse(typeof(Byte));
			
			return new Func<UInt16, Node>(value => {
				if (value == 0) {
					return LightWeight.EmptyNode;
				} else if (value <= Byte.MaxValue) {
					return Node.Leaf(VLQCodec.One, new Byte[] {(Byte) value});
				} else {
					return Node.Leaf(VLQCodec.Two, BitConverter.GetBytes(value));
				}
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type,Delegate> recurse){
			return new Func<Stream, Int32, UInt16>((buffer, count) => {
				if (count == 0) {
					return 0;
				}else if (count == 1) {
					return (UInt16) buffer.ReadByte();
				} else if (count == 2) {
					return BitConverter.ToUInt16(buffer.Read(2), 0);
				} else {
					throw new DataFormatException($"UInt16 values can be 0, 1 or 2 bytes, but {count} found.");
				}
			});
		}
	}
}