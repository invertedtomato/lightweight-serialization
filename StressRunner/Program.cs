using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using InvertedTomato.IO.Buffers;
using InvertedTomato.Serialization.LightWeightSerialization;
using Newtonsoft.Json;

namespace StressTest {
	class Program {
		private static void Main(String[] args) {
			// Open test data (Book => Chapter => Verse => Content)
			var bible = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(File.ReadAllText("esv.json"));

			// Setup output
			var output = new MemoryStream();

			// Start timer
			var timer = Stopwatch.StartNew();

			// Start LightWeight
			var lw = new LightWeight();	

			for (var i = 0; i < 25; i++) {
				// Encode
				var length = lw.Encode(bible, output);

				// Rewind buffer
				output.Seek(0, SeekOrigin.Begin);

				// Decode
				lw.Decode<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(output, length);
			}

			Console.WriteLine(timer.ElapsedMilliseconds);
			Console.ReadKey(true);
		}
	}
}