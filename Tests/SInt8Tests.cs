using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class SInt8Tests {
		enum TestEnum : SByte {
			A = 5,
			B = 6
		}


		[Fact]
		public void SInt8SerializeMin() {
			Assert.Equal(new Byte[] {128}, LightWeight.Serialize(SByte.MinValue));
		}

		[Fact]
		public void SInt8SerializeZero() {
			Assert.Equal(new Byte[] {0x00}, LightWeight.Serialize(0));
		}

		[Fact]
		public void SInt8SerializeMax() {
			Assert.Equal(new Byte[] {127}, LightWeight.Serialize(SByte.MaxValue));
		}

		[Fact]
		public void SInt8SerializeEnum() {
			Assert.Equal(new Byte[] {(Byte) (SByte) TestEnum.B}, LightWeight.Serialize(TestEnum.B));
		}


		[Fact]
		public void SInt8DeserializeMin() {
			Assert.Equal(SByte.MinValue, LightWeight.Deserialize<SByte>(new Byte[] {128}));
		}

		[Fact]
		public void SInt8DeserializeZero() {
			Assert.Equal((SByte) 0, LightWeight.Deserialize<SByte>(new Byte[] {0x00}));
		}

		[Fact]
		public void SInt8DeserializeMax() {
			Assert.Equal(SByte.MaxValue, LightWeight.Deserialize<SByte>(new Byte[] {127}));
		}

		[Fact]
		public void SInt8DeserializeEnum() {
			Assert.Equal(TestEnum.B, LightWeight.Deserialize<TestEnum>(new Byte[] {(Byte) (SByte) TestEnum.B}));
		}
	}
}