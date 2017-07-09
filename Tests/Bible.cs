using InvertedTomato.LightWeightSerialization;
using System;
using System.Collections.Generic;
using System.Text;


public class Bible {
    [LightWeightProperty(0)]
    public Chapter[] Genesis { get; set; }
    [LightWeightProperty(1)]
    public Chapter[] Exodus { get; set; }
    [LightWeightProperty(2)]
    public Chapter[] Leviticus { get; set; }
    [LightWeightProperty(3)]
    public Chapter[] Numbers { get; set; }
    [LightWeightProperty(4)]
    public Chapter[] Deuteronomy { get; set; }
    [LightWeightProperty(5)]
    public Chapter[] Joshua { get; set; }
    [LightWeightProperty(6)]
    public Chapter[] Judges { get; set; }
    [LightWeightProperty(7)]
    public Chapter[] Ruth { get; set; }
    [LightWeightProperty(8)]
    public Chapter[] Samuel1 { get; set; }
    [LightWeightProperty(9)]
    public Chapter[] Samuel2 { get; set; }
    [LightWeightProperty(10)]
    public Chapter[] Kings1 { get; set; }
    [LightWeightProperty(11)]
    public Chapter[] Kings2 { get; set; }
    [LightWeightProperty(12)]
    public Chapter[] Chronicles1 { get; set; }
    [LightWeightProperty(13)]
    public Chapter[] Chronicles2 { get; set; }
    [LightWeightProperty(14)]
    public Chapter[] Ezra { get; set; }
    [LightWeightProperty(15)]
    public Chapter[] Nehemiah { get; set; }
    [LightWeightProperty(16)]
    public Chapter[] Esther { get; set; }
    [LightWeightProperty(17)]
    public Chapter[] Job { get; set; }
    [LightWeightProperty(18)]
    public Chapter[] Psalms { get; set; }
    [LightWeightProperty(19)]
    public Chapter[] Proverbs { get; set; }
    [LightWeightProperty(20)]
    public Chapter[] Ecclesiastes { get; set; }
    [LightWeightProperty(21)]
    public Chapter[] SongOfSolomon { get; set; }
    [LightWeightProperty(22)]
    public Chapter[] Isaiah { get; set; }
    [LightWeightProperty(23)]
    public Chapter[] Jeremiah { get; set; }
    [LightWeightProperty(24)]
    public Chapter[] Lamentations { get; set; }
    [LightWeightProperty(25)]
    public Chapter[] Ezekiel { get; set; }
    [LightWeightProperty(26)]
    public Chapter[] Daniel { get; set; }
    [LightWeightProperty(27)]
    public Chapter[] Hosea { get; set; }
    [LightWeightProperty(28)]
    public Chapter[] Joel { get; set; }
    [LightWeightProperty(29)]
    public Chapter[] Amos { get; set; }
    [LightWeightProperty(30)]
    public Chapter[] Obadiah { get; set; }
    [LightWeightProperty(31)]
    public Chapter[] Jonah { get; set; }
    [LightWeightProperty(32)]
    public Chapter[] Micah { get; set; }
    [LightWeightProperty(33)]
    public Chapter[] Nahum { get; set; }
    [LightWeightProperty(34)]
    public Chapter[] Habakkuk { get; set; }
    [LightWeightProperty(35)]
    public Chapter[] Zephaniah { get; set; }
    [LightWeightProperty(36)]
    public Chapter[] Haggai { get; set; }
    [LightWeightProperty(37)]
    public Chapter[] Zechariah { get; set; }
    [LightWeightProperty(38)]
    public Chapter[] Malachi { get; set; }

    [LightWeightProperty(39)]
    public Chapter[] Matthew { get; set; }
    [LightWeightProperty(40)]
    public Chapter[] Mark { get; set; }
    [LightWeightProperty(41)]
    public Chapter[] Luke { get; set; }
    [LightWeightProperty(42)]
    public Chapter[] John { get; set; }
    [LightWeightProperty(43)]
    public Chapter[] Acts { get; set; }
    [LightWeightProperty(44)]
    public Chapter[] Romans { get; set; }
    [LightWeightProperty(45)]
    public Chapter[] Corinthians1 { get; set; }
    [LightWeightProperty(46)]
    public Chapter[] Corinthians2 { get; set; }
    [LightWeightProperty(47)]
    public Chapter[] Galatians { get; set; }
    [LightWeightProperty(48)]
    public Chapter[] Ephesians { get; set; }
    [LightWeightProperty(49)]
    public Chapter[] Philippians { get; set; }
    [LightWeightProperty(50)]
    public Chapter[] Colossians { get; set; }
    [LightWeightProperty(51)]
    public Chapter[] Thessalonians1 { get; set; }
    [LightWeightProperty(52)]
    public Chapter[] Thessalonians2 { get; set; }
    [LightWeightProperty(53)]
    public Chapter[] Timothy1 { get; set; }
    [LightWeightProperty(54)]
    public Chapter[] Timothy2 { get; set; }
    [LightWeightProperty(55)]
    public Chapter[] Titus { get; set; }
    [LightWeightProperty(56)]
    public Chapter[] Philemon { get; set; }
    [LightWeightProperty(57)]
    public Chapter[] Hebrews { get; set; }
    [LightWeightProperty(58)]
    public Chapter[] James { get; set; }
    [LightWeightProperty(59)]
    public Chapter[] Peter1 { get; set; }
    [LightWeightProperty(60)]
    public Chapter[] Peter2 { get; set; }
    [LightWeightProperty(61)]
    public Chapter[] John1 { get; set; }
    [LightWeightProperty(62)]
    public Chapter[] John2 { get; set; }
    [LightWeightProperty(63)]
    public Chapter[] John3 { get; set; }
    [LightWeightProperty(64)]
    public Chapter[] Jude { get; set; }
    [LightWeightProperty(65)]
    public Chapter[] Revelation { get; set; }
}

public class Chapter {
    [LightWeightProperty(0)]
    public string[] Verses { get; set; }
}