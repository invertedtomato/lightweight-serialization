using System;
using InvertedTomato.Serialization.LightWeightSerialization;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using Xunit;

namespace Tests {
	public class ClassTests {
		[Fact]
		public void Serialize_POCO() {
			var serialized = LightWeight.Serialize(new ThreeInts {
				A = 1,
				B = 9,
				C = 1000
			});

			Assert.Equal(new Byte[] {
				0x81, // B=
				0x09, //   0
				0x81, // A=
				0x01, //   1
				0x82, // C=
				0xE8, 0x03 // 1000
			}, serialized);
		}

		[Fact]
		public void Serialize_POCO_Nested() {
			var serialized = LightWeight.Serialize(new Layered {
				Y = "test",
				Z = new ThreeInts {
					A = 1,
					B = 9,
					C = 1000
				}
			});

			Assert.Equal(new Byte[] {
				0x84, // Y=
				0x74, 0x65, 0x73, 0x74, // "test"
				0x87, // Z=
				0x81, //   B=
				0x09, //     9
				0x81, //   A=
				0x01, //     1
				0x82, //   C=
				0xE8, 0x03 // 1000
			}.ToHexString(), serialized.ToHexString());
		}

		[Fact]
		public void Serialize_POCO_Empty() {
			var target = new Empty {
				A = 1,
				B = 9,
				C = 1000
			};

			Assert.Equal(new Byte[] { }, LightWeight.Serialize(target));
		}
		[Fact]
		public void Deserialize_POCO_Basic() {
			var result = LightWeight.Deserialize<ThreeInts>(new Byte[] {
				0x81, // B=
				0x09, // 9
				0x81, // A=
				0x01, // 1
				0x82, // C=
				0xE8, 0x03 // 1000
			});

			Assert.Equal(1, result.A);
			Assert.Equal(9, result.B);
			Assert.Equal(1000, result.C);
		}

		[Fact]
		public void Deserialize_POCO_Complex() {
			var result = LightWeight.Deserialize<Layered>(new Byte[] {
				0x84, // Y=
				0x74, 0x65, 0x73, 0x74, // "test"
				0x87, // Z=
				0x81, // B=
				0x09, // 9
				0x81, // A=
				0x01, // 1
				0x82, // C=
				0xE8, 0x03 // 1000
			});

			Assert.Equal("test", result.Y);
			Assert.Equal(1, result.Z.A);
			Assert.Equal(9, result.Z.B);
			Assert.Equal(1000, result.Z.C);
		}

		[Fact]
		public void Deserialize_POCO_Empty() {
			var result = LightWeight.Deserialize<Empty>(new Byte[] { });

			Assert.Equal(0, result.A);
			Assert.Equal(0, result.B);
			Assert.Equal(0, result.C);
		}

		public class ThreeInts {
			[LightWeightProperty(1)] public Int32 A;

			[LightWeightProperty(0)] public Int32 B;

			[LightWeightProperty(2)] public Int32 C;
		}
		public class Layered {
			[LightWeightProperty(0)] public String Y;

			[LightWeightProperty(1)] public ThreeInts Z;
		}
		
		public class Empty {
			public Int32 A { get; set; }
			public Int32 B { get; set; }
			public Int32 C { get; set; }
		}
	}
}