using System;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class DecodeBuffer {
		public readonly Byte[] Underlying;
		public Int32 Offset;
		public Int32 Count;

		public DecodeBuffer(Byte[] underlying, Int32 offset, Int32 count) {
			Underlying = underlying;
			Offset = offset;
			Count = count;
		}

		public DecodeBuffer(ArraySegment<Byte> underlying) {
			Underlying = underlying.Array;
			Offset = underlying.Offset;
			Count = underlying.Count;
		}

		public Int32 GetIncrementOffset(Int32 count) {
			if (count > Count) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}

			var offset = Offset;
			Offset += count;
			Count -= count;
			return offset;
		}

		public Byte ReadByte() {
			return Underlying[GetIncrementOffset(1)];
		}

		public DecodeBuffer Extract(Int32 count) {
			return new DecodeBuffer(Underlying, GetIncrementOffset(count), count);
		}
	}
}