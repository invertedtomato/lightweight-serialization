using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class UInt32Tests {
		

		[Fact]
		public void Serialize_UInt32_Max() {
			Assert.Equal(new Byte[] {0xff, 0xff, 0xff, 0xff}, LightWeight.Serialize(UInt32.MaxValue));
		}

		[Fact]
		public void Serialize_UInt32_Min() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
		}

		
		
		[Fact]
		public void Deserialize_UInt32() {
			Assert.Equal((UInt32) UInt16.MaxValue + 1, LightWeight.Deserialize<UInt32>(new Byte[] {0, 0, 1, 0}));
		}
	}
}