using InvertedTomato.IO.Buffers;
using InvertedTomato.Serialization.LightWeightSerialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace StressTest {
    class Program {
        static void Main(string[] args) {
            // Open test data (Book => Chapter => Verse => Content)
            var bible = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(File.ReadAllText("esv.json"));

            var timer = Stopwatch.StartNew();

            var lw = new LightWeight(new LightWeightOptions());
            lw.PrepareFor<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(); // Cheating? Not sure.

            Buffer<byte> lwOutput = null;
            for (var i = 0; i < 25; i++) {
                lwOutput = new Buffer<byte>(100);
                lwOutput.AutoGrow = true;
                lw.Serialize(bible, lwOutput);
        
                lw.Deserialize<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(lwOutput);
            }

            Console.WriteLine(timer.ElapsedMilliseconds);
            Console.ReadKey(true);
        }
    }
}
