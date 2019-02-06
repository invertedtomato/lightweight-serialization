using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using InvertedTomato.Serialization.LightWeightSerialization.InternalCoders;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class LightWeight : ISerializer {
		private readonly List<ICoderGenerator> CodersGenerators = new List<ICoderGenerator>();
		private readonly Dictionary<Type, Delegate> DecoderCache = new Dictionary<Type, Delegate>(); // Func<TOut, T, count>
		private readonly Dictionary<Type, Delegate> EncoderCache = new Dictionary<Type, Delegate>(); // Func<T, TIn>

		private readonly Object Sync = new Object();

		private static readonly Byte[] EmptyArray = new Byte[] { };
		public static readonly Node EmptyNode = Node.Leaf(VLQCodec.Zero, EmptyArray);

		public LightWeight() {
			// Load internal coder generators
			LoadCoderGenerator(new ClassCoderGenerator()); // Must be first, so that it's considered a last resort
			LoadCoderGenerator(new IDictionaryCoderGenerator());
			LoadCoderGenerator(new IListCoderGenerator());
			LoadCoderGenerator(new ArrayCoderGenerator());

			LoadCoderGenerator(new BooleanCoderGenerator());
			LoadCoderGenerator(new SInt8CoderGenerator());
			LoadCoderGenerator(new SInt16CoderGenerator());
			LoadCoderGenerator(new SInt32CoderGenerator());
			LoadCoderGenerator(new SInt64CoderGenerator());
			LoadCoderGenerator(new UInt8CoderGenerator());
			LoadCoderGenerator(new UInt16CoderGenerator());
			LoadCoderGenerator(new UInt32CoderGenerator());
			LoadCoderGenerator(new UInt64CoderGenerator());
			LoadCoderGenerator(new StringCoderGenerator());
			
			
		}

		public Int32 Encode<T>(T value, Stream buffer) {
#if DEBUG
			if (null == buffer) {
				throw new ArgumentNullException(nameof(buffer));
			}
#endif

			// Get root serializer
			var rootSerializer = (Func<T, Node>) GetEncoder<T>();

			// Invoke root serializer
			var output = rootSerializer(value);

			// Squash notes tree into stream
			return Squash(output, buffer);
		}

		public T Decode<T>(Stream input, Int32 count) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count), "Must be at least 0.");
			}
#endif

			// Get root serializer
			var root = GetDecoder<T>();

			// Invoke root serializer
			return (T) root.DynamicInvoke(input, count);
		}

		private Int32 Squash(Node node, Stream buffer) {
			// If this is a leaf node..
			if (node.IsLeaf) {
				// Write leaf
				buffer.Write(node.EncodedValue, 0, node.EncodedValue.Length);
				return node.EncodedValue.Length;
			} else {
				// Iterate horizontally
				var count = 0;
				foreach (var childNode in node.ChildNodes) {
					// Write length
					buffer.Write(childNode.EncodedValueLength, 0, childNode.EncodedValueLength.Length);
					count += childNode.EncodedValueLength.Length;

					// Recurse vertically
					count += Squash(childNode, buffer);
				}

				return count;
			}
		}

		public void LoadCoderGenerator(ICoderGenerator generator) {
			CodersGenerators.Insert(0, generator);
		}

		public void PrepareFor<T>() {
			var type = typeof(T);

			foreach (var coder in CodersGenerators) {
				if (coder.IsCompatibleWith<T>()) {
					EncoderCache[type] = coder.GenerateEncoder(type, GetEncoder);
					DecoderCache[type] = coder.GenerateDecoder(type, GetDecoder);
					return;
				}
			}

			throw new NotSupportedException($"The type {type.ToString()} is not supported. Load a CoderGenerator to add support.");
		}

		protected Delegate GetEncoder<T>() {
			lock (Sync) {
				// If there's no coder, build one
				if (!EncoderCache.TryGetValue(typeof(T), out var encoder)) {
					PrepareFor<T>();
					encoder = EncoderCache[typeof(T)];
				}

				// Return coder
				return encoder;
			}
		}

		protected Delegate GetEncoder(Type type) {
			var method = GetType().GetRuntimeMethods().Single(a => a.Name == nameof(GetEncoder) && a.IsGenericMethod).MakeGenericMethod(type);
			return (Delegate) method.Invoke(this, null);
		}

		protected Delegate GetDecoder<T>() {
			lock (Sync) {
				// If there's no coder, build one
				if (!DecoderCache.TryGetValue(typeof(T), out var decoder)) {
					PrepareFor<T>();
					decoder = DecoderCache[typeof(T)];
				}

				// Return coder
				return decoder;
			}
		}

		protected Delegate GetDecoder(Type type) {
			var method = GetType().GetRuntimeMethods().Single(a => a.Name == nameof(GetDecoder) && a.IsGenericMethod).MakeGenericMethod(type); // NOTE "typeof(LightWeight).GetRuntimeMethod(nameof(EnsureSerializer), new Type[] { })" returns NULL for generics - why?
			return (Delegate) method.Invoke(this, null);
		}


		/// <summary>
		/// Serialize an object into a byte array.
		/// </summary>
		public static Byte[] Serialize<T>(T value) {
			using (var buffer = new MemoryStream()) {
				var lw = new LightWeight();
				lw.Encode(value, buffer);
				return buffer.ToArray();
			}
		}

		/// <summary>
		/// Deserialize an object from a byte array.
		/// </summary>
		public static T Deserialize<T>(Byte[] payload) {
			using (var buffer = new MemoryStream(payload)) {
				var lw = new LightWeight();
				return lw.Decode<T>(buffer, payload.Length);
			}
		}
	}
}