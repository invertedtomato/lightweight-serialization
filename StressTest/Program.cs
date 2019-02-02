using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using InvertedTomato.IO.Buffers;
using InvertedTomato.Serialization.LightWeightSerialization;
using Newtonsoft.Json;

namespace StressTest {
	internal class Program {
		private static void Main(String[] args) {
			// Open test data (Book => Chapter => Verse => Content)
			var bible = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(File.ReadAllText("esv.json"));

			var timer = Stopwatch.StartNew();

			var lw = new LightWeight();
			lw.PrepareFor<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(); // Cheating? Not sure.

			Buffer<Byte> lwOutput = null;
			for (var i = 0; i < 25; i++) {
				lwOutput = new Buffer<Byte>(100);
				lwOutput.AutoGrow = true;
				lw.Encode(bible, lwOutput);

				lw.Decode<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(lwOutput);
			}

			Console.WriteLine(timer.ElapsedMilliseconds);
			Console.ReadKey(true);
		}
	}
}