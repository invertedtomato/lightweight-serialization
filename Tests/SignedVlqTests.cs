using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class SignedVlqTests {
		[Fact]
		public void SignedVlqNeg100To100() {
			for (var i = -100; i < 100; i++) {
				var encoded = SignedVlq.Encode(i);
				var output = SignedVlq.Decode(encoded);
				
				Assert.Equal(i, output);
			}
		}
	}
}