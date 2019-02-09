using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class UInt64Tests {
		enum TestEnum : UInt64 {
			A,
			B
		}

		[Fact]
		public void UInt64SerializeMin() {
			Assert.Equal(UnsignedVlq.Encode(UnsignedVlq.MinValue), LightWeight.Serialize(UnsignedVlq.MinValue));
		}

		[Fact]
		public void UInt64SerializeMax() {
			Assert.Equal(UnsignedVlq.Encode(UnsignedVlq.MaxValue), LightWeight.Serialize(UnsignedVlq.MaxValue));
		}

		[Fact]
		public void UInt64SerializeEnum() {
			Assert.Equal(UnsignedVlq.Encode((Int64) TestEnum.B), LightWeight.Serialize(TestEnum.B));
		}


		[Fact]
		public void UInt64DeserializeMin() {
			Assert.Equal(UnsignedVlq.MinValue, LightWeight.Deserialize<UInt64>(UnsignedVlq.Encode(UnsignedVlq.MinValue)));
		}

		[Fact]
		public void UInt64DeserializeMax() {
			Assert.Equal(UnsignedVlq.MaxValue, LightWeight.Deserialize<UInt64>(UnsignedVlq.Encode(UnsignedVlq.MaxValue)));
		}

		[Fact]
		public void UInt64DeserializeEnum() {
			Assert.Equal(TestEnum.B, LightWeight.Deserialize<TestEnum>(UnsignedVlq.Encode((UInt64) TestEnum.B)));
		}
	}
}