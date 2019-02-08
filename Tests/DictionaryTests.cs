using System;
using System.Collections.Generic;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class DictionaryTests {
		[Fact]
		public void DictionarySerializeNull() {
			var encoded = LightWeight.Serialize<Dictionary<Int32, String>>(null);

			Assert.Equal(new Byte[] {
				0x00 // HEADER NULL
			}, encoded);
		}

		[Fact]
		public void DictionarySerializeEmpty() {
			var encoded = LightWeight.Serialize(new Dictionary<Int32, String>());

			Assert.Equal(new Byte[] {
				0x01 // HEADER Count=0
			}, encoded);
		}

		[Fact]
		public void DictionarySerializeInt32String() {
			var encoded = LightWeight.Serialize(new Dictionary<Int32, String>() {
				{1, "a"},
				{2, "b"}
			});

			Assert.Equal(new Byte[] {
				0x03, // HEADER Count=2
				0x01, // 1=
				0x01, (Byte) 'a', // "a"
				0x02, // 2=
				0x01, (Byte) 'b'
			}, encoded);
		}

		[Fact]
		public void DictionarySerializeNested() {
			var serialized = LightWeight.Serialize(new Dictionary<Int32, Dictionary<Int32, String>> {
				{
					1, new Dictionary<Int32, String> {
						{2, "cake"},
						{3, "food"}
					}
				}, {
					8, new Dictionary<Int32, String> {
						{4, "fork"},
						{5, "food"}
					}
				}
			});

			Assert.Equal(new Byte[] {
				0x03, // HEADER Count=2
				0x01, // 1=
				0x03, //     HEADER Count=2
				0x02, //     2=
				0x05, (Byte) 'c', (Byte) 'a', (Byte) 'k', (Byte) 'e', // cake
				0x03, //     3=
				0x05, (Byte) 'f', (Byte) 'o', (Byte) 'o', (Byte) 'd', // food
				0x02, // 2=
				0x03, //     HEADER Count=2
				0x04, //     4=
				0x05, (Byte) 'f', (Byte) 'o', (Byte) 'r', (Byte) 'k', // fork
				0x05, //     5=
				0x05, (Byte) 'f', (Byte) 'o', (Byte) 'o', (Byte) 'd', // food
			}.ToHexString(), serialized.ToHexString());
		}


		[Fact]
		public void DictionaryDeserializeNull() {
			var decoded = LightWeight.Deserialize<Dictionary<Int32, String>>(new Byte[] {
				0x00 // HEADER NULL
			});

			Assert.Null(decoded);
		}

		[Fact]
		public void DictionaryDeserializeEmpty() {
			var decoded = LightWeight.Deserialize<Dictionary<Int32, String>>(new Byte[] {
				0x01 // HEADER Count=0
			});

			Assert.Equal(new Dictionary<Int32, String>(), decoded);
		}

		[Fact]
		public void DictionaryDeserializeInt32String() {
			var encoded = LightWeight.Deserialize<Dictionary<Int32, String>>(new Byte[] {
				0x03, // HEADER Count=2
				0x01, // 1=
				0x01, (Byte) 'a', // "a"
				0x02, // 2=
				0x01, (Byte) 'b'
			});

			Assert.Equal(new Dictionary<Int32, String>() {
				{1, "a"},
				{2, "b"}
			}, encoded);
		}

		[Fact]
		public void DictionaryDeserializeNested() {
			var decoded = LightWeight.Deserialize<Dictionary<Int32, Dictionary<Int32, String>>>(new Byte[] {
				0x03, // HEADER Count=2
				0x01, // 1=
				0x03, //     HEADER Count=2
				0x02, //     2=
				0x05, (Byte) 'c', (Byte) 'a', (Byte) 'k', (Byte) 'e', // cake
				0x03, //     3=
				0x05, (Byte) 'f', (Byte) 'o', (Byte) 'o', (Byte) 'd', // food
				0x02, // 2=
				0x03, //     HEADER Count=2
				0x04, //     4=
				0x05, (Byte) 'f', (Byte) 'o', (Byte) 'r', (Byte) 'k', // fork
				0x05, //     5=
				0x05, (Byte) 'f', (Byte) 'o', (Byte) 'o', (Byte) 'd', // food
			});

			Assert.Equal(new Dictionary<Int32, Dictionary<Int32, String>> {
				{
					1, new Dictionary<Int32, String> {
						{2, "cake"},
						{3, "food"}
					}
				}, {
					8, new Dictionary<Int32, String> {
						{4, "fork"},
						{5, "food"}
					}
				}
			}, decoded);
		}
	}
}