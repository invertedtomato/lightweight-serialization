using System;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class SchemaMismatchException : Exception {
		public SchemaMismatchException() { }
		public SchemaMismatchException(string message) : base(message) { }
		public SchemaMismatchException(string message, Exception inner) : base(message, inner) { }
	}
}