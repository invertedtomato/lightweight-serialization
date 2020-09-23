using System;

namespace InvertedTomato.Serialization.LightWeightSerialization
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class LightWeightPropertyAttribute : Attribute
    {
        public LightWeightPropertyAttribute(Byte index)
        {
            Index = index;
        }
        
        public Byte Index { get; }
    }
}