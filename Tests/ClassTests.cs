using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;
#pragma warning disable 649

namespace Tests {
	public class ClassTests {
		class One {
			[LightWeightProperty(1)] public Int32 A;

			[LightWeightProperty(0)] public Int32 B;

			[LightWeightProperty(2)] public Int32 C;
		}

		class Two {
			[LightWeightProperty(0)] public String Y;

			[LightWeightProperty(1)] public One Z;
		}

		class SkippedIndex {
			[LightWeightProperty(1)] public One Z;
		}	
		class DuplicateIndex {
			[LightWeightProperty(0)] public One Y;
			[LightWeightProperty(0)] public One Z;
		}

		 class NoIndexs {
			public Int32 A { get; set; }
			public Int32 B { get; set; }
			public Int32 C { get; set; }
		}

		 [Fact]
		 public void ClassSerializeSkippedIndex() {
			 Assert.Throws<MissingIndexException>(() => { LightWeight.Serialize(new SkippedIndex()); });
		 }
		 
		 [Fact]
		 public void ClassSerializeDuplicateIndex() {
			 Assert.Throws<MissingIndexException>(() => { LightWeight.Serialize(new DuplicateIndex()); });
		 }

		 [Fact]
		 public void ClassSerializeNull() {
			 var encoded = LightWeight.Serialize<One>(null);
			 
			 Assert.Equal(new Byte[] {
				 0x00 // HEADER NULL
			 },encoded);
		 }
		 
		 [Fact]
		 public void ClassSerializeEmpty() {
			 var encoded = LightWeight.Serialize(new NoIndexs {
				 A = 1,
				 B = 9,
				 C = 1000
			 });

			 Assert.Equal(new Byte[] {
				 0x01 // HEADER Length=0
			 }, encoded);
		 }
		 
		 [Fact]
		 public void ClassSerializeBasic() {
			 var encoded = LightWeight.Serialize(new One {
				 A = 1,
				 B = 9,
				 C = 1000
			 });

			 Assert.Equal(new Byte[] {
				 0x05, // HEADER Length=4
				 0x01, // [A]=1
				 0x09, // [B]=9
				 0x00,0x00, // [C]=1000
			 }, encoded);
		 }

		 [Fact]
		 public void ClassSerializeNested() {
			 var encoded = LightWeight.Serialize(new Two {
				 Y = "test",
				 Z = new One {
					 A = 1,
					 B = 9,
					 C = 1000
				 }
			 });

			 Assert.Equal(new Byte[] {
				 0x07, // HEADER Length=6
				 0x04, (Byte)'t', (Byte)'e', (Byte)'s', (Byte)'t', // [A]=test
				 0x00, //     HEADER Length=5
				 0x01,  //     [A]=1
				 0x09, //     [B]=9
				 0x00, 0x00 // [C]=1000
			 }, encoded);
		 }
		 
		 
		 
		 
		 
		 
		 [Fact]
		 public void ClassDeserializeSkippedIndex() {
			 Assert.Throws<MissingIndexException>(() => { LightWeight.Deserialize<SkippedIndex>(new Byte[] { }); });
		 }
		 [Fact]
		 public void ClassDeserializeDuplicateIndex() {
			 Assert.Throws<MissingIndexException>(() => { LightWeight.Deserialize<DuplicateIndex>(new Byte[] { }); });
		 }
		 
		 [Fact]
		 public void ClassDeserializeNull() {
			 var decoded = LightWeight.Deserialize<One>(new Byte[] {
				 0x00 // HEADER NULL
			 });
			 
			 Assert.Null(decoded);
		 }
		 
		 [Fact]
		 public void ClassDeserializeEmpty() {
			 var encoded = LightWeight.Deserialize<NoIndexs>(new Byte[] {
				 0x01 // HEADER Length=0
			 });

			 Assert.Equal(new NoIndexs {
				 A = 1,
				 B = 9,
				 C = 1000
			 }, encoded);
		 }
		 
		 [Fact]
		 public void ClassDeserializeBasic() {
			 var encoded = LightWeight.Deserialize<One>(new Byte[] {
				 0x05, // HEADER Length=4
				 0x01, // [A]=1
				 0x09, // [B]=9
				 0x00,0x00, // [C]=1000
			 });

			 Assert.Equal(new One {
				 A = 1,
				 B = 9,
				 C = 1000
			 }, encoded);
		 }

		 [Fact]
		 public void ClassDeserializeNested() {
			 var encoded = LightWeight.Deserialize<Two>(new Byte[] {
				 0x07, // HEADER Length=6
				 0x04, (Byte)'t', (Byte)'e', (Byte)'s', (Byte)'t', // [A]=test
				 0x00, //     HEADER Length=5
				 0x01,  //     [A]=1
				 0x09, //     [B]=9
				 0x00, 0x00 // [C]=1000
			 });

			 Assert.Equal(new Two {
				 Y = "test",
				 Z = new One {
					 A = 1,
					 B = 9,
					 C = 1000
				 }
			 }, encoded);
		 }
		
	}
}