using InvertedTomato.LightWeightSerialization;

class ThreeInts {
    [LightWeightProperty(1)]
    public int A { get; set; }

    [LightWeightProperty(0)]
    public int B { get; set; }

    [LightWeightProperty(2)]
    public int C { get; set; }
}

