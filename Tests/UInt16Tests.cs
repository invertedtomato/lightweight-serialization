using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class UInt16Tests {
		[Fact]
		public void UInt16SerializeMin() {
			Assert.Equal(UnsignedVlq.Encode(UInt16.MinValue), LightWeight.Serialize(UInt16.MinValue));
		}
		
		[Fact]
		public void UInt16SerializeMax() {
			Assert.Equal(UnsignedVlq.Encode(UInt16.MaxValue), LightWeight.Serialize(UInt16.MaxValue));
		}
		
		[Fact]
		public void UInt16DeserializeMin() {
			Assert.Equal(UInt16.MinValue, LightWeight.Deserialize<UInt16>(UnsignedVlq.Encode(UInt16.MinValue)));
		}
		
		[Fact]
		public void UInt16DeserializeMax() {
			Assert.Equal(UInt16.MaxValue, LightWeight.Deserialize<UInt16>(UnsignedVlq.Encode(UInt16.MaxValue)));
		}
	}
}