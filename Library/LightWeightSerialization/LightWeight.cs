using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using InvertedTomato.Serialization.LightWeightSerialization.InternalCoders;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	/// <summary>
	/// Threadsafe.
	/// </summary>
	public class LightWeight : ISerializer {
		private readonly List<ICoderGenerator> CodersGenerators = new List<ICoderGenerator>();
		private readonly Dictionary<Type, Delegate> DecoderCache = new Dictionary<Type, Delegate>(); // Func<TOut, T, count>
		private readonly Dictionary<Type, Delegate> EncoderCache = new Dictionary<Type, Delegate>(); // Func<T, TIn>

		private readonly Object Sync = new Object();

		public LightWeight() {
			// Load internal coder generators (applied in reverse order of priority)
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
			LoadCoderGenerator(new SingleCoderGenerator());
			LoadCoderGenerator(new DoubleCoderGenerator());
		}

		public Byte[] Encode<T>(T value) {
			// Get root serializer
			var rootSerializer = GetEncoder<T>();

			// Invoke root serializer
			var output = (EncodeBuffer) rootSerializer.DynamicInvoke(value);

			// Allocate output buffer
			var buffer = new Byte[output.TotalLength];
			var offset = 0;

			// Squash notes tree into stream
			for (var i = output.Offset; i < output.Offset + output.Count; i++) {
				var payload = output.Underlying[i];
				Buffer.BlockCopy(payload.Array, payload.Offset, buffer, offset, payload.Count);
				offset += payload.Count;
			}

			return buffer;
		}

		public T Decode<T>(Byte[] input) {
			return Decode<T>(new ArraySegment<Byte>(input));
		}


		public T Decode<T>(ArraySegment<Byte> input) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}
#endif

			// Get root serializer
			var root = GetDecoder<T>();

			// Invoke root serializer
			var value = root.DynamicInvoke(new DecodeBuffer(input));
			return (T) value;
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

			throw new NotSupportedException($"The type {type} is not supported. Load a CoderGenerator to add support.");
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
		///     Serialize an object into a byte array.
		/// </summary>
		public static Byte[] Serialize<T>(T value) {
			var lw = new LightWeight();
			return lw.Encode(value);
		}

		/// <summary>
		///     Deserialize an object from a byte array.
		/// </summary>
		public static T Deserialize<T>(Byte[] payload) {
			var lw = new LightWeight();
			return lw.Decode<T>(payload);
		}
	}
}