using System;

namespace InvertedTomato.LightWeightSerialization {
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class LightWeightObjectAttribute : Attribute {
        public LightWeightObjectAttribute() { }
    }
}
