﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public struct Node {
		private const Int32 InitialSize = 8;

		public ArraySegment<Byte>[] Underlying;
		public Int32 Offset { get; private set; }
		public Int32 Count { get; private set; }

		public Int32 TotalLength { get; private set; }

		private Int32 Available {
			get { return Underlying.Length - Offset - Count; }
		}

		/// <summary>
		/// Initialize to a given size, leaving an additional element free at the start for a header.
		/// </summary>
		public Node(Int32 initialSize) {
			TotalLength = 0;
			Offset = 1;
			Count = 0;
			Underlying = new ArraySegment<Byte>[initialSize];
		}

		/// <summary>
		/// Initialize with a given set of payloads, leaving no room for a header.
		/// </summary>
		/// <param name="payloads"></param>
		public Node(params ArraySegment<Byte>[] payloads) {
			TotalLength = payloads.Sum(a => a.Count);
			Offset = 0;
			Count = payloads.Length;
			Underlying = payloads;
		}

		public void Append(ArraySegment<Byte> payload) {
			if (null == Underlying) {
				Offset = 1;
				Underlying = new ArraySegment<Byte>[InitialSize];
			}

			if (Available < 1) {
				Array.Resize(ref Underlying, Underlying.Length * 2);
			}

			TotalLength += payload.Count;
			Underlying[Offset + Count] = payload;
			Count++;
		}

		public void Append(Node node) {
			if (null == Underlying) {
				Offset = 1;
				Underlying = new ArraySegment<Byte>[InitialSize];
			}

			if (Available < node.Count) {
				Array.Resize(ref Underlying, Math.Max(Offset + Count + node.Count, Underlying.Length * 2));
			}

			Array.Copy(node.Underlying, node.Offset, Underlying, Offset + Count, node.Count);
			Count += node.Count;
			TotalLength += node.TotalLength;
		}

		public void SetFirst(ArraySegment<Byte> payload) {
			if (null == Underlying) {
				Offset = 1;
				Underlying = new ArraySegment<Byte>[InitialSize];
			}

			TotalLength += payload.Count - Underlying[0].Count;
			Underlying[0] = payload;
			if (Offset == 1) {
				Offset = 0;
				Count++;
			}
		}

		public override String ToString() {
			return $"count={Count},length={TotalLength}";
		}
	}
}