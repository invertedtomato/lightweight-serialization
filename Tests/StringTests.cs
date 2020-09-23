using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests
{
    public class StringTests
    {
        [Fact]
        public void StringSerializeNull()
        {
            Assert.Equal(UnsignedVlq.Encode(0), LightWeight.Serialize<String>(null));
        }

        [Fact]
        public void StringSerializeEmpty()
        {
            Assert.Equal(UnsignedVlq.Encode(1), LightWeight.Serialize(String.Empty));
        }

        [Fact]
        public void StringSerialize1()
        {
            Assert.Equal(new[] { (Byte)0x02, (Byte)'a' }, LightWeight.Serialize("a"));
        }

        [Fact]
        public void StringDeserializeNull()
        {
            Assert.Null(LightWeight.Deserialize<String>(UnsignedVlq.Encode(0).ToArray()));
        }

        [Fact]
        public void StringDeserializeEmpty()
        {
            Assert.Equal(String.Empty, LightWeight.Deserialize<String>(UnsignedVlq.Encode(1).ToArray()));
        }

        [Fact]
        public void StringDeserializeOne()
        {
            Assert.Equal("a", LightWeight.Deserialize<String>(new[] { (Byte)0x02, (Byte)'a' }));
        }


    }
}