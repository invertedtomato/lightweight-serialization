using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class BooleanTests {
		[Fact]
		public void BooleanDeserializeFalse() {
			Assert.False(LightWeight.Deserialize<Boolean>("00".ParseAsHex()));
		}

		[Fact]
		public void BooleanDeserializeTrue() {
			Assert.True(LightWeight.Deserialize<Boolean>("01".ParseAsHex()));
		}

		[Fact]
		public void BooleanSerializeFalse() {
			Assert.Equal("00", LightWeight.Serialize(false).ToHexString());
		}

		[Fact]
		public void BooleanSerializeTrue() {
			Assert.Equal("01", LightWeight.Serialize(true).ToHexString());
		}
	}
}