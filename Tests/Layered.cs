using System;
using InvertedTomato.Serialization.LightWeightSerialization;

public class Layered {
	[LightWeightProperty(0)] public String Y;

	[LightWeightProperty(1)] public ThreeInts Z;
}