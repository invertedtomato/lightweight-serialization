using System;
using System.IO;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;

namespace InvertedTomato.Serialization.LightWeightSerialization.InternalCoders {
	public class SInt64CoderGenerator  : ICoderGenerator{
		private readonly VLQCodec VLQ = new VLQCodec();

		public Boolean IsCompatibleWith<T>() {
			return typeof(T) == typeof(Int64);
		}

		public Delegate GenerateEncoder(Type type, Func<Type,Delegate> recurse){
			return new Func<Int64, Node>(value => {
				if (value == 0) {
					return LightWeight.EmptyNode;
				} else if (value >= SByte.MinValue && value <= Byte.MaxValue) {
					return Node.Leaf(VLQCodec.One, new Byte[] {(Byte) value});
				} else if (value >= Int16.MinValue && value <= UInt16.MaxValue) {
					return Node.Leaf(VLQCodec.Two, BitConverter.GetBytes((Int16) value));
				} else if (value >= Int32.MinValue && value <= UInt32.MaxValue) {
					return Node.Leaf(VLQCodec.Four, BitConverter.GetBytes((Int32) value));
				} else {
					return Node.Leaf(VLQCodec.Eight, BitConverter.GetBytes(value));
				}
			});
		}

		public Delegate GenerateDecoder(Type type, Func<Type,Delegate> recurse){
			return new Func<Stream, Int32, Int64>((buffer, count) => {
				if (count == 0) {
					return 0;
				}else if (count == 1) {
					return (SByte) buffer.ReadByte();
				} else if (count == 2) {
					return BitConverter.ToInt16(buffer.Read(2), 0);
				} else if (count == 4) {
					return BitConverter.ToInt32(buffer.Read(4), 0);
				} else if (count == 8) {
					return BitConverter.ToInt64(buffer.Read(8), 0);
				} else {
					throw new DataFormatException($"Int64 values can be 0, 1, 2, 4 or 8 bytes, but {count} found.");
				}
			});
		}
	}
}