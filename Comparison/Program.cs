using CommonSerializer;
using CommonSerializer.Jil;
using CommonSerializer.MsgPack.Cli;
using CommonSerializer.ProtobufNet;
using InvertedTomato.LightWeightSerialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Comparison {
    class Program {
        public const ushort TOTAL_VERSES = 31102;

        static void Main(string[] args) {
            var bookNames = new string[] {
                "Genesis","Exodus","Leviticus","Numbers","Deuteronomy","Joshua","Judges","Ruth","1 Samuel","2 Samuel","1 Kings","2 Kings","1 Chronicles","2 Chronicles","Ezra","Nehemiah","Esther","Job","Psalms","Proverbs","Ecclesiastes","Song of Solomon","Isaiah","Jeremiah","Lamentations","Ezekiel","Daniel","Hosea","Joel","Amos","Obadiah","Jonah","Micah","Nahum","Habakkuk","Zephaniah","Haggai","Zechariah","Malachi","Matthew","Mark","Luke","John","Acts","Romans","1 Corinthians","2 Corinthians","Galatians","Ephesians","Philippians","Colossians","1 Thessalonians","2 Thessalonians","1 Timothy","2 Timothy","Titus","Philemon","Hebrews","James","1 Peter","2 Peter","1 John","2 John","3 John","Jude","Revelation"
            };

            // Read bible
            // Book => Chapter => Verse => Content
            var inputBible = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(File.ReadAllText("esv.json"));

            // Open map file
            var bible = new List<Segment>();

            ushort id = 0;
            using (var mapFile = File.CreateText("output.map")) {
                mapFile.AutoFlush = true;
                mapFile.WriteLine("id,book,chapter,verse");

                // Process 
                foreach (var bookName in bookNames) {
                    var book = inputBible[bookName];

                    for (var chapterIdx = 1; chapterIdx < ushort.MaxValue; chapterIdx++) {
                        if (!book.TryGetValue(chapterIdx, out var chapter)) {
                            break;
                        }

                        for (var verseIdx = 1; verseIdx < ushort.MaxValue; verseIdx++) {
                            if (!chapter.TryGetValue(verseIdx, out var verse)) {
                                break;
                            }

                            // Write map
                            mapFile.Write(id);
                            mapFile.Write(",");
                            mapFile.Write(bookName.ToUpperInvariant());
                            mapFile.Write(",");
                            mapFile.Write(chapterIdx);
                            mapFile.Write(",");
                            mapFile.WriteLine(verseIdx);

                            // Compose bible
                            bible.Add(new Segment() {
                                Mode = 0,
                                Content = verse
                            });

                            id++;
                        }
                    }
                }
            }

            var protoBuffTimer = Stopwatch.StartNew();
            File.WriteAllBytes("output.buff", (new ProtobufCommonSerializer()).SerializeToByteArray(bible));
            protoBuffTimer.Stop();

            var msgPackTimer = Stopwatch.StartNew();
            File.WriteAllBytes("output.msg", (new MsgPackCommonSerializer()).SerializeToByteArray(bible));
            msgPackTimer.Stop();

            var jilTimer = Stopwatch.StartNew();
            File.WriteAllBytes("output.jil", (new JilCommonSerializer()).SerializeToByteArray(bible));
            jilTimer.Stop();

            var jsonTimer = Stopwatch.StartNew();
            File.WriteAllText("output.json", JsonConvert.SerializeObject(bible));
            jsonTimer.Stop();

            var lwTimer = Stopwatch.StartNew();
            File.WriteAllBytes("output.lw", LightWeight.Serialize(bible));
            lwTimer.Stop();

            Console.WriteLine("Jil:       {0,5:N0}KB {1,5:N0}ms", File.OpenRead("output.jil").Length / 1024, jilTimer.ElapsedMilliseconds);
            Console.WriteLine("JSON:      {0,5:N0}KB {1,5:N0}ms", File.OpenRead("output.json").Length / 1024, jsonTimer.ElapsedMilliseconds);
            Console.WriteLine("ProtoBuff: {0,5:N0}KB {1,5:N0}ms", File.OpenRead("output.buff").Length / 1024, protoBuffTimer.ElapsedMilliseconds);
            Console.WriteLine("MsgPack:   {0,5:N0}KB {1,5:N0}ms", File.OpenRead("output.msg").Length / 1024, msgPackTimer.ElapsedMilliseconds);
            Console.WriteLine("LW:        {0,5:N0}KB {1,5:N0}ms", File.OpenRead("output.lw").Length / 1024, lwTimer.ElapsedMilliseconds);

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