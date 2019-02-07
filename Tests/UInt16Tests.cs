using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class UInt16Tests {
		[Fact]
		public void Serialize_SInt16_Max() {
			Assert.Equal(new Byte[] {0xff, 0x7f}, LightWeight.Serialize(Int16.MaxValue));
		}

		[Fact]
		public void Serialize_SInt16_Min() {
			Assert.Equal(new Byte[] {0, 0x80}, LightWeight.Serialize(Int16.MinValue));
		}

		[Fact]
		public void Serialize_SInt16_Zero() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
		}


		[Fact]
		public void Deserialize_UInt16() {
			Assert.Equal(Byte.MaxValue + 1, LightWeight.Deserialize<UInt16>(new Byte[] {0, 1}));
		}
	}
}