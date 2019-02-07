using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class UnsignedVlqTests {
		[Fact]
		public void Encode0() {
			Assert.Equal(new Byte[] {0b00000000}, UnsignedVlq.Encode(0));
		}

		[Fact]
		public void Encode1() {
			Assert.Equal(new Byte[] {0b00000001}, UnsignedVlq.Encode(1));
		}

		[Fact]
		public void Encode2() {
			Assert.Equal(new Byte[] {0b00000010}, UnsignedVlq.Encode(2));
		}

		[Fact]
		public void Encode3() {
			Assert.Equal(new Byte[] {0b00000011}, UnsignedVlq.Encode(3));
		}

		[Fact]
		public void Encode127() {
			Assert.Equal(new Byte[] {0b01111111}, UnsignedVlq.Encode(127));
		}

		[Fact]
		public void Encode128() {
			Assert.Equal(new Byte[] {0b10000000, 0b00000000}, UnsignedVlq.Encode(128));
		}

		[Fact]
		public void Encode129() {
			Assert.Equal(new Byte[] {0b10000001, 0b00000000}, UnsignedVlq.Encode(129));
		}

		[Fact]
		public void Encode16511() {
			Assert.Equal(new Byte[] {0b10000000, 0b01111111}, UnsignedVlq.Encode(16511));
		}

		[Fact]
		public void Encode16512() {
			Assert.Equal(new Byte[] {0b10000000, 0b10000000, 0b00000000}, UnsignedVlq.Encode(16512));
		}

		[Fact]
		public void Encode2113663() {
			Assert.Equal(new Byte[] {0b11111111, 0b11111111, 0b01111111}, UnsignedVlq.Encode(2113663));
		}

		[Fact]
		public void Encode2113664() {
			Assert.Equal(new Byte[] {0b10000000, 0b10000000, 0b10000000, 0b00000000}, UnsignedVlq.Encode(2113664));
		}

		[Fact]
		public void EncodeMax() {
			Assert.Equal(new Byte[] {0b11111110, 0b11111110, 0b11111110, 0b11111110,0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000,}, UnsignedVlq.Encode(UnsignedVlq.MaxValue));
		}

		[Fact]
		public void EncodeOverflow() {
			Assert.Throws<OverflowException>(() => { UnsignedVlq.Encode(UInt64.MaxValue); });
		}


		[Fact]
		public void Decode0() {
			Assert.Equal((UInt64) 0, DecompressOne("10000000", 1));
		}

		[Fact]
		public void Decode1() {
			Assert.Equal((UInt64) 1, DecompressOne("10000001", 1));
		}

		[Fact]
		public void Decode2() {
			Assert.Equal((UInt64) 2, DecompressOne("10000010", 1));
		}

		[Fact]
		public void Decode3() {
			Assert.Equal((UInt64) 3, DecompressOne("10000011", 1));
		}

		[Fact]
		public void Decode127() {
			Assert.Equal((UInt64) 127, DecompressOne("11111111", 1));
		}

		[Fact]
		public void Decode128() {
			Assert.Equal((UInt64) 128, DecompressOne("00000000 10000000", 2));
		}

		[Fact]
		public void Decode129() {
			Assert.Equal((UInt64) 129, DecompressOne("00000001 10000000", 2));
		}

		[Fact]
		public void Decode16511() {
			Assert.Equal((UInt64) 16511, DecompressOne("01111111 11111111", 2));
		}

		[Fact]
		public void Decode16512() {
			Assert.Equal((UInt64) 16512, DecompressOne("00000000 00000000 10000000", 3));
		}

		[Fact]
		public void Decode16513() {
			Assert.Equal((UInt64) 16513, DecompressOne("00000001 00000000 10000000", 3));
		}

		[Fact]
		public void Decode2113663() {
			Assert.Equal((UInt64) 2113663, DecompressOne("01111111 01111111 11111111", 3));
		}

		[Fact]
		public void Decode2113664() {
			Assert.Equal((UInt64) 2113664, DecompressOne("00000000 00000000 00000000 10000000", 4));
		}

		[Fact]
		public void DecodeMax() {
			Assert.Equal(VLQCodec.MaxValue, DecompressOne("01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000", 10));
		}

		[Fact]
		public void Decode1_1_1() {
			var set = DecompressMany("10000001 10000001 10000001", 3, 3);
			Assert.Equal(3, set.Length);
			Assert.Equal((UInt64) 1, set[0]);
			Assert.Equal((UInt64) 1, set[1]);
			Assert.Equal((UInt64) 1, set[2]);
		}

		[Fact]
		public void DecodeInputClipped() {
			Assert.Throws<EndOfStreamException>(() => {
				var input = new MemoryStream(BitOperation.ParseToBytes("00000000"));
				var output = Codec.DecompressUnsigned(input, 1).ToArray();
			});
		}

		[Fact]
		public void DecodeOverflow() {
			Assert.Throws<OverflowException>(() => { DecompressOne("01111111 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 01111110 10000000", 11); });
		}

		[Fact]
		public void Decode1_X() {
			var input = new MemoryStream(BitOperation.ParseToBytes("10000001 00000011"));
			Assert.Equal((UInt64) 1, Codec.DecompressUnsigned(input, 1).Single());
		}
	}
}