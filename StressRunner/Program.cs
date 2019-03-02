using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using InvertedTomato.Serialization.LightWeightSerialization;
using Newtonsoft.Json;

namespace StressTest {
	internal class Program {
		private static void Main(String[] args) {
			var runs = 5;
			var iterations = 25;

			// Open test data (Book => Chapter => Verse => Content)
			var bible = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(File.ReadAllText("esv.json"));

			// Do a number of runs
			for (var run = 0; run < runs; run++) {
				// Start timer
				var timer = Stopwatch.StartNew();

				// Start LightWeight
				var lw = new LightWeight();

				for (var itr = 0; itr < 25; itr++) {
					// Encode
					var output = lw.Encode(bible);

					// Decode
					lw.Decode<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(output);
				}

				Console.WriteLine($"{iterations} iterations in {timer.ElapsedMilliseconds}ms");
			}


			//  4-Feb-19 5984ms
			//  8-Feb-19 7201ms
			//  9-Feb-19 5977ms - Added VLQ encoding cache
			// 28-Feb-19 6023ms - No change
			// 28-Feb-19 5845ms - Swapped to using ArraySegment during encoding
			// 28-Feb-19 5128ms - Moved to struct-based nodes
			//  1-Mar-19 5015ms - Remove list in Node
			//  2-Mar-19 4827ms - Removed Stream for decoding, saving a heap of copying
			//  2-Mar-19 4725ms - Swapped from using Stream for encoding, saving some array resizing.
		}
	}
}