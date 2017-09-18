using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using InvertedTomato.Serialization.LightWeightSerialization;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

public class SerializationOutputTests {
    [Fact]
    public void AddArray() {
        var o = new SerializationOutput();
        o.AddArray(new byte[] { 1, 2, 3 });
        o.AddArray(new byte[] { 4, 5 });
        Assert.Equal(7, o.Length);
        Assert.Equal(new byte[] { VLQCodec.Nil + 3, 1, 2, 3, VLQCodec.Nil + 2, 4, 5 }, o.ToArray());
    }

    [Fact]
    public void AddRawArray() {
        var o = new SerializationOutput();
        o.AddRawArray(new byte[] { 1, 2, 3 });
        o.AddRawArray(new byte[] { 4, 5 });
        Assert.Equal(5, o.Length);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5 }, o.ToArray());
    }

    [Fact]
    public void AddRaw() {
        var o = new SerializationOutput();
        o.AddRaw(1);
        o.AddRaw(2);
        o.AddRaw(3);
        Assert.Equal(3, o.Length);
        Assert.Equal(new byte[] { 1, 2, 3 }, o.ToArray());
    }

    [Fact]
    public void Allocate() {
        var o = new SerializationOutput();
        var allocateId = o.Allocate();
        Assert.Throws<InvalidOperationException>(() => {
            o.Generate(new Buffer<byte>(0));
        });
    }

    [Fact]
    public void SetVLQ() {
        var o = new SerializationOutput();
        var allocateId = o.Allocate();
        o.SetVLQ(allocateId, 5);
        Assert.Equal(new byte[] { VLQCodec.Nil + 5 }, o.ToArray());
    }

    [Fact]
    public void Generate() {
        var o = new SerializationOutput();
        o.AddArray(new byte[] { 1, 2, 3 });

        var buffer = new Buffer<byte>(10);
        o.Generate(buffer);
        Assert.Equal(new byte[] { 1, 2, 3 }, buffer.ToArray());
    }
}
