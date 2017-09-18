using InvertedTomato.Serialization.LightWeightSerialization;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ReadRecord {
    [LightWeightProperty(0)]
    public Dictionary<int, Dictionary<int, string>> Genesis;
    [LightWeightProperty(1)]
    public Dictionary<int, Dictionary<int, string>> Exodus;
    [LightWeightProperty(2)]
    public Dictionary<int, Dictionary<int, string>> Leviticus;
    [LightWeightProperty(3)]
    public Dictionary<int, Dictionary<int, string>> Numbers;
    [LightWeightProperty(4)]
    public Dictionary<int, Dictionary<int, string>> Deuteronomy;
    [LightWeightProperty(5)]
    public Dictionary<int, Dictionary<int, string>> Joshua;
    [LightWeightProperty(6)]
    public Dictionary<int, Dictionary<int, string>> Judges;
    [LightWeightProperty(7)]
    public Dictionary<int, Dictionary<int, string>> Ruth;
    [LightWeightProperty(8)]
    [JsonProperty("1 Samuel")]
    public Dictionary<int, Dictionary<int, string>> Samuel1;
    [LightWeightProperty(9)]
    [JsonProperty("2 Samuel")]
    public Dictionary<int, Dictionary<int, string>> Samuel2;
    [LightWeightProperty(10)]
    [JsonProperty("1 Kings")]
    public Dictionary<int, Dictionary<int, string>> Kings1;
    [LightWeightProperty(11)]
    [JsonProperty("2 Kings")]
    public Dictionary<int, Dictionary<int, string>> Kings2;
    [LightWeightProperty(12)]
    [JsonProperty("1 Chronicles")]
    public Dictionary<int, Dictionary<int, string>> Chronicles1;
    [LightWeightProperty(13)]
    [JsonProperty("2 Chronicles")]
    public Dictionary<int, Dictionary<int, string>> Chronicles2;
    [LightWeightProperty(14)]
    public Dictionary<int, Dictionary<int, string>> Ezra;
    [LightWeightProperty(15)]
    public Dictionary<int, Dictionary<int, string>> Nehemiah;
    [LightWeightProperty(16)]
    public Dictionary<int, Dictionary<int, string>> Esther;
    [LightWeightProperty(17)]
    public Dictionary<int, Dictionary<int, string>> Job;
    [LightWeightProperty(18)]
    public Dictionary<int, Dictionary<int, string>> Psalms;
    [LightWeightProperty(19)]
    public Dictionary<int, Dictionary<int, string>> Proverbs;
    [LightWeightProperty(20)]
    public Dictionary<int, Dictionary<int, string>> Ecclesiastes;
    [LightWeightProperty(21)]
    [JsonProperty("Song of Solomon")]
    public Dictionary<int, Dictionary<int, string>> SongOfSolomon;
    [LightWeightProperty(22)]
    public Dictionary<int, Dictionary<int, string>> Isaiah;
    [LightWeightProperty(23)]
    public Dictionary<int, Dictionary<int, string>> Jeremiah;
    [LightWeightProperty(24)]
    public Dictionary<int, Dictionary<int, string>> Lamentations;
    [LightWeightProperty(25)]
    public Dictionary<int, Dictionary<int, string>> Ezekiel;
    [LightWeightProperty(26)]
    public Dictionary<int, Dictionary<int, string>> Daniel;
    [LightWeightProperty(27)]
    public Dictionary<int, Dictionary<int, string>> Hosea;
    [LightWeightProperty(28)]
    public Dictionary<int, Dictionary<int, string>> Joel;
    [LightWeightProperty(29)]
    public Dictionary<int, Dictionary<int, string>> Amos;
    [LightWeightProperty(30)]
    public Dictionary<int, Dictionary<int, string>> Obadiah;
    [LightWeightProperty(31)]
    public Dictionary<int, Dictionary<int, string>> Jonah;
    [LightWeightProperty(32)]
    public Dictionary<int, Dictionary<int, string>> Micah;
    [LightWeightProperty(33)]
    public Dictionary<int, Dictionary<int, string>> Nahum;
    [LightWeightProperty(34)]
    public Dictionary<int, Dictionary<int, string>> Habakkuk;
    [LightWeightProperty(35)]
    public Dictionary<int, Dictionary<int, string>> Zephaniah;
    [LightWeightProperty(36)]
    public Dictionary<int, Dictionary<int, string>> Haggai;
    [LightWeightProperty(37)]
    public Dictionary<int, Dictionary<int, string>> Zechariah;
    [LightWeightProperty(38)]
    public Dictionary<int, Dictionary<int, string>> Malachi;

    [LightWeightProperty(39)]
    public Dictionary<int, Dictionary<int, string>> Matthew;
    [LightWeightProperty(40)]
    public Dictionary<int, Dictionary<int, string>> Mark;
    [LightWeightProperty(41)]
    public Dictionary<int, Dictionary<int, string>> Luke;
    [LightWeightProperty(42)]
    public Dictionary<int, Dictionary<int, string>> John;
    [LightWeightProperty(43)]
    public Dictionary<int, Dictionary<int, string>> Acts;
    [LightWeightProperty(44)]
    public Dictionary<int, Dictionary<int, string>> Romans;
    [LightWeightProperty(45)]
    [JsonProperty("1 Corinthians")]
    public Dictionary<int, Dictionary<int, string>> Corinthians1;
    [LightWeightProperty(46)]
    [JsonProperty("2 Corinthians")]
    public Dictionary<int, Dictionary<int, string>> Corinthians2;
    [LightWeightProperty(47)]
    public Dictionary<int, Dictionary<int, string>> Galatians;
    [LightWeightProperty(48)]
    public Dictionary<int, Dictionary<int, string>> Ephesians;
    [LightWeightProperty(49)]
    public Dictionary<int, Dictionary<int, string>> Philippians;
    [LightWeightProperty(50)]
    public Dictionary<int, Dictionary<int, string>> Colossians;
    [LightWeightProperty(51)]
    [JsonProperty("1 Thessalonians")]
    public Dictionary<int, Dictionary<int, string>> Thessalonians1;
    [LightWeightProperty(52)]
    [JsonProperty("2 Thessalonians")]
    public Dictionary<int, Dictionary<int, string>> Thessalonians2;
    [LightWeightProperty(53)]
    [JsonProperty("1 Timothy")]
    public Dictionary<int, Dictionary<int, string>> Timothy1;
    [LightWeightProperty(54)]
    [JsonProperty("2 Timothy")]
    public Dictionary<int, Dictionary<int, string>> Timothy2;
    [LightWeightProperty(55)]
    public Dictionary<int, Dictionary<int, string>> Titus;
    [LightWeightProperty(56)]
    public Dictionary<int, Dictionary<int, string>> Philemon;
    [LightWeightProperty(57)]
    public Dictionary<int, Dictionary<int, string>> Hebrews;
    [LightWeightProperty(58)]
    public Dictionary<int, Dictionary<int, string>> James;
    [LightWeightProperty(59)]
    [JsonProperty("1 Peter")]
    public Dictionary<int, Dictionary<int, string>> Peter1;
    [LightWeightProperty(60)]
    [JsonProperty("2 Peter")]
    public Dictionary<int, Dictionary<int, string>> Peter2;
    [LightWeightProperty(61)]
    [JsonProperty("1 John")]
    public Dictionary<int, Dictionary<int, string>> John1;
    [LightWeightProperty(62)]
    [JsonProperty("2 John")]
    public Dictionary<int, Dictionary<int, string>> John2;
    [LightWeightProperty(63)]
    [JsonProperty("3 John")]
    public Dictionary<int, Dictionary<int, string>> John3;
    [LightWeightProperty(64)]
    public Dictionary<int, Dictionary<int, string>> Jude;
    [LightWeightProperty(65)]
    public Dictionary<int, Dictionary<int, string>> Revelation;
}