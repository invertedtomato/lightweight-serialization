using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class SInt16Tests {
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
	}
}