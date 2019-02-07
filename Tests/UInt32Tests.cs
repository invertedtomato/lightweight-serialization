using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class UInt32Tests {
		[Fact]
		public void UInt32SerializeMin() {
			Assert.Equal(UnsignedVlq.Encode(UInt32.MinValue), LightWeight.Serialize(UInt32.MinValue));
		}
		
		[Fact]
		public void UInt32SerializeMax() {
			Assert.Equal(UnsignedVlq.Encode(UInt32.MaxValue), LightWeight.Serialize(UInt32.MaxValue));
		}
		
		[Fact]
		public void UInt32DeserializeMin() {
			Assert.Equal(UInt32.MinValue, LightWeight.Deserialize<UInt32>(UnsignedVlq.Encode(UInt32.MinValue)));
		}
		
		[Fact]
		public void UInt32DeserializeMax() {
			Assert.Equal(UInt32.MaxValue, LightWeight.Deserialize<UInt32>(UnsignedVlq.Encode(UInt32.MaxValue)));
		}
	}
}