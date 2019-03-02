using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public static class UnsignedVlq {
		public const UInt64 MinValue = 0;
		public const UInt64 MaxValue = UInt64.MaxValue - 1;

		private const Byte More = 0b10000000;
		private const Byte Mask = 0b01111111;
		private const Int32 PacketSize = 7;
		private const UInt64 MinPacketValue = UInt64.MaxValue >> (64 - PacketSize);
		private const Int32 MaxBytes = 10;

		private static readonly ArraySegment<Byte>[] EncodeCache = new ArraySegment<Byte>[255];

		public static ArraySegment<Byte> Encode(UInt64 value) {
#if DEBUG
			if (value > MaxValue) {
				throw new OverflowException("Symbol is larger than maximum supported value. See UnsignedVlq.MaxValue.");
			}
#endif

			// Lookup cache and return if found
			if (value < (UInt64) EncodeCache.Length && EncodeCache[value].Count > 0) {
				return EncodeCache[value];
			}

			var position = 0;
			var buffer = new Byte[MaxBytes];
			var symbol = value;

			// Iterate through input, taking X bits of data each time, aborting when less than X bits left
			while (symbol > MinPacketValue) {
				// Write payload, skipping MSB bit
				buffer[position++] = (Byte) ((symbol & Mask) | More);

				// Offset value for next cycle
				symbol >>= PacketSize;
				symbol--;
			}

			// Write remaining - marking it as the final byte for symbol
			buffer[position++] = (Byte) (symbol & Mask);


			// Populate cache
			if (value < (UInt64) EncodeCache.Length) {
				return EncodeCache[value] = new ArraySegment<Byte>(buffer, 0, position);
			} else {
				return new ArraySegment<Byte>(buffer, 0, position);
			}
		}

		public static UInt64 Decode(Byte[] input) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}
#endif
			return Decode(new DecodeBuffer(input, 0, input.Length));
		}

		public static UInt64 Decode(DecodeBuffer input) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}
#endif


			// Setup symbol
			UInt64 symbol = 0;
			var bit = 0;

			Int32 b;
			do {
				// Read byte
				b = input.ReadByte();

				// Add input bits to output
				var chunk = (UInt64) (b & Mask);
				var pre = symbol;
				symbol += (chunk + 1) << bit;

#if DEBUG
				// Check for overflow
				if (symbol < pre) {
					throw new OverflowException("Symbol is larger than maximum supported value or is corrupt. See UnsignedVlq.MaxValue.");
				}
#endif

				// Increment bit offset
				bit += PacketSize;
			} while ((b & More) > 0); // If not final byte

			// Remove zero offset
			symbol--;

			// Add to output
			return symbol;
		}
	}
}