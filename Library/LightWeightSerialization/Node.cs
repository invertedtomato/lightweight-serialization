using System;
using System.Collections;
using System.Collections.Generic;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class Node : IEnumerable<ArraySegment<Byte>> {
		private readonly List<ArraySegment<Byte>> Underlying = new List<ArraySegment<Byte>>();
		public Node() { }

		public Node(ArraySegment<Byte> initial) {
			Append(initial);
		}

		public Int32 TotalLength { get; private set; }


		public IEnumerator<ArraySegment<Byte>> GetEnumerator() {
			return Underlying.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Underlying.GetEnumerator();
		}

		public void Append(ArraySegment<Byte> payload) {
			TotalLength += payload.Count;
			Underlying.Add(payload);
		}

		public void Append(Node node) {
			TotalLength += node.TotalLength;
			Underlying.AddRange(node.Underlying);
		}

		public void Prepend(ArraySegment<Byte> payload) {
			TotalLength += payload.Count;
			Underlying.Insert(0, payload);
		}

		public void Prepend(Node node) {
			TotalLength += node.TotalLength;
			Underlying.InsertRange(0, node.Underlying);
		}


		public override String ToString() {
			return $"count={Underlying.Count},length={TotalLength}";
		}
	}
}