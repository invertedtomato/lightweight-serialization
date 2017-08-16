using InvertedTomato.Serialization.LightWeightSerialization;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ReadRecord {
    [LightWeightProperty(0)]
    public Dictionary<int, Dictionary<int, string>> Genesis { get; set; }
    [LightWeightProperty(1)]
    public Dictionary<int, Dictionary<int, string>> Exodus { get; set; }
    [LightWeightProperty(2)]
    public Dictionary<int, Dictionary<int, string>> Leviticus { get; set; }
    [LightWeightProperty(3)]
    public Dictionary<int, Dictionary<int, string>> Numbers { get; set; }
    [LightWeightProperty(4)]
    public Dictionary<int, Dictionary<int, string>> Deuteronomy { get; set; }
    [LightWeightProperty(5)]
    public Dictionary<int, Dictionary<int, string>> Joshua { get; set; }
    [LightWeightProperty(6)]
    public Dictionary<int, Dictionary<int, string>> Judges { get; set; }
    [LightWeightProperty(7)]
    public Dictionary<int, Dictionary<int, string>> Ruth { get; set; }
    [LightWeightProperty(8)]
    [JsonProperty("1 Samuel")]
    public Dictionary<int, Dictionary<int, string>> Samuel1 { get; set; }
    [LightWeightProperty(9)]
    [JsonProperty("2 Samuel")]
    public Dictionary<int, Dictionary<int, string>> Samuel2 { get; set; }
    [LightWeightProperty(10)]
    [JsonProperty("1 Kings")]
    public Dictionary<int, Dictionary<int, string>> Kings1 { get; set; }
    [LightWeightProperty(11)]
    [JsonProperty("2 Kings")]
    public Dictionary<int, Dictionary<int, string>> Kings2 { get; set; }
    [LightWeightProperty(12)]
    [JsonProperty("1 Chronicles")]
    public Dictionary<int, Dictionary<int, string>> Chronicles1 { get; set; }
    [LightWeightProperty(13)]
    [JsonProperty("2 Chronicles")]
    public Dictionary<int, Dictionary<int, string>> Chronicles2 { get; set; }
    [LightWeightProperty(14)]
    public Dictionary<int, Dictionary<int, string>> Ezra { get; set; }
    [LightWeightProperty(15)]
    public Dictionary<int, Dictionary<int, string>> Nehemiah { get; set; }
    [LightWeightProperty(16)]
    public Dictionary<int, Dictionary<int, string>> Esther { get; set; }
    [LightWeightProperty(17)]
    public Dictionary<int, Dictionary<int, string>> Job { get; set; }
    [LightWeightProperty(18)]
    public Dictionary<int, Dictionary<int, string>> Psalms { get; set; }
    [LightWeightProperty(19)]
    public Dictionary<int, Dictionary<int, string>> Proverbs { get; set; }
    [LightWeightProperty(20)]
    public Dictionary<int, Dictionary<int, string>> Ecclesiastes { get; set; }
    [LightWeightProperty(21)]
    [JsonProperty("Song of Solomon")]
    public Dictionary<int, Dictionary<int, string>> SongOfSolomon { get; set; }
    [LightWeightProperty(22)]
    public Dictionary<int, Dictionary<int, string>> Isaiah { get; set; }
    [LightWeightProperty(23)]
    public Dictionary<int, Dictionary<int, string>> Jeremiah { get; set; }
    [LightWeightProperty(24)]
    public Dictionary<int, Dictionary<int, string>> Lamentations { get; set; }
    [LightWeightProperty(25)]
    public Dictionary<int, Dictionary<int, string>> Ezekiel { get; set; }
    [LightWeightProperty(26)]
    public Dictionary<int, Dictionary<int, string>> Daniel { get; set; }
    [LightWeightProperty(27)]
    public Dictionary<int, Dictionary<int, string>> Hosea { get; set; }
    [LightWeightProperty(28)]
    public Dictionary<int, Dictionary<int, string>> Joel { get; set; }
    [LightWeightProperty(29)]
    public Dictionary<int, Dictionary<int, string>> Amos { get; set; }
    [LightWeightProperty(30)]
    public Dictionary<int, Dictionary<int, string>> Obadiah { get; set; }
    [LightWeightProperty(31)]
    public Dictionary<int, Dictionary<int, string>> Jonah { get; set; }
    [LightWeightProperty(32)]
    public Dictionary<int, Dictionary<int, string>> Micah { get; set; }
    [LightWeightProperty(33)]
    public Dictionary<int, Dictionary<int, string>> Nahum { get; set; }
    [LightWeightProperty(34)]
    public Dictionary<int, Dictionary<int, string>> Habakkuk { get; set; }
    [LightWeightProperty(35)]
    public Dictionary<int, Dictionary<int, string>> Zephaniah { get; set; }
    [LightWeightProperty(36)]
    public Dictionary<int, Dictionary<int, string>> Haggai { get; set; }
    [LightWeightProperty(37)]
    public Dictionary<int, Dictionary<int, string>> Zechariah { get; set; }
    [LightWeightProperty(38)]
    public Dictionary<int, Dictionary<int, string>> Malachi { get; set; }

    [LightWeightProperty(39)]
    public Dictionary<int, Dictionary<int, string>> Matthew { get; set; }
    [LightWeightProperty(40)]
    public Dictionary<int, Dictionary<int, string>> Mark { get; set; }
    [LightWeightProperty(41)]
    public Dictionary<int, Dictionary<int, string>> Luke { get; set; }
    [LightWeightProperty(42)]
    public Dictionary<int, Dictionary<int, string>> John { get; set; }
    [LightWeightProperty(43)]
    public Dictionary<int, Dictionary<int, string>> Acts { get; set; }
    [LightWeightProperty(44)]
    public Dictionary<int, Dictionary<int, string>> Romans { get; set; }
    [LightWeightProperty(45)]
    [JsonProperty("1 Corinthians")]
    public Dictionary<int, Dictionary<int, string>> Corinthians1 { get; set; }
    [LightWeightProperty(46)]
    [JsonProperty("2 Corinthians")]
    public Dictionary<int, Dictionary<int, string>> Corinthians2 { get; set; }
    [LightWeightProperty(47)]
    public Dictionary<int, Dictionary<int, string>> Galatians { get; set; }
    [LightWeightProperty(48)]
    public Dictionary<int, Dictionary<int, string>> Ephesians { get; set; }
    [LightWeightProperty(49)]
    public Dictionary<int, Dictionary<int, string>> Philippians { get; set; }
    [LightWeightProperty(50)]
    public Dictionary<int, Dictionary<int, string>> Colossians { get; set; }
    [LightWeightProperty(51)]
    [JsonProperty("1 Thessalonians")]
    public Dictionary<int, Dictionary<int, string>> Thessalonians1 { get; set; }
    [LightWeightProperty(52)]
    [JsonProperty("2 Thessalonians")]
    public Dictionary<int, Dictionary<int, string>> Thessalonians2 { get; set; }
    [LightWeightProperty(53)]
    [JsonProperty("1 Timothy")]
    public Dictionary<int, Dictionary<int, string>> Timothy1 { get; set; }
    [LightWeightProperty(54)]
    [JsonProperty("2 Timothy")]
    public Dictionary<int, Dictionary<int, string>> Timothy2 { get; set; }
    [LightWeightProperty(55)]
    public Dictionary<int, Dictionary<int, string>> Titus { get; set; }
    [LightWeightProperty(56)]
    public Dictionary<int, Dictionary<int, string>> Philemon { get; set; }
    [LightWeightProperty(57)]
    public Dictionary<int, Dictionary<int, string>> Hebrews { get; set; }
    [LightWeightProperty(58)]
    public Dictionary<int, Dictionary<int, string>> James { get; set; }
    [LightWeightProperty(59)]
    [JsonProperty("1 Peter")]
    public Dictionary<int, Dictionary<int, string>> Peter1 { get; set; }
    [LightWeightProperty(60)]
    [JsonProperty("2 Peter")]
    public Dictionary<int, Dictionary<int, string>> Peter2 { get; set; }
    [LightWeightProperty(61)]
    [JsonProperty("1 John")]
    public Dictionary<int, Dictionary<int, string>> John1 { get; set; }
    [LightWeightProperty(62)]
    [JsonProperty("2 John")]
    public Dictionary<int, Dictionary<int, string>> John2 { get; set; }
    [LightWeightProperty(63)]
    [JsonProperty("3 John")]
    public Dictionary<int, Dictionary<int, string>> John3 { get; set; }
    [LightWeightProperty(64)]
    public Dictionary<int, Dictionary<int, string>> Jude { get; set; }
    [LightWeightProperty(65)]
    public Dictionary<int, Dictionary<int, string>> Revelation { get; set; }
}