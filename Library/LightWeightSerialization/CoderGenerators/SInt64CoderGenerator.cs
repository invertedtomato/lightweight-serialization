using System;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.CoderGenerators
{
    public class SInt64CoderGenerator : ICoderGenerator
    {
        public Boolean IsCompatibleWith<T>()
        {
            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();
            return type == typeof(Int64) || // Standard value
                   (typeInfo.IsEnum && typeInfo.GetEnumUnderlyingType() == typeof(Int64)); // Enum value
        }

        public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse)
        {
            return new Func<Int64, EncodeBuffer>(value => { return new EncodeBuffer(SignedVlq.Encode(value)); });
        }

        public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse)
        {
            return new Func<DecodeBuffer, Int64>(input =>
            {
                return SignedVlq.Decode(input);
            });
        }
    }
}