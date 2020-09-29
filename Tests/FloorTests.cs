using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.TransformationAttributes;
using Xunit;

namespace Tests
{
    public class FloorTests // TODO: These tests should be testing the FloorTransform in isloation from the transforms being applied
    {
        class One
        {
            [LightWeightProperty(0)]
            [FloorTransformation(5)]
            public Byte U8;

            [LightWeightProperty(1)]
            [FloorTransformation(5)]
            public UInt16 U16;

            [LightWeightProperty(2)]
            [FloorTransformation(5)]
            public UInt32 U32;

            [LightWeightProperty(3)]
            [FloorTransformation(5)]
            public UInt64 U64;

            [LightWeightProperty(4)]
            [FloorTransformation(5)]
            public SByte S8;

            [LightWeightProperty(5)]
            [FloorTransformation(5)]
            public Int16 S16;

            [LightWeightProperty(6)]
            [FloorTransformation(5)]
            public Int32 S32;

            [LightWeightProperty(7)]
            [FloorTransformation(5)]
            public Int64 S64;
        }

        class Unsupported
        {
            [LightWeightProperty(0)]
            [FloorTransformationAttribute(5)]
            public String ValueString;
        }

        [Fact]
        public void FloorSerializeMin()
        {
            var encoded = LightWeight.Serialize(new One()
            {
                U8 = 5,
                U16 = 5,
                U32 = 5,
                U64 = 5,
                S8 = 5,
                S16 = 5,
                S32 = 5,
                S64 = 5
            });

            Assert.Equal(new Byte[] {
                 0x09, // HEADER Length=8
				 0x00, // [U8]=5
                 0x00, // [U16]=5
                 0x00, // [U32]=5
                 0x00, // [U64]=5
				 0x00, // [S8]=5
                 0x00, // [S16]=5
                 0x00, // [S32]=5
                 0x00, // [S64]=5
			 }.ToHexString(), encoded.ToHexString());
        }

        [Fact]
        public void FloorSerializeMinPlus1()
        {
            var encoded = LightWeight.Serialize(new One()
            {
                U8 = 6,
                U16 = 6,
                U32 = 6,
                U64 = 6,
                S8 = 6,
                S16 = 6,
                S32 = 6,
                S64 = 6
            });

            Assert.Equal(new Byte[] {
                 0x09, // HEADER Length=8
				 0x01, // [U8]=6
                 0x01, // [U16]=6
                 0x01, // [U32]=6
                 0x01, // [U64]=6
				 0x01, // [S8]=6  TODO: need to check this value is correct
                 0x02, // [S16]=6
                 0x02, // [S32]=6
                 0x02, // [S64]=6
			 }.ToHexString(), encoded.ToHexString());
        }

        [Fact]
        public void FloorSerializeUnsupported()
        {
            Assert.Throws<UnsupportedDataTypeException>(() =>
            {
                LightWeight.Serialize(new Unsupported()
                {
                    ValueString = "asdf"
                });
            });
        }


        [Fact]
        public void FloorDeserializeMin()
        {
            var decoded = LightWeight.Deserialize<One>(new Byte[] {
                 0x09, // HEADER Length=8
				 0x00, // [U8]=5
                 0x00, // [U16]=5
                 0x00, // [U32]=5
                 0x00, // [U64]=5
				 0x00, // [S8]=5
                 0x00, // [S16]=5
                 0x00, // [S32]=5
                 0x00, // [S64]=5
			 });

            Assert.Equal((Byte)5, decoded.U8);
            Assert.Equal((UInt16)5, decoded.U16);
            Assert.Equal((UInt32)5, decoded.U32);
            Assert.Equal((UInt64)5, decoded.U64);
            Assert.Equal((SByte)5, decoded.S8);
            Assert.Equal((Int16)5, decoded.S16);
            Assert.Equal((Int32)5, decoded.S32);
            Assert.Equal((Int64)5, decoded.S64);
        }

        [Fact]
        public void FloorDeserializeMinPlus1()
        {
            var decoded = LightWeight.Deserialize<One>(new Byte[] {
                 0x09, // HEADER Length=8
				 0x01, // [U8]=6
                 0x01, // [U16]=6
                 0x01, // [U32]=6
                 0x01, // [U64]=6
				 0x01, // [S8]=6 TODO: need to check this value is correct
                 0x02, // [S16]=6
                 0x02, // [S32]=6
                 0x02, // [S64]=6
			 });

            Assert.Equal((Byte)6, decoded.U8);
            Assert.Equal((UInt16)6, decoded.U16);
            Assert.Equal((UInt32)6, decoded.U32);
            Assert.Equal((UInt64)6, decoded.U64);
            Assert.Equal((SByte)6, decoded.S8);
            Assert.Equal((Int16)6, decoded.S16);
            Assert.Equal((Int32)6, decoded.S32);
            Assert.Equal((Int64)6, decoded.S64);
        }

        [Fact]
        public void FloorDeserializeUnsupported()
        {
            Assert.Throws<UnsupportedDataTypeException>(() =>
            {
                LightWeight.Deserialize<Unsupported>(new Byte[] {
                        0x02, // HEADER Length=1
				        0x01, // [Value]=6
                 });
            });
        }
    }
}