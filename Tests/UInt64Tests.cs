using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class UInt64Tests {
		[Fact]
		public void UInt64SerializeMin() {
			Assert.Equal(UnsignedVlq.Encode(UnsignedVlq.MinValue), LightWeight.Serialize(UnsignedVlq.MinValue));
		}
		
		[Fact]
		public void UInt64SerializeMax() {
			Assert.Equal(UnsignedVlq.Encode(UnsignedVlq.MaxValue), LightWeight.Serialize(UnsignedVlq.MaxValue));
		}
		
		[Fact]
		public void UInt64DeserializeMin() {
			Assert.Equal(UnsignedVlq.MinValue, LightWeight.Deserialize<UInt64>(UnsignedVlq.Encode(UnsignedVlq.MinValue)));
		}
		
		[Fact]
		public void UInt64DeserializeMax() {
			Assert.Equal(UnsignedVlq.MaxValue, LightWeight.Deserialize<UInt64>(UnsignedVlq.Encode(UnsignedVlq.MaxValue)));
		}
	}
}