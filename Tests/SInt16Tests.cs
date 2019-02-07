using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class SInt16Tests {
		[Fact]
		public void SInt16SerializeMin() {
			Assert.Equal(SignedVlq.Encode(Int16.MinValue), LightWeight.Serialize(Int16.MinValue));
		}
		
		[Fact]
		public void SInt16SerializeZero() {
			Assert.Equal(SignedVlq.Encode(0), LightWeight.Serialize(0));
		}
		
		[Fact]
		public void SInt16SerializeMax() {
			Assert.Equal(SignedVlq.Encode(Int16.MaxValue), LightWeight.Serialize(Int16.MaxValue));
		}
		
		[Fact]
		public void SInt16DeserializeMin() {
			Assert.Equal(Int16.MinValue, LightWeight.Deserialize<Int16>(SignedVlq.Encode(Int16.MinValue)));
		}
		
		[Fact]
		public void SInt16DeserializeZero() {
			Assert.Equal((Int16)0, LightWeight.Deserialize<Int16>(SignedVlq.Encode(0)));
		}
		
		[Fact]
		public void SInt16DeserializeMax() {
			Assert.Equal(Int16.MaxValue, LightWeight.Deserialize<Int16>(SignedVlq.Encode(Int16.MaxValue)));
		}
	}
}