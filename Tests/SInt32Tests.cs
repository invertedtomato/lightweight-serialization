using System;
using System.IO;
using InvertedTomato.Serialization.LightWeightSerialization;
using Xunit;

namespace Tests {
	public class SInt32Tests {
		[Fact]
		public void Deserialize_SInt32_255() {
			Assert.Equal(255, LightWeight.Deserialize<Int32>(new Byte[] {0xFF, 0}));
		}


		[Fact]
		public void Deserialize_SInt32_Max() {
			Assert.Equal(Int32.MaxValue, LightWeight.Deserialize<Int32>(new Byte[] {255, 255, 255, 127}));
		}

		[Fact]
		public void Deserialize_SInt32_Min() {
			Assert.Equal(Int32.MinValue, LightWeight.Deserialize<Int32>(new Byte[] {0, 0, 0, 128}));
		}

		[Fact]
		public void Deserialize_SInt32_Zero() {
			Assert.Equal(0, LightWeight.Deserialize<Int32>(new Byte[] { }));
		}

		[Fact]
		public void Serialize_SInt32_255() {
			Assert.Equal(new Byte[] {0xFF, 0x00}, LightWeight.Serialize(255));
		}

		[Fact]
		public void Serialize_SInt32_Max() {
			Assert.Equal(new Byte[] {0xff, 0xff, 0xff, 0x7f}, LightWeight.Serialize(Int32.MaxValue));
		}

		[Fact]
		public void Serialize_SInt32_Min() {
			Assert.Equal(new Byte[] {0, 0, 0, 0x80}, LightWeight.Serialize(Int32.MinValue));
		}

		[Fact]
		public void Serialize_SInt32_Zero() {
			Assert.Equal(new Byte[] { }, LightWeight.Serialize(0));
		}

		[Fact]
		public void SerializeDeserialize_Int32_Neg1000_1000() {
			var lw = new LightWeight();

			using (var buffer = new MemoryStream()) {
				for (var input = -10000; input < 10000; input++) {
					lw.Encode(input, buffer);
					buffer.Seek(0, SeekOrigin.Begin);

					var output = lw.Decode<Int32>(buffer);
					Assert.Equal(input, output);
					buffer.Seek(0, SeekOrigin.Begin);
				}
			}
		}
	}
}