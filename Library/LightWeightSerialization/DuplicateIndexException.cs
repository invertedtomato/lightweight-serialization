using System;

namespace InvertedTomato.Serialization.LightWeightSerialization
{
    public class DuplicateIndexException : Exception
    {
        public DuplicateIndexException() { }
        public DuplicateIndexException(String message) : base(message) { }
        public DuplicateIndexException(String message, Exception inner) : base(message, inner) { }
    }
}