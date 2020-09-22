using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class UInt16Tests {
		enum TestEnum : UInt16 {
			A,
			B
		}

		[Fact]
		public void UInt16SerializeMin() {
			Assert.Equal(UnsignedVlq.Encode(UInt16.MinValue), LightWeight.Serialize(UInt16.MinValue));
		}
		
		[Fact]
		public void UInt16SerializeMax() {
			Assert.Equal(UnsignedVlq.Encode(UInt16.MaxValue), LightWeight.Serialize(UInt16.MaxValue));
		}
		
		[Fact]
		public void UInt16SerializeEnum() {
			Assert.Equal(UnsignedVlq.Encode((UInt16)TestEnum.B).ToArray().ToHexString(), LightWeight.Serialize(TestEnum.B).ToHexString());
		}
		
		
		
		[Fact]
		public void UInt16DeserializeMin() {
			Assert.Equal(UInt16.MinValue, LightWeight.Deserialize<UInt16>(UnsignedVlq.Encode(UInt16.MinValue).ToArray()));
		}
		
		[Fact]
		public void UInt16DeserializeMax() {
			Assert.Equal(UInt16.MaxValue, LightWeight.Deserialize<UInt16>(UnsignedVlq.Encode(UInt16.MaxValue).ToArray()));
		}
		
		[Fact]
		public void UInt16DeserializeEnum() {
			Assert.Equal(TestEnum.B, LightWeight.Deserialize<TestEnum>(UnsignedVlq.Encode((UInt16)TestEnum.B).ToArray()));
		}
	}
}