using System;

namespace InvertedTomato.Serialization.LightWeightSerialization
{
    public class UnsupportedDataTypeException : Exception
    {
        public UnsupportedDataTypeException() { }
        public UnsupportedDataTypeException(String message) : base(message) { }
        public UnsupportedDataTypeException(String message, Exception inner) : base(message, inner) { }
    }
}