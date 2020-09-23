using System;
using System.Collections.Generic;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests
{
    public class ListTests
    {
        [Fact]
        public void ListSerializeNull()
        {
            var encoded = LightWeight.Serialize<List<Int32>>(null);

            Assert.Equal(new Byte[] {
                0x00 // HEADER Null
			}, encoded);
        }


        [Fact]
        public void ListSerializeEmpty()
        {
            var encoded = LightWeight.Serialize(new List<Int32>());

            Assert.Equal(new Byte[] {
                0x01 // HEADER Count=0
			}, encoded);
        }

        [Fact]
        public void ListSerializeInt32()
        {
            var encoded = LightWeight.Serialize(new List<UInt32> { 1, 2, 1000 });

            Assert.Equal(new Byte[] {
                0x04, // HEADER Count=3
				0x01, // [0]=1,
				0x02, // [1]=2,
				0xE8, 0x06 // [2]=1000
			}, encoded);
        }

        [Fact]
        public void ListDeserializeNull()
        {
            var result = LightWeight.Deserialize<List<Int32>>(new Byte[] {
                0x00 // HEADER Null
			});

            Assert.Null(result);
        }

        [Fact]
        public void ListDeserializeEmpty()
        {
            var result = LightWeight.Deserialize<List<Int32>>(new Byte[] {
                0x01 // HEADER Count=0
			});

            Assert.Equal(new List<Int32> { }, result);
        }

        [Fact]
        public void ListDeserializeInt32()
        {
            var result = LightWeight.Deserialize<List<UInt32>>(new Byte[] {

                0x04, // HEADER Count=3
				0x01, // [0]=1,
				0x02, // [1]=2,
				0xE8, 0x06// [2]=1000
			});

            Assert.Equal(new List<UInt32> { 1, 2, 1000 }, result);
        }
    }
}