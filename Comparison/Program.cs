using InvertedTomato.LightWeightSerialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Comparison {
    class Program {
        public const ushort TOTAL_VERSES=31102;

        static void Main(string[] args) {
            var bookNames = new string[] {
                "Genesis","Exodus","Leviticus","Numbers","Deuteronomy","Joshua","Judges","Ruth","1 Samuel","2 Samuel","1 Kings","2 Kings","1 Chronicles","2 Chronicles","Ezra","Nehemiah","Esther","Job","Psalms","Proverbs","Ecclesiastes","Song of Solomon","Isaiah","Jeremiah","Lamentations","Ezekiel","Daniel","Hosea","Joel","Amos","Obadiah","Jonah","Micah","Nahum","Habakkuk","Zephaniah","Haggai","Zechariah","Malachi","Matthew","Mark","Luke","John","Acts","Romans","1 Corinthians","2 Corinthians","Galatians","Ephesians","Philippians","Colossians","1 Thessalonians","2 Thessalonians","1 Timothy","2 Timothy","Titus","Philemon","Hebrews","James","1 Peter","2 Peter","1 John","2 John","3 John","Jude","Revelation"
            };

            // Read bible
            // Book => Chapter => Verse => Content
            var inputBible = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, Dictionary<int, string>>>>(File.ReadAllText("esv.json"));

            // Open map file
            var bible = new Segment[TOTAL_VERSES]; // TODO: Should be List<T>

            ushort id = 0;
            using (var mapFile = File.CreateText("map.csv")) {
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
                            bible[id] = (new Segment() {
                                Mode = 1,
                                Content = verse
                            });

                            id++;
                        }
                    }
                }
            }

            File.WriteAllText("output.json", JsonConvert.SerializeObject(bible));
            File.WriteAllBytes("output.lw", LightWeight.Serialize(bible));

            /*

            var lw = Serializer.Serialize(bible);

            Console.WriteLine("JSON: " + json.Length);
            Console.WriteLine("LW:   " + lw.Length);
            */

            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }


    }
}