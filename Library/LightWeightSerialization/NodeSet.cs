using System;
using System.Collections;
using System.Collections.Generic;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class NodeSet : IEnumerable<Node> {
		private readonly Node[] Underlying;
		private Int32 Position;
		public Int32 TotalLength { get; private set; }
		
		public Int32 Length => Underlying.Length;

		public NodeSet(Int32 length) {
			Underlying = new Node[length];
		}

		public void Add(Node item) {
			Underlying[Position++] = item;
			TotalLength += item.TotalLength;
		}

		public IEnumerator<Node> GetEnumerator() {
			return ((IEnumerable<Node>) Underlying).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Underlying.GetEnumerator();
		}
	}
}