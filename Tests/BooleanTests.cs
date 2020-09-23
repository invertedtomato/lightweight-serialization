using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests
{
    public class BooleanTests
    {
        [Fact]
        public void BooleanDeserializeFalse()
        {
            Assert.False(LightWeight.Deserialize<Boolean>(UnsignedVlq.Encode(0).ToArray()));
        }

        [Fact]
        public void BooleanDeserializeTrue()
        {
            Assert.True(LightWeight.Deserialize<Boolean>(UnsignedVlq.Encode(1).ToArray()));
        }

        [Fact]
        public void BooleanSerializeFalse()
        {
            Assert.Equal(UnsignedVlq.Encode(0), LightWeight.Serialize(false));
        }

        [Fact]
        public void BooleanSerializeTrue()
        {
            Assert.Equal(UnsignedVlq.Encode(1), LightWeight.Serialize(true));
        }
    }
}