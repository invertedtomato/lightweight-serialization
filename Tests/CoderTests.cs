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
}