using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class UInt64Tests {
		
		[Fact]
		public void Serialize_UInt64_Min() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
		}

		[Fact]
		public void Serialize_UInt64_UMax() {
			Assert.Equal(new Byte[] {0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff}, LightWeight.Serialize(UInt64.MaxValue));
		}
		
		[Fact]
		public void Deserialize_UInt64() {
			Assert.Equal((UInt64) UInt32.MaxValue + 1, LightWeight.Deserialize<UInt64>(new Byte[] {0, 0, 0, 0, 1, 0, 0, 0}));
		}
		
		[Fact]
		public void Deserialize_UMax() {
			Assert.Equal(UInt64.MaxValue, LightWeight.Deserialize<UInt64>(new Byte[] {255, 255, 255, 255, 255, 255, 255, 255}));
		}
	}
}