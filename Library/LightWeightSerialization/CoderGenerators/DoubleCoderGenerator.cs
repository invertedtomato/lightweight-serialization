using System;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.CoderGenerators
{
    public class DoubleCoderGenerator : ICoderGenerator
    {
        public Boolean IsCompatibleWith<T>()
        {
            return typeof(T) == typeof(Double);
        }

        public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse)
        {
            return new Func<Double, EncodeBuffer>(value =>
            {
                return new EncodeBuffer(new ArraySegment<Byte>(BitConverter.GetBytes(value)));
            });
        }

        public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse)
        {
            return new Func<DecodeBuffer, Double>(input =>
            {
                return BitConverter.ToDouble(input.Underlying, input.GetIncrementOffset(8));
            });
        }
    }
}