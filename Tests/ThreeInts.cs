using System;
using InvertedTomato.Serialization.LightWeightSerialization;

public class ThreeInts {
	[LightWeightProperty(1)] public Int32 A;

	[LightWeightProperty(0)] public Int32 B;

	[LightWeightProperty(2)] public Int32 C;
}