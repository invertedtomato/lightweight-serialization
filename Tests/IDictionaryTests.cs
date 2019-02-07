using System;
using System.Collections.Generic;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class IDictionaryTests {
		[Fact]
		public void Deserialize_IDict_Empty() {
			var result = LightWeight.Deserialize<Dictionary<Int32, String>>(new Byte[] { });

			Assert.Equal(new Dictionary<Int32, String>(), result);
		}

		[Fact]
		public void Deserialize_IDict_Nested() {
			var result = LightWeight.Deserialize<Dictionary<Int32, Dictionary<Int32, String>>>(new Byte[] {
				0x81, // [K]=
				0x01, //   1
				0x8E, // [V]=
				0x81, //   [K]=
				0x02, //     2
				0x84, //   [V]=
				(Byte) 'c', (Byte) 'a', (Byte) 'k', (Byte) 'e', // cake
				0x81, //   [K]=
				0x03, //     3
				0x84, //   [V]=
				(Byte) 'f', (Byte) 'o', (Byte) 'o', (Byte) 'd', // food
				0x81, // [K]=
				0x08, //   8
				0x8E, // [V]=
				0x81, //   [K]=
				0x04, //     4
				0x84, //   [V]=
				(Byte) 'f', (Byte) 'o', (Byte) 'r', (Byte) 'k', // fork
				0x81, //   [K]=
				0x05, //     5
				0x84, //   [V]=
				(Byte) 'f', (Byte) 'o', (Byte) 'o', (Byte) 'd' // food
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
			}, result);
		}

		[Fact]
		public void Deserialize_IDict_SInt32_String() {
			var result = LightWeight.Deserialize<Dictionary<Int32, String>>(new Byte[] {
				0x81, 0x01, // 1 =
				0x81, (Byte) 'a', // 'a'
				0x81, 0x02, // 2 =
				0x81, (Byte) 'b', // 'b'
				0x81, 0x03, // 3 =
				0x81, (Byte) 'c' // 'c'
			});

			Assert.Equal(new Dictionary<Int32, String> {
				{1, "a"},
				{2, "b"},
				{3, "c"}
			}, result);
		}

		[Fact]
		public void Serialize_IDict_Empty() {
			var serialized = LightWeight.Serialize(new Dictionary<Int32, String>());

			Assert.Equal(new Byte[] { }, serialized);
		}

		[Fact]
		public void Serialize_IDict_Nested() {
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
				0x81, // [K]=
				0x01, //   1
				0x8E, // [V]=
				0x81, //   [K]=
				0x02, //     2
				0x84, //   [V]=
				(Byte) 'c', (Byte) 'a', (Byte) 'k', (Byte) 'e', // cake
				0x81, //   [K]=
				0x03, //     3
				0x84, //   [V]=
				(Byte) 'f', (Byte) 'o', (Byte) 'o', (Byte) 'd', // food
				0x81, // [K]=
				0x08, //   8
				0x8E, // [V]=
				0x81, //   [K]=
				0x04, //     4
				0x84, //   [V]=
				(Byte) 'f', (Byte) 'o', (Byte) 'r', (Byte) 'k', // fork
				0x81, //   [K]=
				0x05, //     5
				0x84, //   [V]=
				(Byte) 'f', (Byte) 'o', (Byte) 'o', (Byte) 'd' // food
			}.ToHexString(), serialized.ToHexString());
		}

		[Fact]
		public void Serialize_IDict_SInt32_String() {
			var serialized = LightWeight.Serialize(new Dictionary<Int32, String> {
				{1, "a"},
				{2, "b"},
				{3, "c"}
			});

			Assert.Equal(new Byte[] {
				0x81, 0x01, // 1 =
				0x81, (Byte) 'a', // 'a'
				0x81, 0x02, // 2 =
				0x81, (Byte) 'b', // 'b'
				0x81, 0x03, // 3 =
				0x81, (Byte) 'c' // 'c'
			}, serialized);
		}
	}
}