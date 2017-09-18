using InvertedTomato.Serialization.LightWeightSerialization;

public class Layered {
    [LightWeightProperty(0)]
    public string Y;

    [LightWeightProperty(1)]
    public ThreeInts Z;
}
