using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class SingleTests {
		[Fact]
		public void SingleSerializeMin() {
			Assert.Equal(BitConverter.GetBytes(Single.MinValue), LightWeight.Serialize(Single.MinValue));
		}
		
		[Fact]
		public void SingleSerializeMax() {
			Assert.Equal(BitConverter.GetBytes(Single.MaxValue), LightWeight.Serialize(Single.MaxValue));
		}



		[Fact] 
		public void SingleDeserializeMin() {
			Assert.Equal(Single.MinValue, LightWeight.Deserialize<Single>(BitConverter.GetBytes(Single.MinValue)));
		}
		
		[Fact]
		public void SingleDeserializeMax() {
			Assert.Equal(Single.MaxValue, LightWeight.Deserialize<Single>(BitConverter.GetBytes(Single.MaxValue)));
		}
	}
}