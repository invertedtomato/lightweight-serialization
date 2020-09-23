using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;
#pragma warning disable 649

namespace Tests
{
    public class ClassTests
    { // TODO: Test deserializing on mismatched classes - upgrading, downgrading and appropriate exceptions on mismatch
        class One
        {
            [LightWeightProperty(1)] public UInt32 A;

            [LightWeightProperty(0)] public UInt32 B;

            [LightWeightProperty(2)] public UInt32 C;
        }

        class Two
        {
            [LightWeightProperty(0)] public String Y;

            [LightWeightProperty(1)] public One Z;
        }

        class SkippedIndex
        {
            [LightWeightProperty(1)] public One Z;
        }
        class DuplicateIndex
        {
            [LightWeightProperty(0)] public One Y;
            [LightWeightProperty(0)] public One Z;
        }

        class NoIndexs
        {
            public UInt32 A { get; set; }
            public UInt32 B { get; set; }
            public UInt32 C { get; set; }
        }

        [Fact]
        public void ClassSerializeSkippedIndex()
        {
            Assert.Throws<MissingIndexException>(() => { LightWeight.Serialize(new SkippedIndex()); });
        }

        [Fact]
        public void ClassSerializeDuplicateIndex()
        {
            Assert.Throws<DuplicateIndexException>(() => { LightWeight.Serialize(new DuplicateIndex()); });
        }

        [Fact]
        public void ClassSerializeNull()
        {
            var encoded = LightWeight.Serialize<One>(null);

            Assert.Equal(new Byte[] {
                 0x00 // HEADER NULL
			 }, encoded);
        }

        [Fact]
        public void ClassSerializeEmpty()
        {
            var encoded = LightWeight.Serialize(new NoIndexs
            {
                A = 1,
                B = 9,
                C = 1000
            });

            Assert.Equal(new Byte[] {
                 0x01 // HEADER Length=0
			 }, encoded);
        }

        [Fact]
        public void ClassSerializeBasic()
        {
            var encoded = LightWeight.Serialize(new One
            {
                A = 1,
                B = 9,
                C = 1000
            });

            Assert.Equal(new Byte[] {
                 0x05, // HEADER Length=4
				 0x09, // [A]=9
				 0x01, // [B]=1
				 0xE8, 0x06, // [C]=1000
			 }, encoded);
        }

        [Fact]
        public void ClassSerializeNested()
        {
            var encoded = LightWeight.Serialize(new Two
            {
                Y = "test",
                Z = new One
                {
                    A = 1,
                    B = 9,
                    C = 1000
                }
            });

            Assert.Equal(new Byte[] {
                 11, // HEADER Length=10
				 5, (Byte)'t', (Byte)'e', (Byte)'s', (Byte)'t', // [A]=test
				 5, //     HEADER Length=4
				 0x09, //     [A]=9
				 0x01,  //     [B]=1
				 0xE8, 0x06 // [C]=1000
			 }, encoded);
        }






        [Fact]
        public void ClassDeserializeSkippedIndex()
        {
            Assert.Throws<MissingIndexException>(() => { LightWeight.Deserialize<SkippedIndex>(new Byte[] { }); });
        }
        [Fact]
        public void ClassDeserializeDuplicateIndex()
        {
            Assert.Throws<DuplicateIndexException>(() => { LightWeight.Deserialize<DuplicateIndex>(new Byte[] { }); });
        }

        [Fact]
        public void ClassDeserializeNull()
        {
            var decoded = LightWeight.Deserialize<One>(new Byte[] {
                 0x00 // HEADER NULL
			 });

            Assert.Null(decoded);
        }


        [Fact]
        public void ClassDeserializeEmpty()
        {
            var encoded = LightWeight.Deserialize<NoIndexs>(new Byte[] {
                 0x01 // HEADER Length=0
			 });

            Assert.Equal((UInt32)0, encoded.A);
            Assert.Equal((UInt32)0, encoded.B);
            Assert.Equal((UInt32)0, encoded.C);
        }

        [Fact]
        public void ClassDeserializeBasic()
        {
            var encoded = LightWeight.Deserialize<One>(new Byte[] {
                 0x05, // HEADER Length=4
				 0x09, // [A]=9
				 0x01, // [B]=1
				 0xE8, 0x06, // [C]=1000
			 });

            Assert.Equal((UInt32)1, encoded.A);
            Assert.Equal((UInt32)1, encoded.A);
            Assert.Equal((UInt32)9, encoded.B);
            Assert.Equal((UInt32)1000, encoded.C);
        }

        [Fact]
        public void ClassDeserializeNested()
        {
            var encoded = LightWeight.Deserialize<Two>(new Byte[] {
                 11, // HEADER Length=10
				 5, (Byte)'t', (Byte)'e', (Byte)'s', (Byte)'t', // [A]=test
				 5, //     HEADER Length=4
				 0x09, //     [A]=9
				 0x01,  //     [B]=1
				 0xE8, 0x06 // [C]=1000
			 });



            Assert.Equal("test", encoded.Y);
            Assert.Equal((UInt32)1, encoded.Z.A);
            Assert.Equal((UInt32)1, encoded.Z.A);
            Assert.Equal((UInt32)9, encoded.Z.B);
            Assert.Equal((UInt32)1000, encoded.Z.C);
        }

    }
}