using System;
using System.Collections;
using System.Collections.Generic;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public struct Node : IEnumerable<ArraySegment<Byte>> {
		private List<ArraySegment<Byte>> Underlying;

		public Int32 TotalLength { get; private set; }


		public Node(ArraySegment<Byte> payload) {
			TotalLength = 0;
			Underlying = new List<ArraySegment<Byte>>();
			Append(payload);
		}
		
		public IEnumerator<ArraySegment<Byte>> GetEnumerator() {
			return Underlying.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Underlying.GetEnumerator();
		}

		public void Append(ArraySegment<Byte> payload) {
			if (null == Underlying) {
				Underlying = new List<ArraySegment<Byte>>();
			}
			TotalLength += payload.Count;
			Underlying.Add(payload);
		}

		public void Append(Node node) {
			if (null == Underlying) {
				Underlying = new List<ArraySegment<Byte>>();
			}
			TotalLength += node.TotalLength;
			Underlying.AddRange(node.Underlying);
		}

		public void Prepend(ArraySegment<Byte> payload) {
			if (null == Underlying) {
				Underlying = new List<ArraySegment<Byte>>();
			}
			TotalLength += payload.Count;
			Underlying.Insert(0, payload);
		}

		public void Prepend(Node node) {
			if (null == Underlying) {
				Underlying = new List<ArraySegment<Byte>>();
			}
			TotalLength += node.TotalLength;
			Underlying.InsertRange(0, node.Underlying);
		}


		public override String ToString() {
			return $"count={Underlying.Count},length={TotalLength}";
		}
	}
}