using CommonSerializer;
using CommonSerializer.MsgPack.Cli;
using CommonSerializer.ProtobufNet;
using InvertedTomato.IO.Buffers;
using InvertedTomato.Serialization.LightWeightSerialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Comparison {
    class Program {
        /* After C'tof input #1
         *FORMAT      SIZE   SERIALIZE    DESERIALIZE
            JSON:      4,062KB    82ms    34ms
            ProtoBuff: 4,024KB   531ms   161ms
            MsgPack:   3,905KB   490ms   144ms
            LW:        3,918KB   206ms   194ms
         */

        /* After adding ScatterBuffer
         *FORMAT      SIZE   SERIALIZE    DESERIALIZE
            JSON:      4,062KB    75ms    30ms
            ProtoBuff: 4,024KB   417ms    97ms
            MsgPack:   3,905KB   258ms    82ms
            LW:        3,918KB   176ms    -
         */

        static void Main(string[] args) {
            // Open test data (Book => Chapter => Verse => Content)
            var bible = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(File.ReadAllText("esv.json"));


            byte[] pbOutput;
            var pbSerialize = Stopwatch.StartNew();
            pbOutput = (new ProtobufCommonSerializer()).SerializeToByteArray(bible);
            pbSerialize.Stop();
            var pbDeserialize = Stopwatch.StartNew();
            var pbResult = (new ProtobufCommonSerializer()).Deserialize<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(new MemoryStream(pbOutput));
            if (pbResult.Count != bible.Count) {
                Console.WriteLine("ProtoBuff DISQUALIFIED");
            }
            pbDeserialize.Stop();


            byte[] mpOutput;
            var mpSerialize = Stopwatch.StartNew();
            mpOutput = (new MsgPackCommonSerializer()).SerializeToByteArray(bible);
            mpSerialize.Stop();
            var mpDeserialize = Stopwatch.StartNew();
            var mpResult = (new MsgPackCommonSerializer()).Deserialize<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(new MemoryStream(mpOutput));
            if (mpResult.Count != bible.Count) {
                Console.WriteLine("MsgPack DISQUALIFIED");
            }
            mpDeserialize.Stop();

            string nsOutput;
            var nsSerialize = Stopwatch.StartNew();
            nsOutput = JsonConvert.SerializeObject(bible);
            nsSerialize.Stop();
            var nsDeserialize = Stopwatch.StartNew();
            var nsResult = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(nsOutput);
            if (nsResult.Count != bible.Count) {
                Console.WriteLine("Json DISQUALIFIED");
            }
            nsDeserialize.Stop();

            var lw = new LightWeight(new LightWeightOptions());
            lw.PrepareFor<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(); // Cheating? Not sure.
            Buffer<byte> lwOutput = new Buffer<byte>(100);
            lwOutput.AutoGrow = true;
            var lwSerialize = Stopwatch.StartNew();
            lw.Serialize(bible, lwOutput);
            lwSerialize.Stop();
            var lwDeserialize = Stopwatch.StartNew();
            var lwResult = bible;// lw.Deserialize<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(lwOutput);
            //////////////////!!!!
            if (lwResult.Count != bible.Count) {
                Console.WriteLine("LightWeight DISQUALIFIED");
            }
            lwSerialize.Stop();

            Console.WriteLine("FORMAT      SIZE   SERIALIZE    DESERIALIZE");
            Console.WriteLine("JSON:      {0,5:N0}KB {1,5:N0}ms {2,5:N0}ms", nsOutput.Length / 1024, nsSerialize.ElapsedMilliseconds, nsDeserialize.ElapsedMilliseconds);
            Console.WriteLine("ProtoBuff: {0,5:N0}KB {1,5:N0}ms {2,5:N0}ms", pbOutput.Length / 1024, pbSerialize.ElapsedMilliseconds, pbDeserialize.ElapsedMilliseconds);
            Console.WriteLine("MsgPack:   {0,5:N0}KB {1,5:N0}ms {2,5:N0}ms", mpOutput.Length / 1024, mpSerialize.ElapsedMilliseconds, mpDeserialize.ElapsedMilliseconds);
            Console.WriteLine("LW:        {0,5:N0}KB {1,5:N0}ms {2,5:N0}ms", lwOutput.End / 1024, lwSerialize.ElapsedMilliseconds, lwDeserialize.ElapsedMilliseconds);

            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }


    }

    public static class Extensions {
        public static byte[] SerializeToByteArray<T>(this ICommonSerializer target, T value) {
            using (var stream = new MemoryStream()) {
                target.Serialize(stream, value);
                return stream.ToArray();
            }
        }
    }
}