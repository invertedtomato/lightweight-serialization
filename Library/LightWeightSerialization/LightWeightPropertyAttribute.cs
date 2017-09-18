using System;

namespace InvertedTomato.Serialization.LightWeightSerialization {
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class LightWeightPropertyAttribute : Attribute {
        public LightWeightPropertyAttribute(byte index) {
            Index = index;
        }

        public byte Index { get; private set; }
    }
}
