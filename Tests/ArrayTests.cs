using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class ArrayTests {
		[Fact]
		public void ArraySerializeNull1() {
			var encoded = LightWeight.Serialize<Boolean[]>(null);
			Assert.Equal(new Byte[] {
				0x00 // HEADER NULL
			}, encoded);
		}

		[Fact]
		public void ArraySerializeNull2() {
			var encoded = LightWeight.Serialize(new ArrayNullClass());
			Assert.Equal(new Byte[] {
				0x02, // HEADER Length=1
				0x00 // HEADER NULL
			}, encoded);
		}

		class ArrayNullClass {
			public Int32[] B { get; set; }
		}

		[Fact]
		public void ArraySerializeEmpty() {
			var encoded = LightWeight.Serialize(new Int32[] { });
			Assert.Equal(new Byte[] {
				0x01 // HEADER Count=0
			}, encoded);
		}


		[Fact]
		public void ArraySerializeBoolean() {
			var serialized = LightWeight.Serialize(new[] {true, true, false});

			Assert.Equal(new Byte[] {
				0x04, // HEADER Length=3
				0x01, // [0]=true
				0x01, // [1]=true,
				0x00 // [2]=false
			}, serialized);
		}


		[Fact]
		public void ArraySerializeSInt16() {
			var serialized = LightWeight.Serialize(new Int16[] {1, 2, 1000});

			Assert.Equal(new Byte[] {
				0x04, // HEADER Count=3
				0x01, // [0]=1
				0x02, // [1]=2
				0x00, 0x00 // [2]=1000
			}, serialized);
		}

		[Fact]
		public void ArraySerializeString() {
			var serialized = LightWeight.Serialize(new[] {"a", "bc", "def", "", null});

			Assert.Equal(new Byte[] {
				0x06, // HEADER Length=5
				0x02, (Byte) 'a', // [0]=a
				0x03, (Byte) 'b', (Byte) 'c', // [1]=bc
				0x04, (Byte) 'd', (Byte) 'e', (Byte) 'f', // [2]=def
				0x01, // [3]=
				0x00 // [4]=NULL
			}, serialized);
		}


		[Fact]
		public void ArrayDeserializeNull1() {
			var result = LightWeight.Deserialize<Boolean[]>(new Byte[] {
				0x00 // HEADER NULL
			});

			Assert.Null( result);
		}

		[Fact]
		public void ArrayDeserializeNull2() {
			var result = LightWeight.Deserialize<ArrayNullClass>(new Byte[] {
				0x02, // HEADER Length=1
				0x00 // HEADER NULL
			});
			
			Assert.Null( result.B);
		}

		[Fact]
		public void ArrayDeserializeEmpty() {
			var result = LightWeight.Deserialize<Int32[]>(new Byte[] {
				0x01 // HEADER Count=0
			});
			
			Assert.Equal(new Int32[]{}, result);
		}

		[Fact]
		public void ArrayDeserializeBoolean() {
			var result = LightWeight.Deserialize<Boolean[]>(new Byte[] {
				0x03, // HEADER Count=3
				0x01, // [0]=true
				0x01, // [1]=true
				0x00 // [2]=false
			});

			Assert.Equal(new[] {true, true, false}, result);
		}

		[Fact]
		public void ArrayDeserializeSInt16() {
			var result = LightWeight.Deserialize<Int16[]>(new Byte[] {
				0x03, // HEADER Count=3
				0x01, // [0]=1
				0x02, // [1]=2
				0x00, 0x00 // [3]=??
			});

			Assert.Equal(new Int16[] {1, 2, 1000}, result);
		}
	}
}