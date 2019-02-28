using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;
using InvertedTomato.Serialization.LightWeightSerialization.InternalCoders;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class LightWeight : ISerializer {
		private static readonly Byte[] EmptyArray = { };
		private readonly List<ICoderGenerator> CodersGenerators = new List<ICoderGenerator>();
		private readonly Dictionary<Type, Delegate> DecoderCache = new Dictionary<Type, Delegate>(); // Func<TOut, T, count>
		private readonly Dictionary<Type, Delegate> EncoderCache = new Dictionary<Type, Delegate>(); // Func<T, TIn>

		private readonly Object Sync = new Object();

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

		public void Encode<T>(T value, Stream buffer) {
#if DEBUG
			if (null == buffer) {
				throw new ArgumentNullException(nameof(buffer));
			}
#endif

			// Get root serializer
			var rootSerializer = GetEncoder<T>();

			// Invoke root serializer
			var output = (Node)rootSerializer.DynamicInvoke(value);

			// Squash notes tree into stream
			foreach (var payload in output) {
				buffer.Write(payload.Array, payload.Offset, payload.Count);
			}
		}

		public T Decode<T>(Stream input) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}
#endif

			// Get root serializer
			var root = GetDecoder<T>();

			// Invoke root serializer
			var value = root.DynamicInvoke(input);
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
			using (var buffer = new MemoryStream()) {
				var lw = new LightWeight();
				lw.Encode(value, buffer);
				return buffer.ToArray();
			}
		}

		/// <summary>
		///     Deserialize an object from a byte array.
		/// </summary>
		public static T Deserialize<T>(Byte[] payload) {
			using (var buffer = new MemoryStream(payload)) {
				var lw = new LightWeight();
				return lw.Decode<T>(buffer);
			}
		}
	}
}