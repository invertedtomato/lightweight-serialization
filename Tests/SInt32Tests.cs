using System;
using System.IO;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests
{
    public class SInt32Tests
    {
        enum TestEnum
        {
            A,
            B
        }

        [Fact]
        public void SInt32SerializeMin()
        {
            Assert.Equal(SignedVlq.Encode(Int32.MinValue), LightWeight.Serialize(Int32.MinValue));
        }

        [Fact]
        public void SInt32SerializeZero()
        {
            Assert.Equal(SignedVlq.Encode(0), LightWeight.Serialize(0));
        }

        [Fact]
        public void SInt32SerializeMax()
        {
            Assert.Equal(SignedVlq.Encode(Int32.MaxValue), LightWeight.Serialize(Int32.MaxValue));
        }

        [Fact]
        public void SInt32SerializeEnum()
        {
            Assert.Equal(SignedVlq.Encode((Int32)TestEnum.B), LightWeight.Serialize(TestEnum.B));
        }


        [Fact]
        public void SInt32DeserializeMin()
        {
            Assert.Equal(Int32.MinValue, LightWeight.Deserialize<Int32>(SignedVlq.Encode(Int32.MinValue).ToArray()));
        }

        [Fact]
        public void SInt32DeserializeZero()
        {
            Assert.Equal((Int32)0, LightWeight.Deserialize<Int32>(SignedVlq.Encode(0).ToArray()));
        }

        [Fact]
        public void SInt32DeserializeMax()
        {
            Assert.Equal(Int32.MaxValue, LightWeight.Deserialize<Int32>(SignedVlq.Encode(Int32.MaxValue).ToArray()));
        }

        [Fact]
        public void SInt32DeserializeEnum()
        {
            Assert.Equal(TestEnum.B, LightWeight.Deserialize<TestEnum>(SignedVlq.Encode((Int32)TestEnum.B).ToArray()));
        }


        [Fact]
        public void SerializeDeserialize_Int32_Neg1000_1000()
        {
            var lw = new LightWeight();

            for (var input = -10000; input < 10000; input++)
            {
                var encoded = lw.Encode(input);

                var output = lw.Decode<Int32>(encoded);
                Assert.Equal(input, output);
            }
        }
    }
}