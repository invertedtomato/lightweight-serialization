using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class SInt16Tests {
		

		[Fact]
		public void Serialize_UInt16_Max() {
			Assert.Equal(new Byte[] {0xff, 0xff}, LightWeight.Serialize(UInt16.MaxValue));
		}

		[Fact]
		public void Serialize_UInt16_Min() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
		}
		
		[Fact]
		public void Deserialize_SInt16_Max() {
			Assert.Equal(Int16.MaxValue, LightWeight.Deserialize<Int16>(new Byte[] {255, 127}));
		}

		[Fact]
		public void Deserialize_SInt16_Min() {
			Assert.Equal(Int16.MinValue, LightWeight.Deserialize<Int16>(new Byte[] {0, 128}));
		}

		[Fact]
		public void Deserialize_SInt16_Zero() {
			Assert.Equal(0, LightWeight.Deserialize<Int16>(new Byte[] { }));
		}

	}
}