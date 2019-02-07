using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class SInt8Tests {
		[Fact]
		public void Serialize_SInt8_Max() {
			Assert.Equal(new Byte[] {0x7f}, LightWeight.Serialize(SByte.MaxValue));
		}

		[Fact]
		public void Serialize_SInt8_Min() {
			Assert.Equal(new Byte[] {0x80}, LightWeight.Serialize(SByte.MinValue));
		}

		[Fact]
		public void Serialize_SInt8_Zero() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
		}
		[Fact]
		public void Deserialize_SInt8_Max() {
			Assert.Equal(SByte.MaxValue, LightWeight.Deserialize<SByte>(new Byte[] {127})); // TODO: check
		}


		[Fact]
		public void Deserialize_SInt8_Min() {
			Assert.Equal(SByte.MinValue, LightWeight.Deserialize<SByte>(new Byte[] {128})); // TODO: check
		}

		[Fact]
		public void Deserialize_SInt8_Zero() {
			Assert.Equal(0, LightWeight.Deserialize<SByte>(new Byte[] { }));
		}

	}
}