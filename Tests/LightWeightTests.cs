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
}