using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Coders;
using Xunit;

public class CoderTests {
    [Fact]
    public void Serialize_Bool_False() {
        var o = new SerializationOutput();
        BoolCoder.Serialize(false, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_Bool_True() {
        var o = new SerializationOutput();
        BoolCoder.Serialize(true, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 1, 0xff }, o.ToArray());
    }

    [Fact]
    public void Serialize_SInt8_Min() {
        var o = new SerializationOutput();
        SInt8Coder.Serialize(sbyte.MinValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 1, 0x80 }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt8_Zero() {
        var o = new SerializationOutput();
        SInt8Coder.Serialize(0, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt8_Max() {
        var o = new SerializationOutput();
        SInt8Coder.Serialize(sbyte.MaxValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 1, 0x7f }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt16_Min() {
        var o = new SerializationOutput();
        SInt16Coder.Serialize(short.MinValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 2, 0, 0x80 }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt16_Zero() {
        var o = new SerializationOutput();
        SInt16Coder.Serialize(0, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt16_Max() {
        var o = new SerializationOutput();
        SInt16Coder.Serialize(short.MaxValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 2, 0xff, 0x7f }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt32_Min() {
        var o = new SerializationOutput();
        SInt32Coder.Serialize(int.MinValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 4, 0, 0, 0, 0x80 }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt32_Zero() {
        var o = new SerializationOutput();
        SInt32Coder.Serialize(0, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt32_Max() {
        var o = new SerializationOutput();
        SInt32Coder.Serialize(int.MaxValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 4, 0xff, 0xff, 0xff, 0x7f }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt64_Min() {
        var o = new SerializationOutput();
        SInt64Coder.Serialize(long.MinValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 8, 0, 0, 0, 0, 0, 0, 0, 0x80 }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt64_Zero() {
        var o = new SerializationOutput();
        SInt64Coder.Serialize(0, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_SInt64_Max() {
        var o = new SerializationOutput();
        SInt64Coder.Serialize(long.MaxValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 8, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0x7f }, o.ToArray());
    }

    [Fact]
    public void Serialize_UInt8_Min() {
        var o = new SerializationOutput();
        UInt8Coder.Serialize(byte.MinValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_UInt8_Max() {
        var o = new SerializationOutput();
        UInt8Coder.Serialize(byte.MaxValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil+1, 0xff }, o.ToArray());
    }
    [Fact]
    public void Serialize_UInt16_Min() {
        var o = new SerializationOutput();
        UInt16Coder.Serialize(ushort.MinValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_UInt16_Max() {
        var o = new SerializationOutput();
        UInt16Coder.Serialize(ushort.MaxValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 2, 0xff ,0xff}, o.ToArray());
    }
    [Fact]
    public void Serialize_UInt32_Min() {
        var o = new SerializationOutput();
        UInt32Coder.Serialize(uint.MinValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_UInt32_Max() {
        var o = new SerializationOutput();
        UInt32Coder.Serialize(uint.MaxValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 4, 0xff, 0xff, 0xff, 0xff }, o.ToArray());
    }
    [Fact]
    public void Serialize_UInt64_Min() {
        var o = new SerializationOutput();
        UInt64Coder.Serialize(ulong.MinValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_UInt64_UMax() {
        var o = new SerializationOutput();
        UInt64Coder.Serialize(ulong.MaxValue, o);
        Assert.Equal(new byte[] { VLQCodec.Nil + 8, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff }, o.ToArray());
    }


    [Fact]
    public void Serialize_String_Null() {
        var o = new SerializationOutput();
        StringCoder.Serialize(null, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_String_Zero() {
        var o = new SerializationOutput();
        StringCoder.Serialize(string.Empty, o);
        Assert.Equal(new byte[] { VLQCodec.Nil }, o.ToArray());
    }
    [Fact]
    public void Serialize_String_1() {
        var o = new SerializationOutput();
        StringCoder.Serialize("a", o);
        Assert.Equal(new byte[] { VLQCodec.Nil+1, (byte)'a' }, o.ToArray());
    }



    
    [Fact]
    public void Deserialize_Bool_False() {
        Assert.Equal(false, LightWeight.Deserialize<bool>(new byte[] { }));
    }
    [Fact]
    public void Deserialize_Bool_True() {
        Assert.Equal(true, LightWeight.Deserialize<bool>(new byte[] { byte.MaxValue }));
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