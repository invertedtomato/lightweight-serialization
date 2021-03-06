using System;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.CoderGenerators
{
    public class UInt16CoderGenerator : ICoderGenerator
    {
        public Boolean IsCompatibleWith<T>()
        {
            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();
            return type == typeof(UInt16) || // Standard value
                   (typeInfo.IsEnum && typeInfo.GetEnumUnderlyingType() == typeof(UInt16)); // Enum value
        }

        public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse)
        {
            return new Func<UInt16, EncodeBuffer>(value =>
            {
                return new EncodeBuffer(UnsignedVlq.Encode(value));
            });
        }

        public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse)
        {
            return new Func<DecodeBuffer, UInt16>(input =>
            {
                return (UInt16)UnsignedVlq.Decode(input);
            });
        }
    }
}