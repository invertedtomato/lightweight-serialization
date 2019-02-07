using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class SInt64Tests {
		[Fact]
		public void Deserialize_SInt64_Max() {
			Assert.Equal(Int64.MaxValue, LightWeight.Deserialize<Int64>(new Byte[] {255, 255, 255, 255, 255, 255, 255, 127}));
		}

		[Fact]
		public void Deserialize_SInt64_Min() {
			Assert.Equal(Int64.MinValue, LightWeight.Deserialize<Int64>(new Byte[] {0, 0, 0, 0, 0, 0, 0, 128}));
		}

		[Fact]
		public void Deserialize_SInt64_Zero() {
			Assert.Equal(0, LightWeight.Deserialize<Int64>(new Byte[] { }));
		}

		[Fact]
		public void Serialize_SInt64_Max() {
			Assert.Equal(new Byte[] {0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f}, LightWeight.Serialize(Int64.MaxValue));
		}

		[Fact]
		public void Serialize_SInt64_Min() {
			Assert.Equal(new Byte[] {0, 0, 0, 0, 0, 0, 0, 0x80}, LightWeight.Serialize(Int64.MinValue));
		}

		[Fact]
		public void Serialize_SInt64_Zero() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
		}
	}
}