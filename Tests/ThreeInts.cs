using InvertedTomato.Serialization.LightWeightSerialization;

public class ThreeInts {
    [LightWeightProperty(1)]
    public int A;

    [LightWeightProperty(0)]
    public int B;

    [LightWeightProperty(2)]
    public int C;
}

