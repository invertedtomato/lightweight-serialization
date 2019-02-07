using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class StringTests {
		[Fact]
		public void Deserialize_String_1() {
			Assert.Equal("a", LightWeight.Deserialize<String>(new[] {(Byte) 'a'}));
		}

		[Fact]
		public void Deserialize_String_Zero() {
			Assert.Equal(String.Empty, LightWeight.Deserialize<String>(new Byte[] { })); // TODO: handling of nulls/empties?
		}

		[Fact]
		public void Serialize_String_1() {
			Assert.Equal(new[] {(Byte) 'a'}, LightWeight.Serialize("a"));
		}


		[Fact]
		public void Serialize_String_Null() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize<String>(null));
		}

		[Fact]
		public void Serialize_String_Zero() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize(String.Empty));
		}
	}
}