using System;
using System.IO;

namespace InvertedTomato.Serialization
{
    public interface ISerializer
    {
        Byte[] Encode<T>(T value);
        T Decode<T>(Byte[] input);
    }
}