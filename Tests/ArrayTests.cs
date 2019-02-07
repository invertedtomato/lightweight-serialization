using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class ArrayTests {
		[Fact]
		public void Deserialize_Array_Bool() {
			var result = LightWeight.Deserialize<Boolean[]>(new Byte[] {
				0x81, // [0]
				0x00, // true
				0x81, // [1]
				0x00, // true
				0x80 // [2]
				// false
			});

			Assert.Equal(new[] {true, true, false}, result);
		}

		[Fact]
		public void Deserialize_Array_SInt16() {
			var result = LightWeight.Deserialize<Int16[]>(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x82, // [2]
				0xE8, 0x03 // 1000
			});

			Assert.Equal(new Int16[] {1, 2, 1000}, result);
		}

		[Fact]
		public void Deserialize_Array_SInt32() {
			var result = LightWeight.Deserialize<Int32[]>(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x82, // [2]
				0xE8, 0x03 // 1000
			});

			Assert.Equal(new[] {1, 2, 1000}, result);
		}

		[Fact]
		public void Deserialize_Array_SInt64() {
			var result = LightWeight.Deserialize<Int64[]>(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x82, // [2]
				0xE8, 0x03 // 1000
			});

			Assert.Equal(new Int64[] {1, 2, 1000}, result);
		}

		[Fact]
		public void Deserialize_Array_SInt8() {
			var result = LightWeight.Deserialize<SByte[]>(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x81, // [2]
				0x03 // 3
			});

			Assert.Equal(new SByte[] {1, 2, 3}, result);
		}

		[Fact]
		public void Deserialize_Array_String() {
			var result = LightWeight.Deserialize<String[]>(new Byte[] {
				0x81, // [0]
				(Byte) 'a',
				0x82, // [1]
				(Byte) 'b', (Byte) 'c',
				0x80 // [2]
			});

			Assert.Equal(new[] {"a", "bc", ""}, result);
		}

		[Fact]
		public void Deserialize_Array_UInt16() {
			var result = LightWeight.Deserialize<UInt16[]>(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x82, // [2]
				0xE8, 0x03 // 1000
			});

			Assert.Equal(new UInt16[] {1, 2, 1000}, result);
		}

		[Fact]
		public void Deserialize_Array_UInt32() {
			var result = LightWeight.Deserialize<UInt32[]>(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x82, // [2]
				0xE8, 0x03 // 1000
			});

			Assert.Equal(new UInt32[] {1, 2, 1000}, result);
		}

		[Fact]
		public void Deserialize_Array_UInt64() {
			var result = LightWeight.Deserialize<UInt64[]>(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x82, // [2]
				0xE8, 0x03 // 1000
			});

			Assert.Equal(new UInt64[] {1, 2, 1000}, result);
		}

		[Fact]
		public void Deserialize_Array_UInt8() {
			var result = LightWeight.Deserialize<Byte[]>(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x81, // [3]
				0xff, // 255
				0x81, // [2]
				0x03 // 3
			});

			Assert.Equal(new Byte[] {1, 2, 255, 3}, result);
		}

		[Fact]
		public void Serialize_Array_Bool() {
			var serialized = LightWeight.Serialize(new[] {true, true, false});

			Assert.Equal(new Byte[] {
				0x81, // [0]
				0x00, // true
				0x81, // [1]
				0x00, // true
				0x80 // [2]
				// false
			}, serialized);
		}

		[Fact]
		public void Serialize_Array_Null() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize<Boolean[]>(null));
		}

		[Fact]
		public void Serialize_Array_SInt16() {
			var serialized = LightWeight.Serialize(new Int16[] {1, 2, 1000});

			Assert.Equal(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x82, // [2]
				0xE8, 0x03 // 1000
			}, serialized);
		}

		[Fact]
		public void Serialize_Array_SInt32() {
			var serialized = LightWeight.Serialize(new[] {1, 2, 1000});

			Assert.Equal(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x82, // [2]
				0xE8, 0x03 // 1000
			}, serialized);
		}

		[Fact]
		public void Serialize_Array_SInt64() {
			var serialized = LightWeight.Serialize(new Int64[] {1, 2, 1000});

			Assert.Equal(new Byte[] {
				0x81, // [0]
				0x01, // 1
				0x81, // [1]
				0x02, // 2
				0x82, // [2]
				0xE8, 0x03 // 1000
			}, serialized);
		}

		[Fact]
		public void Serialize_Array_SInt8() {
			var serialized = LightWeight.Serialize(new SByte[] {1, 2, 3});

			Assert.Equal(new Byte[] {
				0x81, // [0]=
				0x01, //   1
				0x81, // [1]=
				0x02, //   2
				0x81, // [2]=
				0x03 //   3
			}, serialized);
		}

		[Fact]
		public void Serialize_Array_String() {
			var serialized = LightWeight.Serialize(new[] {"a", "bc", "def", "", null});

			Assert.Equal(new Byte[] {
				0x81, // [0]=
				(Byte) 'a',
				0x82, // [1]=
				(Byte) 'b', (Byte) 'c',
				0x83, // [2]=
				(Byte) 'd', (Byte) 'e', (Byte) 'f',
				0x80, //[3]=
				0x80 // [4]=
			}, serialized);
		}

		[Fact]
		public void Serialize_Array_UInt16() {
			var serialized = LightWeight.Serialize(new UInt16[] {1, 2, 1000});

			Assert.Equal(new Byte[] {
				0x81, // [0]=
				0x01, //   1
				0x81, // [1]=
				0x02, //   2
				0x82, // [2]=
				0xE8, 0x03 // 1000
			}, serialized);
		}

		[Fact]
		public void Serialize_Array_UInt32() {
			var serialized = LightWeight.Serialize(new UInt32[] {1, 2, 1000});

			Assert.Equal(new Byte[] {
				0x81, // [0]=
				0x01, //   1
				0x81, // [1]=
				0x02, //   2
				0x82, // [2]=
				0xE8, 0x03 // 1000
			}, serialized);
		}

		[Fact]
		public void Serialize_Array_UInt64() {
			var serialized = LightWeight.Serialize(new UInt64[] {1, 2, 1000});

			Assert.Equal(new Byte[] {
				0x81, // [0]=
				0x01, //   1
				0x81, // [1]=
				0x02, //   2
				0x82, // [2]=
				0xE8, 0x03 // 1000
			}, serialized);
		}

		[Fact]
		public void Serialize_Array_UInt8() {
			var serialized = LightWeight.Serialize(new Byte[] {1, 2, 3});

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