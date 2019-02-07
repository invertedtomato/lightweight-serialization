using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class UInt8Tests {
		[Fact]
		public void UInt8DeserializeMax() {
			Assert.Equal(Byte.MaxValue, LightWeight.Deserialize<Byte>("FF".ParseAsHex()));
		}

		[Fact]
		public void UInt8DeserializeMin() {
			Assert.Equal(Byte.MinValue, LightWeight.Deserialize<Byte>("00".ParseAsHex()));
		}

		[Fact]
		public void UInt8SerializeMax() {
			Assert.Equal("FF", LightWeight.Serialize(Byte.MaxValue).ToHexString());
		}

		[Fact]
		public void UInt8SerializeMin() {
			Assert.Equal("00", LightWeight.Serialize(Byte.MinValue).ToHexString());
		}
	}
}