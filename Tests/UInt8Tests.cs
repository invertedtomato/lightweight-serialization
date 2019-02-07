using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class UInt8Tests {
		
		[Fact]
		public void Serialize_UInt8_Max() {
			Assert.Equal(new Byte[] {0xff}, LightWeight.Serialize(Byte.MaxValue));
		}

		[Fact]
		public void Serialize_UInt8_Min() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
		}
		
		[Fact]
		public void Deserialize_UInt8() {
			Assert.Equal(1, LightWeight.Deserialize<Byte>(new Byte[] {1}));
		}
	}
}