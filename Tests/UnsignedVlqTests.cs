using System;
using System.IO;
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
			Assert.Equal(new Byte[] {0b11111111, 0b01111111}, UnsignedVlq.Encode(16511));
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
			Assert.Equal(new Byte[] {0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000}, UnsignedVlq.Encode(UnsignedVlq.MaxValue));
		}

		[Fact]
		public void EncodeOverflow() {
			Assert.Throws<OverflowException>(() => { UnsignedVlq.Encode(UInt64.MaxValue); });
		}

		[Fact]
		public void Decode0() {
			Assert.Equal((UInt64) 0, UnsignedVlq.Decode(new Byte[] {0b00000000}));
		}

		[Fact]
		public void Decode1() {
			Assert.Equal((UInt64) 1, UnsignedVlq.Decode(new Byte[] {0b00000001}));
		}

		[Fact]
		public void Decode2() {
			Assert.Equal((UInt64) 2, UnsignedVlq.Decode(new Byte[] {0b00000010}));
		}

		[Fact]
		public void Decode3() {
			Assert.Equal((UInt64) 3, UnsignedVlq.Decode(new Byte[] {0b00000011}));
		}

		[Fact]
		public void Decode127() {
			Assert.Equal((UInt64) 127, UnsignedVlq.Decode(new Byte[] {0b01111111}));
		}

		[Fact]
		public void Decode128() {
			Assert.Equal((UInt64) 128, UnsignedVlq.Decode(new Byte[] {0b10000000, 0b00000000}));
		}

		[Fact]
		public void Decode129() {
			Assert.Equal((UInt64) 129, UnsignedVlq.Decode(new Byte[] {0b10000001, 0b00000000}));
		}

		[Fact]
		public void Decode16511() {
			Assert.Equal((UInt64) 16511, UnsignedVlq.Decode(new Byte[] {0b11111111, 0b01111111}));
		}

		[Fact]
		public void Decode16512() {
			Assert.Equal((UInt64) 16512, UnsignedVlq.Decode(new Byte[] {0b10000000, 0b10000000, 0b00000000}));
		}

		[Fact]
		public void Decode16513() {
			Assert.Equal((UInt64) 16513, UnsignedVlq.Decode(new Byte[] {0b10000001, 0b10000000, 0b00000000}));
		}

		[Fact]
		public void Decode2113663() {
			Assert.Equal((UInt64) 2113663, UnsignedVlq.Decode(new Byte[] {0b11111111, 0b11111111, 0b01111111}));
		}

		[Fact]
		public void Decode2113664() {
			Assert.Equal((UInt64) 2113664, UnsignedVlq.Decode(new Byte[] {0b10000000, 0b10000000, 0b10000000, 0b00000000}));
		}

		[Fact]
		public void DecodeMax() {
			Assert.Equal(UnsignedVlq.MaxValue, UnsignedVlq.Decode(new Byte[] {0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000}));
		}

		[Fact]
		public void Decode1_1_1() {
			var stream = new DecodeBuffer(new Byte[] {0b00000001, 0b00000001, 0b00000001});
			Assert.Equal((UInt64) 1, UnsignedVlq.Decode(stream));
			Assert.Equal((UInt64) 1, UnsignedVlq.Decode(stream));
			Assert.Equal((UInt64) 1, UnsignedVlq.Decode(stream));
			Assert.Throws<EndOfStreamException>(() => { return UnsignedVlq.Decode(stream); });
		}

		[Fact]
		public void DecodeInputClipped() {
			Assert.Throws<EndOfStreamException>(() => { UnsignedVlq.Decode(new Byte[] {0b10000000}); });
		}

		[Fact]
		public void DecodeOverflow() {
			Assert.Throws<OverflowException>(() => { UnsignedVlq.Decode(new Byte[] {0b11111111, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000}); });
		}

		[Fact]
		public void Decode1_X() {
			Assert.Equal((UInt64) 1, UnsignedVlq.Decode(new Byte[] {0b00000001, 0b10000011}));
		}
	}
}