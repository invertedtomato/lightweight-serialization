using System;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class MissingIndexException : Exception {
		public MissingIndexException() { }
		public MissingIndexException(string message) : base(message) { }
		public MissingIndexException(string message, Exception inner) : base(message, inner) { }
	}
}