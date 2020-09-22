using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class DoubleTests {
		[Fact]
		public void DoubleSerializeMin() {
			Assert.Equal(BitConverter.GetBytes(Double.MinValue), LightWeight.Serialize(Double.MinValue));
		}
		
		[Fact]
		public void DoubleSerializeMax() {
			Assert.Equal(BitConverter.GetBytes(Double.MaxValue), LightWeight.Serialize(Double.MaxValue));
		}



		[Fact] 
		public void DoubleDeserializeMin() {
			Assert.Equal(Double.MinValue, LightWeight.Deserialize<Double>(BitConverter.GetBytes(Double.MinValue)));
		}
		
		[Fact]
		public void DoubleDeserializeMax() {
			Assert.Equal(Double.MaxValue, LightWeight.Deserialize<Double>(BitConverter.GetBytes(Double.MaxValue)));
		}
	}
}