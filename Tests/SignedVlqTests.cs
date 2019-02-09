using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class SignedVlqTests {
		[Fact]
		public void SignedVlqNeg100To100() {
			for (var i = -100; i < 100; i++) {
				var encoded = SignedVlq.Encode(i);
				var output = SignedVlq.Decode(encoded);

				Assert.Equal(i, output);
			}
		}

		[Fact]
		public void SignedVlqEncodeDecode16() {
			var encoded = SignedVlq.Encode(Int16.MaxValue);
			Assert.Equal(Int16.MaxValue, SignedVlq.Decode(encoded));
		}

		[Fact]
		public void SignedVlqEncodeDecode32() {
			var encoded = SignedVlq.Encode(Int32.MaxValue);
			Assert.Equal(Int32.MaxValue, SignedVlq.Decode(encoded));
		}

		[Fact]
		public void SignedVlqEncodeDecodeA() {
			var encoded = SignedVlq.Encode(SignedVlq.MaxValue - Int32.MaxValue ^ 16);
			Assert.Equal(SignedVlq.MaxValue - Int32.MaxValue ^ 16, SignedVlq.Decode(encoded));
		}

		[Fact]
		public void SignedVlqEncodeMin() {
			Assert.Equal(new Byte[] {0b11111101, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000}, SignedVlq.Encode(SignedVlq.MinValue));
		}

		[Fact]
		public void SignedVlqDecodeMin() {
			Assert.Equal(SignedVlq.MinValue, SignedVlq.Decode(new Byte[] {0b11111101, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000}));
		}

		[Fact]
		public void SignedVlqEncodeMax() {
			Assert.Equal(new Byte[] {0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000}, SignedVlq.Encode(SignedVlq.MaxValue));
		}

		[Fact]
		public void SignedVlqDecodeMax() {
			Assert.Equal(SignedVlq.MaxValue, SignedVlq.Decode(new Byte[] {0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b00000000}));
		}

		[Fact]
		public void SignedVlqDecode4611686018427387903() {
			Assert.Equal(4611686018427387903, SignedVlq.Decode(new Byte[] {0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b11111110, 0b01111110}));
		}
	}
}