using System;

namespace InvertedTomato.Serialization
{
    public class DataFormatException : Exception
    {
        public DataFormatException() { }
        public DataFormatException(String message) : base(message) { }
        public DataFormatException(String message, Exception innerException) : base(message, innerException) { }
    }
}