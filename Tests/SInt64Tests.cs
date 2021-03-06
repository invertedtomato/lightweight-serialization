using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests
{
    public class SInt64Tests
    {
        enum TestEnum : Int64
        {
            A,
            B
        }

        [Fact]
        public void SInt64SerializeMin()
        {
            Assert.Equal(SignedVlq.Encode(SignedVlq.MinValue), LightWeight.Serialize(SignedVlq.MinValue));
        }

        [Fact]
        public void SInt64SerializeZero()
        {
            Assert.Equal(SignedVlq.Encode(0), LightWeight.Serialize(0));
        }

        [Fact]
        public void SInt64SerializeMax()
        {
            Assert.Equal(SignedVlq.Encode(SignedVlq.MaxValue), LightWeight.Serialize(SignedVlq.MaxValue));
        }

        [Fact]
        public void SInt64SerializeEnum()
        {
            Assert.Equal(SignedVlq.Encode((Int64)TestEnum.B), LightWeight.Serialize(TestEnum.B));
        }


        [Fact]
        public void SInt64DeserializeMin()
        {
            Assert.Equal(SignedVlq.MinValue, LightWeight.Deserialize<Int64>(SignedVlq.Encode(SignedVlq.MinValue).ToArray()));
        }

        [Fact]
        public void SInt64DeserializeZero()
        {
            Assert.Equal((Int64)0, LightWeight.Deserialize<Int64>(SignedVlq.Encode(0).ToArray()));
        }

        [Fact]
        public void SInt64Deserialize10()
        {
            Assert.Equal((Int64)10, LightWeight.Deserialize<Int64>(SignedVlq.Encode(10).ToArray()));
        }

        [Fact]
        public void SInt64DeserializeMax()
        {
            Assert.Equal(SignedVlq.MaxValue, LightWeight.Deserialize<Int64>(SignedVlq.Encode(SignedVlq.MaxValue).ToArray()));
        }

        [Fact]
        public void SInt64DeserializeEnum()
        {
            Assert.Equal(TestEnum.B, LightWeight.Deserialize<TestEnum>(SignedVlq.Encode((Int64)TestEnum.B).ToArray()));
        }
    }
}