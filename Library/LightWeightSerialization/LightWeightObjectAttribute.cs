using System;

namespace InvertedTomato.Serialization.LightWeightSerialization {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class LightWeightObjectAttribute : Attribute {
        public LightWeightObjectAttribute() { }
    }
}
