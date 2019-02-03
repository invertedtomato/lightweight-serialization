using System;
using System.Linq;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class Node {
		/// <summary>
		/// Create a new leaf node.
		/// </summary>
		public static Node Leaf(Byte[] encodedValueLength, Byte[] encodedValue) {
#if DEBUG
			if (null == encodedValueLength) {
				throw new ArgumentNullException(nameof(encodedValueLength));
			}

			if (null == encodedValue) {
				throw new ArgumentNullException(nameof(encodedValue));
			}
#endif

			return new Node() {
				EncodedValueLength = encodedValueLength,
				EncodedValue = encodedValue,

				TotalLength = encodedValueLength.Length + encodedValue.Length,
				TotalCount = 1,

				ChildNodes = null // Leaf
			};
		}

		/// <summary>
		/// Create a new non-leaf node.
		/// </summary>
		public static Node NonLeaf(Byte[] encodedLength, NodeSet childNodes) {
#if DEBUG
			if (null == encodedLength) {
				throw new ArgumentNullException(nameof(encodedLength));
			}

			if (null == childNodes) {
				throw new ArgumentNullException(nameof(childNodes));
			}
#endif

			return new Node() {
				EncodedValueLength = encodedLength,
				EncodedValue = null,

				TotalLength = encodedLength.Length + childNodes.Sum(a => a.TotalLength),
				TotalCount = childNodes.Sum(a => a.TotalCount) + 1,

				ChildNodes = childNodes // Non-leaf
			};
		}

		private Node() { }

		/// <summary>
		/// The encoded length of this node's payload.
		/// </summary>
		public Byte[] EncodedValueLength { get; private set; }

		/// <summary>
		/// (Only if a leaf node) the encoded value of the field.
		/// </summary>
		public Byte[] EncodedValue { get; private set; }
		

		/// <summary>
		/// Total number of bytes of this nodes and all decendants.
		/// </summary>
		public Int32 TotalLength { get; private set; }

		/// <summary>
		/// Total number of decendants.
		/// </summary>
		public Int32 TotalCount { get; private set; }
		
		

		/// <summary>
		/// (Only if non-lead) This nodes decendants.
		/// </summary>
		public NodeSet ChildNodes { get; private set; }

		/// <summary>
		/// If this is a leaf node.
		/// </summary>
		public Boolean IsLeaf {
			get { return null != EncodedValue; }
		}

		public override String ToString() {
			if (IsLeaf) {
				return $"LEAF Count={TotalCount},Length={TotalLength}";
			} else {
				return $"NON-LEAF Count={TotalCount},Length={TotalLength},Children={ChildNodes.Length}";
			}
		}
	}
}