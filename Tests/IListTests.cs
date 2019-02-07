using System;
using System.Collections.Generic;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class IListTests {
		[Fact]
		public void Deserialize_IList_SInt32() {
			var result = LightWeight.Deserialize<List<Int32>>(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x82, // [3]
				0xFF, 0x00, // 255
				0x81, // [2]
				0x03 // 3
			});

			Assert.Equal(new List<Int32> {1, 2, 255, 3}, result);
		}

		[Fact]
		public void Serialize_IList_SInt32() {
			var serialized = LightWeight.Serialize(new List<Int32> {1, 2, 3});

			Assert.Equal(new Byte[] {
				0x81, // [0]=
				0x01, //   1
				0x81, // [1]=
				0x02, //   2
				0x81, // [2]=
				0x03 //   3
			}, serialized);
		}
	}
}