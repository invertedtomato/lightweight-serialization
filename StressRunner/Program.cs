﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using InvertedTomato.IO.Buffers;
using InvertedTomato.Serialization.LightWeightSerialization;
using Newtonsoft.Json;

namespace StressTest {
	class Program {
		private static void Main(String[] args) {
			var runs = 3;
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
					// Setup output
					using (var output = new MemoryStream()) {
						// Encode
						lw.Encode(bible, output);

						// Rewind buffer
						output.Seek(0, SeekOrigin.Begin);

						// Decode
						lw.Decode<Dictionary<String, Dictionary<Int32, Dictionary<Int32, String>>>>(output);
					}
				}

				Console.WriteLine($"{iterations} iterations in {timer.ElapsedMilliseconds}ms");
			}


			// 4-Feb-19 5984ms
		}
	}
}