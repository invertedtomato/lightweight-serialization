using InvertedTomato.IO.Bits;
using InvertedTomato.LightWeightSerialization;
using System;
using Xunit;

public class LightWeightTests {
    [Fact]
    public void Serialize_Boolean_False() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize(false));
    }
    [Fact]
    public void Serialize_Boolean_True() {
        Assert.Equal(new byte[] { byte.MaxValue }, LightWeight.Serialize(true));
    }

    [Fact]
    public void Serialize_Integer_SInt8() {
        Assert.Equal(new byte[] { 1 }, LightWeight.Serialize(1));
    }
    [Fact]
    public void Serialize_Integer_SInt16() {
        Assert.Equal(new byte[] { 0, 1 }, LightWeight.Serialize(byte.MaxValue + 1));
    }
    [Fact]
    public void Serialize_Integer_SInt32() {
        Assert.Equal(new byte[] { 0, 128, 0, 0 }, LightWeight.Serialize(short.MaxValue + 1));
    }
    [Fact]
    public void Serialize_Integer_SInt64() {
        Assert.Equal(new byte[] { 0, 0, 0, 128, 0, 0, 0, 0 }, LightWeight.Serialize((long)int.MaxValue + 1));
    }
    [Fact]
    public void Serialize_Integer_SMax() {
        Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }, LightWeight.Serialize(long.MaxValue));
    }

    [Fact]
    public void Serialize_Integer_UInt8() {
        Assert.Equal(new byte[] { 1 }, LightWeight.Serialize(1));
    }
    [Fact]
    public void Serialize_Integer_UInt16() {
        Assert.Equal(new byte[] { 0, 1 }, LightWeight.Serialize(byte.MaxValue + 1));
    }
    [Fact]
    public void Serialize_Integer_UInt32() {
        Assert.Equal(new byte[] { 0, 0, 1, 0 }, LightWeight.Serialize(ushort.MaxValue + 1));
    }
    [Fact]
    public void Serialize_Integer_UInt64() {
        Assert.Equal(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 }, LightWeight.Serialize((ulong)uint.MaxValue + 1));
    }
    [Fact]
    public void Serialize_Integer_UMax() {
        Assert.Equal(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, LightWeight.Serialize(ulong.MaxValue));
    }

    [Fact]
    public void Serialize_String_0() {
        Assert.Equal(new byte[] { }, LightWeight.Serialize(""));
    }
    [Fact]
    public void Serialize_String_1() {
        Assert.Equal(new byte[] { (byte)'a' }, LightWeight.Serialize("a"));
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
    public void Deserialize_Integer_SInt8() {
        Assert.Equal(1, LightWeight.Deserialize<sbyte>(new byte[] { 1 }));
    }
    [Fact]
    public void Deserialize_Integer_SInt16() {
        Assert.Equal(byte.MaxValue + 1, LightWeight.Deserialize<short>(new byte[] { 0, 1 }));
    }
    [Fact]
    public void Deserialize_Integer_SInt32() {
        Assert.Equal(short.MaxValue + 1, LightWeight.Deserialize<int>(new byte[] { 0, 128, 0, 0 }));
    }
    [Fact]
    public void Deserialize_Integer_SInt64() {
        Assert.Equal((long)int.MaxValue + 1, LightWeight.Deserialize<long>(new byte[] { 0, 0, 0, 128, 0, 0, 0, 0 }));
    }
    [Fact]
    public void Deserialize_Integer_SMax() {
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
    public void Deserialize_String_0() {
        Assert.Equal(string.Empty, LightWeight.Deserialize<string>(new byte[] { })); // TODO: handling of nulls/empties?
    }
    [Fact]
    public void Deserialize_String_1() {
        Assert.Equal("a", LightWeight.Deserialize<string>(new byte[] { (byte)'a' }));
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

        Assert.Equal(9, result.A);
        Assert.Equal(1, result.B);
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