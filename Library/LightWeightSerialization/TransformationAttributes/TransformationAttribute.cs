using System;

namespace InvertedTomato.Serialization.LightWeightSerialization.TransformationAttributes
{
    public abstract class TransformationAttribute : Attribute
    {
        public abstract Object ApplyEffect(Object input);
        public abstract Object RemoveEffect(Object input);
    }
}