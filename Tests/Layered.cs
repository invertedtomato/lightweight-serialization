using InvertedTomato.Serialization.LightWeightSerialization;

class Layered {
    [LightWeightProperty(0)]
    public string Y { get; set; }

    [LightWeightProperty(1)]
    public ThreeInts Z { get; set; }
}
