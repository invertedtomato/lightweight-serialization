using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Coders;
using Xunit;

public class CoderTests {
    [Fact]
    public void Serialize_Bool_False() {
        Assert.Equal(new byte[] { }, BoolCoder.Serialize(false).Payload);
    }
    [Fact]
    public void Serialize_Bool_True() {
        Assert.Equal(new byte[] { 0x00 }, BoolCoder.Serialize(true).Payload);
    }

    [Fact]
    public void Serialize_SInt8_Min() {
        Assert.Equal(new byte[] { 0x80 }, SInt8Coder.Serialize(sbyte.MinValue).Payload);
    }
    [Fact]
    public void Serialize_SInt8_Zero() {
        Assert.Equal(new byte[] { }, SInt8Coder.Serialize(0).Payload);
    }
    [Fact]
    public void Serialize_SInt8_Max() {
        Assert.Equal(new byte[] { 0x7f }, SInt8Coder.Serialize(sbyte.MaxValue).Payload);
    }
    [Fact]
    public void Serialize_SInt16_Min() {
        Assert.Equal(new byte[] { 0, 0x80 }, SInt16Coder.Serialize(short.MinValue).Payload);
    }
    [Fact]
    public void Serialize_SInt16_Zero() {
        Assert.Equal(new byte[] { }, SInt16Coder.Serialize(0).Payload);
    }
    [Fact]
    public void Serialize_SInt16_Max() {
        Assert.Equal(new byte[] { 0xff, 0x7f }, SInt16Coder.Serialize(short.MaxValue).Payload);
    }
    [Fact]
    public void Serialize_SInt32_Min() {
        Assert.Equal(new byte[] { 0, 0, 0, 0x80 }, SInt32Coder.Serialize(int.MinValue).Payload);
    }
    [Fact]
    public void Serialize_SInt32_Zero() {
        Assert.Equal(new byte[] { }, SInt32Coder.Serialize(0).Payload);
    }
    [Fact]
    public void Serialize_SInt32_Max() {
        Assert.Equal(new byte[] { 0xff, 0xff, 0xff, 0x7f }, SInt32Coder.Serialize(int.MaxValue).Payload);
    }
    [Fact]
    public void Serialize_SInt64_Min() {
        Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0x80 }, SInt64Coder.Serialize(long.MinValue).Payload);
    }
    [Fact]
    public void Serialize_SInt64_Zero() {
        Assert.Equal(new byte[] { }, SInt64Coder.Serialize(0).Payload);
    }
    [Fact]
    public void Serialize_SInt64_Max() {
        Assert.Equal(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f }, SInt64Coder.Serialize(long.MaxValue).Payload);
    }

    [Fact]
    public void Serialize_UInt8_Min() {
        Assert.Equal(new byte[] { }, UInt8Coder.Serialize(0).Payload);
    }
    [Fact]
    public void Serialize_UInt8_Max() {
        Assert.Equal(new byte[] { 0xff }, UInt8Coder.Serialize(byte.MaxValue).Payload);
    }
    [Fact]
    public void Serialize_UInt16_Min() {
        Assert.Equal(new byte[] { }, UInt16Coder.Serialize(0).Payload);
    }
    [Fact]
    public void Serialize_UInt16_Max() {
        Assert.Equal(new byte[] { 0xff, 0xff }, UInt16Coder.Serialize(ushort.MaxValue).Payload);
    }
    [Fact]
    public void Serialize_UInt32_Min() {
        Assert.Equal(new byte[] { }, UInt32Coder.Serialize(0).Payload);
    }
    [Fact]
    public void Serialize_UInt32_Max() {
        Assert.Equal(new byte[] { 0xff, 0xff, 0xff, 0xff }, UInt32Coder.Serialize(uint.MaxValue).Payload);
    }
    [Fact]
    public void Serialize_UInt64_Min() {
        Assert.Equal(new byte[] { }, UInt64Coder.Serialize(0).Payload);
    }
    [Fact]
    public void Serialize_UInt64_UMax() {
        Assert.Equal(new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }, UInt64Coder.Serialize(ulong.MaxValue).Payload);
    }


    [Fact]
    public void Serialize_String_Null() {
        Assert.Equal(new byte[] { }, StringCoder.Serialize(null).Payload);
    }
    [Fact]
    public void Serialize_String_Zero() {
        Assert.Equal(new byte[] { }, StringCoder.Serialize(string.Empty).Payload);
    }
    [Fact]
    public void Serialize_String_1() {
        Assert.Equal(new byte[] { (byte)'a' }, StringCoder.Serialize("a").Payload);
    }




    [Fact]
    public void Deserialize_Bool_False() {
        Assert.Equal(false, LightWeight.Deserialize<bool>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_Bool_True() {
        Assert.Equal(true, LightWeight.Deserialize<bool>(new byte[] { 0x00 }));
    }


    [Fact]
    public void Deserialize_SInt8_Min() {
        Assert.Equal(sbyte.MinValue, LightWeight.Deserialize<sbyte>(new byte[] { 128 })); // TODO: check
    }
    [Fact]
    public void Deserialize_SInt8_Zero() {
        Assert.Equal(0, LightWeight.Deserialize<sbyte>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_SInt8_Max() {
        Assert.Equal(sbyte.MaxValue, LightWeight.Deserialize<sbyte>(new byte[] { 127 })); // TODO: check
    }
    [Fact]
    public void Deserialize_SInt16_Min() {
        Assert.Equal(short.MinValue, LightWeight.Deserialize<short>(new byte[] { 0, 128 }));
    }
    [Fact]
    public void Deserialize_SInt16_Zero() {
        Assert.Equal(0, LightWeight.Deserialize<short>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_SInt16_Max() {
        Assert.Equal(short.MaxValue, LightWeight.Deserialize<short>(new byte[] { 255, 127 }));
    }
    [Fact]
    public void Deserialize_SInt32_Min() {
        Assert.Equal(int.MinValue, LightWeight.Deserialize<int>(new byte[] { 0, 0, 0, 128 }));
    }
    [Fact]
    public void Deserialize_SInt32_Zero() {
        Assert.Equal(0, LightWeight.Deserialize<int>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_SInt32_Max() {
        Assert.Equal(int.MaxValue, LightWeight.Deserialize<int>(new byte[] { 255, 255, 255, 127 }));
    }
    [Fact]
    public void Deserialize_SInt64_Min() {
        Assert.Equal(long.MinValue, LightWeight.Deserialize<long>(new byte[] { 0, 0, 0, 0, 0, 0, 0, 128 }));
    }
    [Fact]
    public void Deserialize_SInt64_Zero() {
        Assert.Equal((long)0, LightWeight.Deserialize<long>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_SInt64_Max() {
        Assert.Equal(long.MaxValue, LightWeight.Deserialize<long>(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }));
    }

    [Fact]
    public void Deserialize_UInt8() {
        Assert.Equal(1, LightWeight.Deserialize<byte>(new byte[] { 1 }));
    }
    [Fact]
    public void Deserialize_UInt16() {
        Assert.Equal((ushort)byte.MaxValue + 1, LightWeight.Deserialize<ushort>(new byte[] { 0, 1 }));
    }
    [Fact]
    public void Deserialize_UInt32() {
        Assert.Equal((uint)ushort.MaxValue + 1, LightWeight.Deserialize<uint>(new byte[] { 0, 0, 1, 0 }));
    }
    [Fact]
    public void Deserialize_UInt64() {
        Assert.Equal((ulong)uint.MaxValue + 1, LightWeight.Deserialize<ulong>(new byte[] { 0, 0, 0, 0, 1, 0, 0, 0 }));
    }
    [Fact]
    public void Deserialize_UMax() {
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


}