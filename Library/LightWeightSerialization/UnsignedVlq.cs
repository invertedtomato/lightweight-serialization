using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public static class UnsignedVlq {
		public const UInt64 MaxValue = UInt64.MaxValue - 1;

		private const Byte More = 0b10000000;
		private const Byte Mask = 0b01111111;
		private const Int32 PacketSize = 7;
		private const UInt64 MinPacketValue = UInt64.MaxValue >> (64 - PacketSize);

		public static Byte[] Encode(UInt64 value) {
#if DEBUG
			if (value > MaxValue) {
				throw new OverflowException("Symbol is larger than maximum supported value. See VLQCodec.MaxValue");
			}
#endif

			using (var output = new MemoryStream()) {
				// Iterate through input, taking X bits of data each time, aborting when less than X bits left
				while (value > MinPacketValue) {
					// Write payload, skipping MSB bit
					output.WriteByte((Byte) ((value & Mask) | More));

					// Offset value for next cycle
					value >>= PacketSize;
					value--;
				}

				// Write remaining - marking it as the final byte for symbol
				output.WriteByte((Byte) (value & Mask));

				return output.ToArray();
			}
		}

		public static UInt64 Decode(Byte[] input) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}
#endif
			using (var stream = new MemoryStream(input)) {
				return Decode(stream);
			}
		}

		public static UInt64 Decode(Stream input) {
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
				if ((b = input.ReadByte()) == -1) {
					throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
				}

				// Add input bits to output
				var chunk = (UInt64) (b & Mask);
				var pre = symbol;
				symbol += (chunk + 1) << bit;

#if DEBUG
				// Check for overflow
				if (symbol < pre) {
					throw new OverflowException("Input symbol larger than the supported limit of 64 bits. Probable corrupt input.");
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