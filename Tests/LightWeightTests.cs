using System;
using System.Collections.Generic;
using System.Linq;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

public class LightWeightTests {
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
	public void Deserialize_Bool_False() {
		Assert.False(LightWeight.Deserialize<Boolean>(new Byte[] { }));
	}

	[Fact]
	public void Deserialize_Bool_True() {
		Assert.True(LightWeight.Deserialize<Boolean>(new Byte[] {0x00}));
	}

	[Fact]
	public void Deserialize_Dict_SInt32_String() {
		var result = LightWeight.Deserialize<Dictionary<Int32, String>>(new Byte[] {
			0x81, 0x01, // 1 =
			0x81, (Byte) 'a', // 'a'
			0x81, 0x02, // 2 =
			0x81, (Byte) 'b', // 'b'
			0x81, 0x03, // 3 =
			0x81, (Byte) 'c' // 'c'
		});

		Assert.Equal(new Dictionary<Int32, String> {
			{1, "a"},
			{2, "b"},
			{3, "c"}
		}, result);
	}


	[Fact]
	public void Deserialize_List_SInt32() {
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
	public void Deserialize_POCO_Basic() {
		var result = LightWeight.Deserialize<ThreeInts>(new Byte[] {
			0x81, // B=
			0x09, // 9
			0x81, // A=
			0x01, // 1
			0x82, // C=
			0xE8, 0x03 // 1000
		});

		Assert.Equal(1, result.A);
		Assert.Equal(9, result.B);
		Assert.Equal(1000, result.C);
	}

	[Fact]
	public void Deserialize_POCO_Complex() {
		var result = LightWeight.Deserialize<Layered>(new Byte[] {
			0x84, // Y=
			0x74, 0x65, 0x73, 0x74, // "test"
			0x87, // Z=
			0x81, // B=
			0x09, // 9
			0x81, // A=
			0x01, // 1
			0x82, // C=
			0xE8, 0x03 // 1000
		});

		Assert.Equal("test", result.Y);
		Assert.Equal(1, result.Z.A);
		Assert.Equal(9, result.Z.B);
		Assert.Equal(1000, result.Z.C);
	}

	[Fact]
	public void Deserialize_POCO_Empty() {
		var result = LightWeight.Deserialize<Empty>(new Byte[] { });

		Assert.Equal(0, result.A);
		Assert.Equal(0, result.B);
		Assert.Equal(0, result.C);
	}

	[Fact]
	public void Deserialize_SInt16_Max() {
		Assert.Equal(Int16.MaxValue, LightWeight.Deserialize<Int16>(new Byte[] {255, 127}));
	}

	[Fact]
	public void Deserialize_SInt16_Min() {
		Assert.Equal(Int16.MinValue, LightWeight.Deserialize<Int16>(new Byte[] {0, 128}));
	}

	[Fact]
	public void Deserialize_SInt16_Zero() {
		Assert.Equal(0, LightWeight.Deserialize<Int16>(new Byte[] { }));
	}

	[Fact]
	public void Deserialize_SInt32_Max() {
		Assert.Equal(Int32.MaxValue, LightWeight.Deserialize<Int32>(new Byte[] {255, 255, 255, 127}));
	}

	[Fact]
	public void Deserialize_SInt32_255() {
		Assert.Equal(255, LightWeight.Deserialize<Int32>(new Byte[] {0xFF, 0}));
	}
	
	[Fact]
	public void Deserialize_SInt32_Min() {
		Assert.Equal(Int32.MinValue, LightWeight.Deserialize<Int32>(new Byte[] {0, 0, 0, 128}));
	}

	[Fact]
	public void Deserialize_SInt32_Zero() {
		Assert.Equal(0, LightWeight.Deserialize<Int32>(new Byte[] { }));
	}

	[Fact]
	public void Deserialize_SInt64_Max() {
		Assert.Equal(Int64.MaxValue, LightWeight.Deserialize<Int64>(new Byte[] {255, 255, 255, 255, 255, 255, 255, 127}));
	}

	[Fact]
	public void Deserialize_SInt64_Min() {
		Assert.Equal(Int64.MinValue, LightWeight.Deserialize<Int64>(new Byte[] {0, 0, 0, 0, 0, 0, 0, 128}));
	}

	[Fact]
	public void Deserialize_SInt64_Zero() {
		Assert.Equal(0, LightWeight.Deserialize<Int64>(new Byte[] { }));
	}

	[Fact]
	public void Deserialize_SInt8_Max() {
		Assert.Equal(SByte.MaxValue, LightWeight.Deserialize<SByte>(new Byte[] {127})); // TODO: check
	}


	[Fact]
	public void Deserialize_SInt8_Min() {
		Assert.Equal(SByte.MinValue, LightWeight.Deserialize<SByte>(new Byte[] {128})); // TODO: check
	}

	[Fact]
	public void Deserialize_SInt8_Zero() {
		Assert.Equal(0, LightWeight.Deserialize<SByte>(new Byte[] { }));
	}

	[Fact]
	public void Deserialize_String_1() {
		Assert.Equal("a", LightWeight.Deserialize<String>(new[] {(Byte) 'a'}));
	}

	[Fact]
	public void Deserialize_String_Zero() {
		Assert.Equal(String.Empty, LightWeight.Deserialize<String>(new Byte[] { })); // TODO: handling of nulls/empties?
	}

	[Fact]
	public void Deserialize_UInt16() {
		Assert.Equal(Byte.MaxValue + 1, LightWeight.Deserialize<UInt16>(new Byte[] {0, 1}));
	}

	[Fact]
	public void Deserialize_UInt32() {
		Assert.Equal((UInt32) UInt16.MaxValue + 1, LightWeight.Deserialize<UInt32>(new Byte[] {0, 0, 1, 0}));
	}

	[Fact]
	public void Deserialize_UInt64() {
		Assert.Equal((UInt64) UInt32.MaxValue + 1, LightWeight.Deserialize<UInt64>(new Byte[] {0, 0, 0, 0, 1, 0, 0, 0}));
	}

	[Fact]
	public void Deserialize_UInt8() {
		Assert.Equal(1, LightWeight.Deserialize<Byte>(new Byte[] {1}));
	}

	[Fact]
	public void Deserialize_UMax() {
		Assert.Equal(UInt64.MaxValue, LightWeight.Deserialize<UInt64>(new Byte[] {255, 255, 255, 255, 255, 255, 255, 255}));
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
			0x03  //   3
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
			0x03  //   3
		}, serialized);
	}

	[Fact]
	public void Serialize_Bool_False() {
		Assert.Equal(new Byte[] { }, LightWeight.Serialize(false));
	}

	[Fact]
	public void Serialize_Bool_True() {
		Assert.Equal(new Byte[] {0x00}, LightWeight.Serialize(true));
	}

	[Fact]
	public void Serialize_IDict_SInt32_String() {
		var serialized = LightWeight.Serialize(new Dictionary<Int32, String> {
			{1, "a"},
			{2, "b"},
			{3, "c"}
		});

		Assert.Equal(new Byte[] {
			0x81, 0x01, // 1 =
			0x81, (Byte) 'a', // 'a'
			0x81, 0x02, // 2 =
			0x81, (Byte) 'b', // 'b'
			0x81, 0x03, // 3 =
			0x81, (Byte) 'c' // 'c'
		}, serialized);
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
			0x03  //   3
		}, serialized);
	}

	[Fact]
	public void Serialize_POCO_Basic() {
		var serialized = LightWeight.Serialize(new ThreeInts {
			A = 1,
			B = 9,
			C = 1000
		});

		Assert.Equal(new Byte[] {
			0x81, // B=
			0x09, //   0
			0x81, // A=
			0x01, //   1
			0x82, // C=
			0xE8, 0x03 // 1000
		}, serialized);
	}

	[Fact]
	public void Serialize_POCO_Complex() {
		var serialized = LightWeight.Serialize(new Layered {
			Y = "test",
			Z = new ThreeInts {
				A = 1,
				B = 9,
				C = 1000
			}
		});
		
		Assert.Equal(new Byte[] {
			0x84, // Y=
			0x74, 0x65, 0x73, 0x74, // "test"
			0x87, // Z=
			0x81, //   B=
			0x09, //     9
			0x81, //   A=
			0x01, //     1
			0x82, //   C=
			0xE8, 0x03 // 1000
		}.ToHexString(), serialized.ToHexString());
	}

	[Fact]
	public void Serialize_POCO_Empty() {
		var target = new Empty {
			A = 1,
			B = 9,
			C = 1000
		};

		Assert.Equal(new Byte[] { }, LightWeight.Serialize(target));
	}

	[Fact]
	public void Serialize_SInt16_Max() {
		Assert.Equal(new Byte[] {0xff, 0x7f}, LightWeight.Serialize(Int16.MaxValue));
	}

	[Fact]
	public void Serialize_SInt16_Min() {
		Assert.Equal(new Byte[] {0, 0x80}, LightWeight.Serialize(Int16.MinValue));
	}

	[Fact]
	public void Serialize_SInt16_Zero() {
		Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
	}

	[Fact]
	public void Serialize_SInt32_Max() {
		Assert.Equal(new Byte[] {0xff, 0xff, 0xff, 0x7f}, LightWeight.Serialize(Int32.MaxValue));
	}

	[Fact]
	public void Serialize_SInt32_255() {
		Assert.Equal(new Byte[]{0xFF, 0x00}, LightWeight.Serialize(255));
	}
	
	[Fact]
	public void Serialize_SInt32_Min() {
		Assert.Equal(new Byte[] {0, 0, 0, 0x80}, LightWeight.Serialize(Int32.MinValue));
	}

	[Fact]
	public void Serialize_SInt32_Zero() {
		Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
	}

	[Fact]
	public void Serialize_SInt64_Max() {
		Assert.Equal(new Byte[] {0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f}, LightWeight.Serialize(Int64.MaxValue));
	}

	[Fact]
	public void Serialize_SInt64_Min() {
		Assert.Equal(new Byte[] {0, 0, 0, 0, 0, 0, 0, 0x80}, LightWeight.Serialize(Int64.MinValue));
	}

	[Fact]
	public void Serialize_SInt64_Zero() {
		Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
	}

	[Fact]
	public void Serialize_SInt8_Max() {
		Assert.Equal(new Byte[] {0x7f}, LightWeight.Serialize(SByte.MaxValue));
	}

	[Fact]
	public void Serialize_SInt8_Min() {
		Assert.Equal(new Byte[] {0x80}, LightWeight.Serialize(SByte.MinValue));
	}

	[Fact]
	public void Serialize_SInt8_Zero() {
		Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
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

	[Fact]
	public void Serialize_UInt16_Max() {
		Assert.Equal(new Byte[] {0xff, 0xff}, LightWeight.Serialize(UInt16.MaxValue));
	}

	[Fact]
	public void Serialize_UInt16_Min() {
		Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
	}

	[Fact]
	public void Serialize_UInt32_Max() {
		Assert.Equal(new Byte[] {0xff, 0xff, 0xff, 0xff}, LightWeight.Serialize(UInt32.MaxValue));
	}

	[Fact]
	public void Serialize_UInt32_Min() {
		Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
	}

	[Fact]
	public void Serialize_UInt64_Min() {
		Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
	}

	[Fact]
	public void Serialize_UInt64_UMax() {
		Assert.Equal(new Byte[] {0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff}, LightWeight.Serialize(UInt64.MaxValue));
	}

	[Fact]
	public void Serialize_UInt8_Max() {
		Assert.Equal(new Byte[] {0xff}, LightWeight.Serialize(Byte.MaxValue));
	}

	[Fact]
	public void Serialize_UInt8_Min() {
		Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
	}
}