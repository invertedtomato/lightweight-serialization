using System;
using System.IO;
using System.Reflection;

namespace InvertedTomato.Serialization.LightWeightSerialization.CoderGenerators
{
    public class UInt8CoderGenerator : ICoderGenerator
    {
        public Boolean IsCompatibleWith<T>()
        {
            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();
            return type == typeof(Byte) || // Standard value
                   (typeInfo.IsEnum && typeInfo.GetEnumUnderlyingType() == typeof(Byte)); // Enum value
        }

        public Delegate GenerateEncoder(Type type, Func<Type, Delegate> recurse)
        {
            return new Func<Byte, EncodeBuffer>(value => { return new EncodeBuffer(new ArraySegment<Byte>(new Byte[] { value })); });
        }

        public Delegate GenerateDecoder(Type type, Func<Type, Delegate> recurse)
        {
            return new Func<DecodeBuffer, Byte>(input =>
            {
                return input.ReadByte();
            });
        }
    }
}