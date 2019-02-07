using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class UInt32Tests {
		[Fact]
		public void UInt32DeserializeMax() {
			Assert.Equal(Byte.MaxValue, LightWeight.Deserialize<Byte>("FF".ParseAsHex()));
		}

		[Fact]
		public void UInt32DeserializeMin() {
			Assert.Equal(Byte.MinValue, LightWeight.Deserialize<Byte>("00".ParseAsHex()));
		}

		[Fact]
		public void UInt32SerializeMax() {
			Assert.Equal("FF", LightWeight.Serialize(Byte.MaxValue).ToHexString());
		}

		[Fact]
		public void UInt32SerializeMin() {
			Assert.Equal("00", LightWeight.Serialize(Byte.MinValue).ToHexString());
		}
	}
}