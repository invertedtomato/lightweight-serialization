using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class SInt16Tests {
		enum TestEnum : Int16 {
			A,
			B
		}
		
		
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
		public void SInt16SerializeEnum() {
			Assert.Equal(SignedVlq.Encode((Int16)TestEnum.B), LightWeight.Serialize(TestEnum.B));
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
		public void SInt16DeserializeEnum() {
			Assert.Equal(TestEnum.B, LightWeight.Deserialize<TestEnum>(SignedVlq.Encode((Int16)TestEnum.B)));
		}
		
		[Fact]
		public void SInt16DeserializeMax() {
			Assert.Equal(Int16.MaxValue, LightWeight.Deserialize<Int16>(SignedVlq.Encode(Int16.MaxValue)));
		}
	}
}