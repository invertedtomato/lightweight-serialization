using System;

namespace InvertedTomato.Serialization.LightWeightSerialization
{
    public class SchemaMismatchException : Exception
    {
        public SchemaMismatchException() { }
        public SchemaMismatchException(String message) : base(message) { }
        public SchemaMismatchException(String message, Exception inner) : base(message, inner) { }
    }
}