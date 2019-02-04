using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using InvertedTomato.Compression.Integers;
using InvertedTomato.Serialization.LightWeightSerialization.Extensions;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class LightWeight : ISerializer {
		private readonly Dictionary<Type, Delegate> Decoders = new Dictionary<Type, Delegate>(); // Func<TOut, T>
		private readonly Dictionary<Type, Delegate> Encoders = new Dictionary<Type, Delegate>(); // Func<T, TIn>

		private readonly Object Sync = new Object();
		private readonly VLQCodec VLQ = new VLQCodec();

		private static readonly Byte[] EmptyArray = new Byte[] { };
		private static readonly Node EmptyLeafNode = Node.Leaf(VLQCodec.Zero, EmptyArray);

		public LightWeight() { }

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

		public void PrepareFor<T>() {
			Encoders[typeof(T)] = GenerateEncoderFor<T>();
			Decoders[typeof(T)] = GenerateDecoderFor<T>();
		}

		protected Delegate GetEncoder<T>() {
			lock (Sync) {
				// If there's no coder, build one
				if (!Encoders.TryGetValue(typeof(T), out var encoder)) {
					PrepareFor<T>();
					encoder = Encoders[typeof(T)];
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
				if (!Decoders.TryGetValue(typeof(T), out var decoder)) {
					PrepareFor<T>();
					decoder = Decoders[typeof(T)];
				}

				// Return coder
				return decoder;
			}
		}

		protected Delegate GetDecoder(Type type) {
			var method = GetType().GetRuntimeMethods().Single(a => a.Name == nameof(GetDecoder) && a.IsGenericMethod).MakeGenericMethod(type); // NOTE "typeof(LightWeight).GetRuntimeMethod(nameof(EnsureSerializer), new Type[] { })" returns NULL for generics - why?
			return (Delegate) method.Invoke(this, null);
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

		protected Delegate GenerateEncoderFor<T>() {
			if (typeof(T) == typeof(Boolean)) {
				return GenerateBoolEncoder();
			}

			if (typeof(T) == typeof(SByte)) {
				return GenerateSInt8Encoder();
			}

			if (typeof(T) == typeof(Int16)) {
				return GenerateSInt16Encoder();
			}

			if (typeof(T) == typeof(Int32)) {
				return GenerateSInt32Encoder();
			}

			if (typeof(T) == typeof(Int64)) {
				return GenerateSInt64Encoder();
			}

			if (typeof(T) == typeof(Byte)) {
				return GenerateUInt8Encoder();
			}

			if (typeof(T) == typeof(UInt16)) {
				return GenerateUInt16Encoder();
			}

			if (typeof(T) == typeof(UInt32)) {
				return GenerateUInt32Encoder();
			}

			if (typeof(T) == typeof(UInt64)) {
				return GenerateUInt64Encoder();
			}

			if (typeof(T) == typeof(String)) {
				return GenerateStringEncoder();
			}

			if (typeof(T).IsArray) {
				return GenerateArrayEncoder<T>();
			}

			if (typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T))) {
				return GenerateListEncoder<T>();
			}

			if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T))) {
				return GenerateDictionaryEncoder<T>();
			}

			if (typeof(T).GetTypeInfo().IsClass) {
				return GeneratePOCOEncoder<T>();
			}

			throw new NotSupportedException();
		}

		protected Delegate GenerateDecoderFor<T>() {
			if (typeof(T) == typeof(Boolean)) {
				return GenerateBoolDecoder();
			}

			if (typeof(T) == typeof(SByte)) {
				return GenerateSInt8Decoder();
			}

			if (typeof(T) == typeof(Int16)) {
				return GenerateSInt16Decoder();
			}

			if (typeof(T) == typeof(Int32)) {
				return GenerateSInt32Decoder();
			}

			if (typeof(T) == typeof(Int64)) {
				return GenerateSInt64Decoder();
			}

			if (typeof(T) == typeof(Byte)) {
				return GenerateUInt8Decoder();
			}

			if (typeof(T) == typeof(UInt16)) {
				return GenerateUInt16Decoder();
			}

			if (typeof(T) == typeof(UInt32)) {
				return GenerateUInt32Decoder();
			}

			if (typeof(T) == typeof(UInt64)) {
				return GenerateUInt64Decoder();
			}

			if (typeof(T) == typeof(String)) {
				return GenerateStringDecoder();
			}

			if (typeof(T).IsArray) {
				return GenerateArrayDecoder<T>();
			}

			if (typeof(IList).GetTypeInfo().IsAssignableFrom(typeof(T))) {
				return GenerateListDecoder<T>();
			}

			if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(typeof(T))) {
				return GenerateDictionaryDecoder<T>();
			}

			if (typeof(T).GetTypeInfo().IsClass) {
				return GeneratePOCODecoder<T>();
			}

			throw new NotSupportedException();
		}

		private Func<Boolean, Node> GenerateBoolEncoder() {
			return value => {
				if (!value) {
					return EmptyLeafNode;
				}

				return Node.Leaf(VLQCodec.One, new Byte[] {0x00});
			};
		}

		private Func<Stream, Int32, Boolean> GenerateBoolDecoder() {
			return (buffer, count) => {
				if (count == 0) {
					return false;
				}
#if DEBUG
				if (count != 1) {
					throw new DataFormatException($"Boolean values can be no more than 1 byte long, but {count} found..");
				}
#endif
				if (buffer.ReadByte() != 0x00) {
					throw new DataFormatException("Boolean values cannot be anything other than 0x00.");
				}

				return true;
			};
		}

		private Func<SByte, Node> GenerateSInt8Encoder() {
			return value => {
				if (value == 0) {
					return EmptyLeafNode;
				}

				return Node.Leaf(VLQCodec.One, new[] {(Byte) value});
			};
		}

		private Func<Stream, Int32, SByte> GenerateSInt8Decoder() {
			return (buffer, count) => {
				if (count == 0) {
					return 0;
				} else if (count == 1) {
					return (SByte) buffer.ReadByte();
				} else {
					throw new DataFormatException($"SInt64 values can be 0 or 1 bytes, but {count} found..");
				}
			};
		}

		private Func<Int16, Node> GenerateSInt16Encoder() {
			var smaller = GenerateSInt8Encoder();

			return value => {
				if (value <= SByte.MaxValue && value >= SByte.MinValue) {
					return smaller((SByte) value);
				}

				return Node.Leaf(VLQCodec.Two, BitConverter.GetBytes(value));
			};
		}

		private Func<Stream, Int32, Int16> GenerateSInt16Decoder() {
			var smaller = GenerateSInt8Decoder();

			return (buffer, count) => {
				if (count < 2) {
					return smaller(buffer, count);
				} else if (count == 2) {
					return BitConverter.ToInt16(buffer.Read(2), 0);
				} else {
					throw new DataFormatException($"SInt64 values can be 0, 1 or 2 bytes, but {count} found.");
				}
			};
		}

		private Func<Int32, Node> GenerateSInt32Encoder() {
			var smaller = GenerateSInt16Encoder();

			return value => {
				if (value <= Int16.MaxValue && value >= Int16.MinValue) {
					return smaller((Int16) value);
				}

				return Node.Leaf(VLQCodec.Four, BitConverter.GetBytes(value));
			};
		}

		private Func<Stream, Int32, Int32> GenerateSInt32Decoder() {
			var smaller = GenerateSInt16Decoder();

			return (buffer, count) => {
				if (count < 4) {
					return smaller(buffer, count);
				} else if (count == 4) {
					return BitConverter.ToInt32(buffer.Read(4), 0);
				} else {
					var dump = buffer.Read(5);
					throw new DataFormatException($"SInt32 values can be 0, 1, 2 or 4 bytes, but {count} found.");
				}
			};
		}

		private Func<Int64, Node> GenerateSInt64Encoder() {
			var smaller = GenerateSInt32Encoder();
			return value => {
				if (value <= Int32.MaxValue && value >= Int32.MinValue) {
					return smaller((Int32) value);
				}

				return Node.Leaf(VLQCodec.Eight, BitConverter.GetBytes(value));
			};
		}

		private Func<Stream, Int32, Int64> GenerateSInt64Decoder() {
			var smaller = GenerateSInt32Decoder();

			return (buffer, count) => {
				if (count < 8) {
					return smaller(buffer, count);
				} else if (count == 8) {
					return BitConverter.ToInt64(buffer.Read(8), 0);
				} else {
					throw new DataFormatException($"SInt64 values can be 0, 1, 2, 4 or 8 bytes, but {count} found..");
				}
			};
		}

		private Func<Byte, Node> GenerateUInt8Encoder() {
			return value => {
				if (value == 0) {
					return EmptyLeafNode;
				}

				return Node.Leaf(VLQCodec.One, new[] {value});
			};
		}

		private Func<Stream, Int32, Byte> GenerateUInt8Decoder() {
			return (buffer, count) => {
				if (count == 0) {
					return 0;
				} else if (count == 1) {
					return (Byte) buffer.ReadByte();
				} else {
					throw new DataFormatException($"UInt64 values can be 0 or 1 bytes, but {count} found..");
				}
			};
		}

		private Func<UInt16, Node> GenerateUInt16Encoder() {
			var smaller = GenerateUInt8Encoder();
			return value => {
				if (value <= Byte.MaxValue) {
					return smaller((Byte) value);
				}

				return Node.Leaf(VLQCodec.Two, BitConverter.GetBytes(value));
			};
		}

		private Func<Stream, Int32, UInt16> GenerateUInt16Decoder() {
			var smaller = GenerateUInt8Decoder();

			return (buffer, count) => {
				if (count < 2) {
					return smaller(buffer, count);
				} else if (count == 2) {
					return BitConverter.ToUInt16(buffer.Read(2), 0);
				} else {
					throw new DataFormatException($"UInt64 values can be 0, 1 or 2 bytes, but {count} found..");
				}
			};
		}

		private Func<UInt32, Node> GenerateUInt32Encoder() {
			var smaller = GenerateUInt16Encoder();
			return value => {
				if (value <= UInt16.MaxValue) {
					return smaller((UInt16) value);
				}

				return Node.Leaf(VLQCodec.Four, BitConverter.GetBytes(value));
			};
		}

		private Func<Stream, Int32, UInt32> GenerateUInt32Decoder() {
			var smaller = GenerateUInt16Decoder();
			return (buffer, count) => {
				if (count < 4) {
					return smaller(buffer, count);
				} else if (count == 4) {
					return BitConverter.ToUInt32(buffer.Read(4), 0);
				} else {
					throw new DataFormatException($"UInt32 values can be 0, 1, 2 or 4 bytes, but {count} found..");
				}
			};
		}

		private Func<UInt64, Node> GenerateUInt64Encoder() {
			var smaller = GenerateUInt32Encoder();
			return value => {
				if (value <= UInt32.MaxValue) {
					return smaller((UInt32) value);
				}

				return Node.Leaf(VLQCodec.Eight, BitConverter.GetBytes(value));
			};
		}

		private Func<Stream, Int32, UInt64> GenerateUInt64Decoder() {
			var smaller = GenerateUInt32Decoder();

			return (buffer, count) => {
				if (count < 8) {
					return smaller(buffer, count);
				} else if (count == 8) {
					return BitConverter.ToUInt64(buffer.Read(8), 0);
				} else {
					throw new DataFormatException($"UInt64 values can be 0, 1, 2, 4 or 8 bytes, but {count} found..");
				}
			};
		}

		private Func<String, Node> GenerateStringEncoder() {
			return value => {
				if (null == value) {
					return EmptyLeafNode;
				}

				var encodedValue = Encoding.UTF8.GetBytes(value);
				var encodedLength = VLQ.CompressUnsigned((UInt64)encodedValue.Length).ToArray();
				return Node.Leaf(encodedLength, encodedValue);
			};
		}

		private Func<Stream, Int32, String> GenerateStringDecoder() {
			return (buffer, count) => Encoding.UTF8.GetString(buffer.Read(count), 0, count);
		}

		private Func<Array, Node> GenerateArrayEncoder<T>() {
			// Get serializer for sub items
			var valueEncoder = GetEncoder(typeof(T).GetElementType());

			return value => {
				// Handle nulls
				if (null == value) {
					return EmptyLeafNode;
				}

				// Serialize elements
				var childNodes = new NodeSet(value.Length);
				foreach (var subValue in value) {
					childNodes.Add((Node) valueEncoder.DynamicInvoke(subValue));
				}

				// Encode length
				var encodedLength = VLQ.CompressUnsigned((UInt64)childNodes.TotalLength).ToArray();

				return Node.NonLeaf(encodedLength, childNodes);
			};
		}

		private Func<Stream, Int32, Array> GenerateArrayDecoder<T>() {
			// Get deserializer for sub items
			var valueDecoder = GetDecoder(typeof(T).GetElementType());

			return (buffer, count) => {
				// Instantiate list
				var container = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(typeof(T).GetElementType()));

				// Deserialize until we reach length limit
				while (count > 0) {
					// Extract length
					count -= VLQ.DecompressUnsigned(buffer, out var length);

					// Deserialize element
					var element = valueDecoder.DynamicInvoke(buffer, (Int32) length);
					count -= (Int32)length;

					// Add to output
					container.Add(element);
				}

				// Convert to array and return
				var output = Array.CreateInstance(typeof(T).GetElementType(), container.Count);
				container.CopyTo(output, 0);

				return output;
			};
		}

		private Func<IList, Node> GenerateListEncoder<T>() {
			// Get serializer for sub items
			var valueEncoder = GetEncoder(typeof(T).GenericTypeArguments[0]);

			return value => {
				// Handle nulls
				if (null == value) {
					return EmptyLeafNode;
				}

				// Serialize elements
				var childNodes = new NodeSet(value.Count);
				foreach (var element in value) {
					childNodes.Add((Node) valueEncoder.DynamicInvoke(element));
				}

				// Encode length
				var encodedLength = VLQ.CompressUnsigned((UInt64)childNodes.TotalLength).ToArray();

				return Node.NonLeaf(encodedLength, childNodes);
			};
		}

		private Func<Stream, Int32, T> GenerateListDecoder<T>() {
			// Get deserializer for sub items
			var valueDecoder = GetDecoder(typeof(T).GenericTypeArguments[0]);

			return (buffer, count) => {
				// Instantiate list
				var output = (IList) Activator.CreateInstance(typeof(T)); //typeof(List<>).MakeGenericType(type.GenericTypeArguments)

				// Deserialize until we reach length limit
				while (count > 0) {
					// Extract length
					count -= VLQ.DecompressUnsigned(buffer, out var length);

					// Deserialize element
					var element = valueDecoder.DynamicInvoke(buffer, (Int32) length);
					count -= (Int32)length;

					// Add to output
					output.Add(element);
				}

				return (T) output;
			};
		}

		private Func<IDictionary, Node> GenerateDictionaryEncoder<T>() {
			// Get serializer for sub items
			var keyEncoder = GetEncoder(typeof(T).GenericTypeArguments[0]);
			var valueEncoder = GetEncoder(typeof(T).GenericTypeArguments[1]);

			return value => {
				// Handle nulls
				if (null == value) {
					return EmptyLeafNode;
				}

				// Serialize elements   
				var childNodes = new NodeSet(value.Count * 2);
				var e = value.GetEnumerator();
				while (e.MoveNext()) {
					childNodes.Add((Node) keyEncoder.DynamicInvoke(e.Key));
					childNodes.Add((Node) valueEncoder.DynamicInvoke(e.Value));
				}

				// Encode length
				var encodedLength = VLQ.CompressUnsigned((UInt64)childNodes.TotalLength).ToArray();

				return Node.NonLeaf(encodedLength, childNodes);
			};
		}

		private Func<Stream, Int32, IDictionary> GenerateDictionaryDecoder<T>() {
			// Get deserializer for sub items
			var keyDecoder = GetDecoder(typeof(T).GenericTypeArguments[0]);
			var valueDecoder = GetDecoder(typeof(T).GenericTypeArguments[1]);

			return (buffer, count) => {
				// Instantiate dictionary
				var output = (IDictionary) Activator.CreateInstance(typeof(T));

				// Loop through input buffer until depleted
				while (count > 0) {
					// Deserialize key
					count -= VLQ.DecompressUnsigned(buffer, out var keyLength);
					var keyValue = keyDecoder.DynamicInvoke(buffer, (Int32)keyLength);
					count -= (Int32)keyLength;

					// Deserialize value
					count -= VLQ.DecompressUnsigned(buffer, out var valueLength);
					var valueValue = valueDecoder.DynamicInvoke(buffer, (Int32)valueLength);
					count -= (Int32)valueLength;

					// Add to output
					output[keyValue] = valueValue;
				}

				return output;
			};
		}

		private Func<T, Node> GeneratePOCOEncoder<T>() {
			// Find all properties decorated with LightWeightProperty attribute

			var fields = new FieldInfo[Byte.MaxValue]; // Index => Field
			var encoders = new Delegate[Byte.MaxValue]; // Index => Encoder
			var maxIndex = -1;
			foreach (var property in typeof(T).GetRuntimeFields()) {
				// Get property attribute which tells us the properties' index
				var attribute = (LightWeightPropertyAttribute) property.GetCustomAttribute(typeof(LightWeightPropertyAttribute), false);
				if (null == attribute) {
					// No attribute found, skip
					continue;
				}

				// Check for duplicate index
				if (null != fields[attribute.Index]) {
					throw new DuplicateIndexException($"The index {fields[attribute.Index]} is already used and cannot be reused.");
				}

				// Note the max index used
				if (attribute.Index > maxIndex) {
					maxIndex = attribute.Index;
				}

				// Find/create encoder
				var encoder = GetEncoder(property.FieldType);

				// Store property in lookup
				fields[attribute.Index] = property;
				encoders[attribute.Index] = encoder;
			}

			// If no properties, shortcut the whole thing and return a blank
			if (maxIndex == -1) {
				return value => EmptyLeafNode;
			}

			return value => {
				// Handle nulls
				if (null == value) {
					return EmptyLeafNode;
				}

				var childNodes = new NodeSet(maxIndex + 1);

				for (Byte i = 0; i <= maxIndex; i++) {
					var field = fields[i];
					var encoder = encoders[i];

					if (null == field) {
						childNodes.Add(EmptyLeafNode);
					} else {
						// Get the serializer for the sub-item
						var subType = GetEncoder(field.FieldType);

						// Get it's method info
						var subMethodInfo = subType.GetMethodInfo();

						// Extract value
						var v = field.GetValue(value);

						// Add to output
						childNodes.Add((Node) encoder.DynamicInvoke(v));
					}
				}

				// Encode length
				var encodedLength = VLQ.CompressUnsigned((UInt64)childNodes.TotalLength).ToArray();

				return Node.NonLeaf(encodedLength, childNodes);
			};
		}

		private Func<Stream, Int32, T> GeneratePOCODecoder<T>() {
			// Build vector of property types
			var fields = new FieldInfo[Byte.MaxValue];
			var decoders = new Delegate [Byte.MaxValue];
			foreach (var field in typeof(T).GetRuntimeFields()) {
				// TODO: Add property support
				// Get property attribute which tells us the properties' index
				var attribute = (LightWeightPropertyAttribute) field.GetCustomAttribute(typeof(LightWeightPropertyAttribute));

				// Skip if not found, or index doesn't match
				if (null == attribute) {
					// No attribute found, skip
					continue;
				}

				// Check for duplicate index
				if (null != fields[attribute.Index]) {
					throw new DuplicateIndexException($"The index {fields[attribute.Index]} is already used and cannot be reused.");
				}

				// Find/generate decoder
				var decoder = GetDecoder(field.FieldType);

				// Store values in lookup
				fields[attribute.Index] = field;
				decoders[attribute.Index] = decoder;
			}

			return (buffer, count) => {
				// Instantiate output
				var output = (T) Activator.CreateInstance(typeof(T));

				// Prepare for object deserialization
				var index = -1;

				// Attempt to read field length, if we've reached the end of the payload, abort
				while (count > 0) {
					// Get the length in a usable format
					count -= VLQ.DecompressUnsigned(buffer, out var length);

					// Increment the index
					index++;

					// Get field, and skip if it doesn't exist (it'll be ignored)
					var field = fields[index];
					if (null == field) {
						continue;
					}

					// Deserialize value
					var value = decoders[index].DynamicInvoke(buffer, (Int32)length);
					count -= (Int32)length;

					// Set it on property
					field.SetValue(output, value);
				}

				return output;
			};
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