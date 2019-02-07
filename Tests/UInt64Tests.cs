using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class UInt64Tests {
		[Fact]
		public void UInt64DeserializeMax() {
			Assert.Equal(Byte.MaxValue, LightWeight.Deserialize<Byte>("FF".ParseAsHex()));
		}

		[Fact]
		public void UInt64DeserializeMin() {
			Assert.Equal(Byte.MinValue, LightWeight.Deserialize<Byte>("00".ParseAsHex()));
		}

		[Fact]
		public void UInt64SerializeMax() {
			Assert.Equal("FF", LightWeight.Serialize(Byte.MaxValue).ToHexString());
		}

		[Fact]
		public void UInt64SerializeMin() {
			Assert.Equal("00", LightWeight.Serialize(Byte.MinValue).ToHexString());
		}
	}
}