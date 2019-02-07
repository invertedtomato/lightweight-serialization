using System;
using System.Collections;
using System.Collections.Generic;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class Node : IEnumerable<Byte[]> {
		private readonly List<Byte[]> Underlying = new List<Byte[]>();
		public Node() { }

		public Node(Byte[] initial) {
			Append(initial);
		}

		public Int32 TotalLength { get; private set; }


		public IEnumerator<Byte[]> GetEnumerator() {
			return Underlying.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Underlying.GetEnumerator();
		}

		public void Append(Byte[] payload) {
			TotalLength += payload.Length;
			Underlying.Add(payload);
		}

		public void Append(Node node) {
			TotalLength += node.TotalLength;
			Underlying.AddRange(node);
		}

		public void Prepend(Byte[] payload) {
			TotalLength += payload.Length;
			Underlying.Insert(0, payload);
		}

		public void Prepend(Node node) {
			TotalLength += node.TotalLength;
			Underlying.InsertRange(0, node);
		}


		public override String ToString() {
			return $"count={Underlying.Count},length={TotalLength}";
		}
	}
}