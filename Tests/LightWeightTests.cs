using InvertedTomato.IO.Bits;
using InvertedTomato.LightWeightSerialization;
using System;
using Xunit;

public class LightWeightTests {
    private readonly LightWeight LightWeight = new LightWeight();

    [Fact]
    public void Serialize_Boolean_False() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize(false));
    }
    [Fact]
    public void Serialize_Boolean_True() {
        Assert.Equal(new byte[] { byte.MaxValue }, LightWeight.Serialize(true));
    }


    [Fact]
    public void Serialize_Integer_SInt8_Min() {
        Assert.Equal(new byte[] { 128 }, LightWeight.Serialize(sbyte.MinValue)); //TODO: Check
    }
    [Fact]
    public void Serialize_Integer_SInt8_Zero() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize((sbyte)0));
    }
    [Fact]
    public void Serialize_Integer_SInt8_Max() {
        Assert.Equal(new byte[] { 127 }, LightWeight.Serialize(sbyte.MaxValue)); //TODO: Check
    }
    [Fact]
    public void Serialize_Integer_SInt16_Min() {
        Assert.Equal(new byte[] { 0, 128 }, LightWeight.Serialize(short.MinValue));
    }
    [Fact]
    public void Serialize_Integer_SInt16_Zero() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize((short)0));
    }
    [Fact]
    public void Serialize_Integer_SInt16_Max() {
        Assert.Equal(new byte[] { 255, 127 }, LightWeight.Serialize(short.MaxValue));
    }
    [Fact]
    public void Serialize_Integer_SInt32_Min() {
        Assert.Equal(new byte[] { 0, 0, 0, 128 }, LightWeight.Serialize(int.MinValue));
    }
    [Fact]
    public void Serialize_Integer_SInt32_Zero() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize((int)0));
    }
    [Fact]
    public void Serialize_Integer_SInt32_Max() {
        Assert.Equal(new byte[] { 255, 255, 255, 127 }, LightWeight.Serialize(int.MaxValue));
    }
    [Fact]
    public void Serialize_Integer_SInt64_Min() {
        Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 0, 0, 128 }, LightWeight.Serialize(long.MinValue));
    }
    [Fact]
    public void Serialize_Integer_SInt64_Zero() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize((long)0));
    }
    [Fact]
    public void Serialize_Integer_SInt64_Max() {
        Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }, LightWeight.Serialize(long.MaxValue));
    }

    [Fact]
    public void Serialize_Integer_UInt8_Min() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize(byte.MinValue));
    }
    [Fact]
    public void Serialize_Integer_UInt8_Max() {
        Assert.Equal(new byte[] { 255 }, LightWeight.Serialize(byte.MaxValue));
    }
    [Fact]
    public void Serialize_Integer_UInt16_Min() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize(ushort.MinValue));
    }
    [Fact]
    public void Serialize_Integer_UInt16_Max() {
        Assert.Equal(new byte[] { 255, 255 }, LightWeight.Serialize(ushort.MaxValue));
    }
    [Fact]
    public void Serialize_Integer_UInt32_Min() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize(uint.MinValue));
    }
    [Fact]
    public void Serialize_Integer_UInt32_Max() {
        Assert.Equal(new byte[] { 255, 255, 255, 255 }, LightWeight.Serialize(uint.MaxValue));
    }
    [Fact]
    public void Serialize_Integer_UInt64_Min() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize(ulong.MinValue));
    }
    [Fact]
    public void Serialize_Integer_UInt64_UMax() {
        Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, LightWeight.Serialize(ulong.MaxValue));
    }

    [Fact]
    public void Serialize_String_Zero() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize(""));
    }
    [Fact]
    public void Serialize_String_1() {
        Assert.Equal(new byte[] { (byte)'a' }, LightWeight.Serialize("a"));
    }

    [Fact]
    public void Serialize_Array_Bool() {
        var serialized = LightWeight.Serialize(new bool[] { true, true, false });

        Assert.Equal(new byte[] {
            0x81, // [0]
                0xff, // true
            0x81, // [1]
                0xff, // true
            0x80 // [2]
                // false
        }, serialized);
    }

    [Fact]
    public void Serialize_Array_SInt8() {
        var serialized = LightWeight.Serialize(new sbyte[] { 1, 2, 3 });

        Assert.Equal(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x81, // [2]
                0x03 // 3
        }, serialized);
    }
    [Fact]
    public void Serialize_Array_SInt16() {
        var serialized = LightWeight.Serialize(new short[] { 1, 2, 1000 });

        Assert.Equal(new byte[] {
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
        var serialized = LightWeight.Serialize(new int[] { 1, 2, 1000 });

        Assert.Equal(new byte[] {
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
        var serialized = LightWeight.Serialize(new long[] { 1, 2, 1000 });

        Assert.Equal(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x82, // [2]
                0xE8, 0x03 // 1000
        }, serialized);
    }

    [Fact]
    public void Serialize_Array_UInt8() {
        var serialized = LightWeight.Serialize(new byte[] { 1, 2, 3 });

        Assert.Equal(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x81, // [2]
                0x03 // 3
        }, serialized);
    }
    [Fact]
    public void Serialize_Array_UInt16() {
        var serialized = LightWeight.Serialize(new ushort[] { 1, 2, 1000 });

        Assert.Equal(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x82, // [2]
                0xE8, 0x03 // 1000
        }, serialized);
    }
    [Fact]
    public void Serialize_Array_UInt32() {
        var serialized = LightWeight.Serialize(new uint[] { 1, 2, 1000 });

        Assert.Equal(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x82, // [2]
                0xE8, 0x03 // 1000
        }, serialized);
    }
    [Fact]
    public void Serialize_Array_UInt64() {
        var serialized = LightWeight.Serialize(new ulong[] { 1, 2, 1000 });

        Assert.Equal(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x82, // [2]
                0xE8, 0x03 // 1000
        }, serialized);
    }

    [Fact]
    public void Serialize_Array_String() {
        var serialized = LightWeight.Serialize(new string[] { "a", "bc", "def" });

        Assert.Equal(new byte[] {
            0x81, // [0]
                (byte)'a',
            0x82, // [1]
                (byte)'b', (byte)'c',
            0x83, // [2]
                (byte)'d',(byte)'e', (byte)'f'
        }, serialized);
    }

    [Fact]
    public void Serialize_Object_Empty() {
        var target = new Empty() {
            A = 1,
            B = 9,
            C = 1000
        };

        Assert.Equal(new byte[] { }, LightWeight.Serialize(target));
    }
    [Fact]
    public void Serialize_Object_Basic() {
        var serialized = LightWeight.Serialize(new ThreeInts() {
            A = 1,
            B = 9,
            C = 1000
        });

        Assert.Equal(new byte[] {
            0x81, // B=
                0x09, // 0
            0x81, // A=
                0x01, // 1
            0x82, // C=
                0xE8, 0x03 // 1000
        }, serialized);
    }
    [Fact]
    public void Serialize_Object_Complex() {
        var serialized = LightWeight.Serialize(new Layered() {
            Y = "test",
            Z = new ThreeInts() {
                A = 1,
                B = 9,
                C = 1000
            }
        });

        Assert.Equal(new byte[] {
            0x84,// Y=
                0x74,0x65,0x73,0x74, // "test"
            0x87, // Z=
                0x81, // B=
                    0x09, // 9
                0x81, // A=
                    0x01, // 1
                0x82, // B=
                    0xE8, 0x03 // 1000
        }, serialized);
    }


    [Fact]
    public void Deserialize_Boolean_False() {
        Assert.Equal(false, LightWeight.Deserialize<bool>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_Boolean_True() {
        Assert.Equal(true, LightWeight.Deserialize<bool>(new byte[] { byte.MaxValue }));
    }


    [Fact]
    public void Deserialize_Integer_SInt8_Min() {
        Assert.Equal(sbyte.MinValue, LightWeight.Deserialize<sbyte>(new byte[] { 128 })); // TODO: check
    }
    [Fact]
    public void Deserialize_Integer_SInt8_Zero() {
        Assert.Equal(0, LightWeight.Deserialize<sbyte>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_Integer_SInt8_Max() {
        Assert.Equal(sbyte.MaxValue, LightWeight.Deserialize<sbyte>(new byte[] { 127 })); // TODO: check
    }
    [Fact]
    public void Deserialize_Integer_SInt16_Min() {
        Assert.Equal(short.MinValue, LightWeight.Deserialize<short>(new byte[] { 0, 128 }));
    }
    [Fact]
    public void Deserialize_Integer_SInt16_Zero() {
        Assert.Equal(0, LightWeight.Deserialize<short>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_Integer_SInt16_Max() {
        Assert.Equal(short.MaxValue, LightWeight.Deserialize<short>(new byte[] { 255, 127 }));
    }
    [Fact]
    public void Deserialize_Integer_SInt32_Min() {
        Assert.Equal(int.MinValue, LightWeight.Deserialize<int>(new byte[] { 0, 0, 0, 128 }));
    }
    [Fact]
    public void Deserialize_Integer_SInt32_Zero() {
        Assert.Equal(0, LightWeight.Deserialize<int>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_Integer_SInt32_Max() {
        Assert.Equal(int.MaxValue, LightWeight.Deserialize<int>(new byte[] { 255, 255, 255, 127 }));
    }
    [Fact]
    public void Deserialize_Integer_SInt64_Min() {
        Assert.Equal(long.MinValue, LightWeight.Deserialize<long>(new byte[] { 0, 0, 0, 0, 0, 0, 0, 128 }));
    }
    [Fact]
    public void Deserialize_Integer_SInt64_Zero() {
        Assert.Equal((long)0, LightWeight.Deserialize<long>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_Integer_SInt64_Max() {
        Assert.Equal(long.MaxValue, LightWeight.Deserialize<long>(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }));
    }

    [Fact]
    public void Deserialize_Integer_UInt8() {
        Assert.Equal(1, LightWeight.Deserialize<byte>(new byte[] { 1 }));
    }
    [Fact]
    public void Deserialize_Integer_UInt16() {
        Assert.Equal((ushort)byte.MaxValue + 1, LightWeight.Deserialize<ushort>(new byte[] { 0, 1 }));
    }
    [Fact]
    public void Deserialize_Integer_UInt32() {
        Assert.Equal((uint)ushort.MaxValue + 1, LightWeight.Deserialize<uint>(new byte[] { 0, 0, 1, 0 }));
    }
    [Fact]
    public void Deserialize_Integer_UInt64() {
        Assert.Equal((ulong)uint.MaxValue + 1, LightWeight.Deserialize<ulong>(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 }));
    }
    [Fact]
    public void Deserialize_Integer_UMax() {
        Assert.Equal(ulong.MaxValue, LightWeight.Deserialize<ulong>(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }));
    }

    [Fact]
    public void Deserialize_String_Zero() {
        Assert.Equal(string.Empty, LightWeight.Deserialize<string>(new byte[] { })); // TODO: handling of nulls/empties?
    }
    [Fact]
    public void Deserialize_String_1() {
        Assert.Equal("a", LightWeight.Deserialize<string>(new byte[] { (byte)'a' }));
    }

    [Fact]
    public void Deserialize_Array_Bool() {
        var result = LightWeight.Deserialize<bool[]>(new byte[] {
            0x81, // [0]
                0xff, // true
            0x81, // [1]
                0xff, // true
            0x80, // [2]
                // false
        });

        Assert.Equal(new bool[] { true, true, false }, result);
    }

    [Fact]
    public void Deserialize_Array_SInt8() {
        var result = LightWeight.Deserialize<sbyte[]>(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x81, // [2]
                0x03 // 3
        });

        Assert.Equal(new sbyte[] { 1, 2, 3 }, result);
    }
    [Fact]
    public void Deserialize_Array_SInt16() {
        var result = LightWeight.Deserialize<short[]>(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x82, // [2]
                0xE8, 0x03 // 1000
        });

        Assert.Equal(new short[] { 1, 2, 1000 }, result);
    }
    [Fact]
    public void Deserialize_Array_SInt32() {
        var result = LightWeight.Deserialize<int[]>(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x82, // [2]
                0xE8, 0x03 // 1000
        });

        Assert.Equal(new int[] { 1, 2, 1000 }, result);
    }
    [Fact]
    public void Deserialize_Array_SInt64() {
        var result = LightWeight.Deserialize<long[]>(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x82, // [2]
                0xE8, 0x03 // 1000
        });

        Assert.Equal(new long[] { 1, 2, 1000 }, result);
    }

    [Fact]
    public void Deserialize_Array_UInt8() {
        var result = LightWeight.Deserialize<byte[]>(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x81, // [2]
                0x03 // 3
        });

        Assert.Equal(new byte[] { 1, 2, 3 }, result);
    }
    [Fact]
    public void Deserialize_Array_UInt16() {
        var result = LightWeight.Deserialize<ushort[]>(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x82, // [2]
                0xE8, 0x03 // 1000
        });

        Assert.Equal(new ushort[] { 1, 2, 1000 }, result);
    }
    [Fact]
    public void Deserialize_Array_UInt32() {
        var result = LightWeight.Deserialize<uint[]>(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x82, // [2]
                0xE8, 0x03 // 1000
        });

        Assert.Equal(new uint[] { 1, 2, 1000 }, result);
    }
    [Fact]
    public void Deserialize_Array_UInt64() {
        var result = LightWeight.Deserialize<ulong[]>(new byte[] {
            0x81, // [0]
                0x01, // 1
            0x81, // [1]
                0x02, // 2
            0x82, // [2]
                0xE8, 0x03 // 1000
        });

        Assert.Equal(new ulong[] { 1, 2, 1000 }, result);
    }

    [Fact]
    public void Deserialize_Array_String() {
        var result = LightWeight.Deserialize<string[]>(new byte[] {
            0x81, // [0]
                (byte)'a',
            0x82, // [1]
                (byte)'b', (byte)'c',
            0x80, // [2]
        });

        Assert.Equal(new string[] { "a", "bc", "" }, result);
    }

    [Fact]
    public void Deserialize_Object_Empty() {
        var result = LightWeight.Deserialize<Empty>(new byte[] { });

        Assert.Equal(0, result.A);
        Assert.Equal(0, result.B);
        Assert.Equal(0, result.C);
    }
    [Fact]
    public void Deserialize_Object_Basic() {
        var result = LightWeight.Deserialize<ThreeInts>(new byte[] {
            0x81,// B=
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
    public void Deserialize_Object_Complex() {
        var result = LightWeight.Deserialize<Layered>(new byte[] {
            0x84, // Y=
                0x74,0x65,0x73,0x74, // "test"
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
}