using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommonSerializer;
using CommonSerializer.MsgPack.Cli;
using CommonSerializer.ProtobufNet;
using InvertedTomato.Serialization.LightWeightSerialization;
using Newtonsoft.Json;

namespace Comparison {
	internal class inputifinpuProgram {
		/* 5-Feb-19
		 *FORMAT      SIZE   SERIALIZE    DESERIALIZE
			JSON:      4,062KB   143ms    84ms
			ProtoBuff: 4,024KB   239ms    84ms
			MsgPack:   3,905KB   109ms    90ms
			LW:        3,918KB   182ms   179ms
		 */

		private static void Main(String[] args) {
			// Open test data (Book => Chapter => Verse => Content)
			var bible = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(File.ReadAllText("esv.json"));


			Byte[] pbOutput;
			var pbSerialize = Stopwatch.StartNew();
			pbOutput = new ProtobufCommonSerializer().SerializeToByteArray(bible);
			pbSerialize.Stop();
			var pbDeserialize = Stopwatch.StartNew();
			var pbResult = new ProtobufCommonSerializer().Deserialize<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(new MemoryStream(pbOutput));
			if (pbResult.Count != bible.Count) {
				Console.WriteLine("ProtoBuff DISQUALIFIED");
			}

			pbDeserialize.Stop();


			Byte[] mpOutput;
			var mpSerialize = Stopwatch.StartNew();
			mpOutput = new MsgPackCommonSerializer().SerializeToByteArray(bible);
			mpSerialize.Stop();
			var mpDeserialize = Stopwatch.StartNew();
			var mpResult = new MsgPackCommonSerializer().Deserialize<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(new MemoryStream(mpOutput));
			if (mpResult.Count != bible.Count) {
				Console.WriteLine("MsgPack DISQUALIFIED");
			}

			mpDeserialize.Stop();

			String nsOutput;
			var nsSerialize = Stopwatch.StartNew();
			nsOutput = JsonConvert.SerializeObject(bible);
			nsSerialize.Stop();
			var nsDeserialize = Stopwatch.StartNew();
			var nsResult = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(nsOutput);
			if (nsResult.Count != bible.Count) {
				Console.WriteLine("Json DISQUALIFIED");
			}

			nsDeserialize.Stop();

			var lw = new LightWeight();
			lw.PrepareFor<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(); // Cheating? Not sure.
			var lwOutput = new MemoryStream();
			var lwSerialize = Stopwatch.StartNew();
			lw.Encode(bible, lwOutput);
			lwSerialize.Stop();
			lwOutput.Seek(0, SeekOrigin.Begin);
			var lwDeserialize = Stopwatch.StartNew();
			var lwResult = lw.Decode<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(lwOutput);
			if (lwResult.Count != bible.Count) {
				Console.WriteLine("LightWeight DISQUALIFIED");
			}

			lwSerialize.Stop();

			Console.WriteLine("FORMAT      SIZE   SERIALIZE    DESERIALIZE");
			Console.WriteLine("JSON:      {0,5:N0}KB {1,5:N0}ms {2,5:N0}ms", nsOutput.Length / 1024, nsSerialize.ElapsedMilliseconds, nsDeserialize.ElapsedMilliseconds);
			Console.WriteLine("ProtoBuff: {0,5:N0}KB {1,5:N0}ms {2,5:N0}ms", pbOutput.Length / 1024, pbSerialize.ElapsedMilliseconds, pbDeserialize.ElapsedMilliseconds);
			Console.WriteLine("MsgPack:   {0,5:N0}KB {1,5:N0}ms {2,5:N0}ms", mpOutput.Length / 1024, mpSerialize.ElapsedMilliseconds, mpDeserialize.ElapsedMilliseconds);
			Console.WriteLine("LW:        {0,5:N0}KB {1,5:N0}ms {2,5:N0}ms", lwOutput.Length / 1024, lwSerialize.ElapsedMilliseconds, lwDeserialize.ElapsedMilliseconds);

			Console.WriteLine("Done.");
			Console.ReadKey(true);
		}
	}

	public static class Extensions {
		public static Byte[] SerializeToByteArray<T>(this ICommonSerializer target, T value) {
			using (var stream = new MemoryStream()) {
				target.Serialize(stream, value);
				return stream.ToArray();
			}
		}
	}
}