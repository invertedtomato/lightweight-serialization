using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	public class LightWeight : ISerializer {
		private readonly Dictionary<Type, Delegate> Decoders = new Dictionary<Type, Delegate>(); // Func<TOut, T>
		private readonly Dictionary<Type, Delegate> Encoders = new Dictionary<Type, Delegate>(); // Func<T, TIn>
		private readonly Object Sync = new Object();
		private readonly VLQCodec VLQ = new VLQCodec();

		public LightWeight() {
		}

		public void Encode<T>(T value, Buffer<Byte> buffer) {
#if DEBUG
			if (null == buffer) {
				throw new ArgumentNullException(nameof(buffer));
			}
#endif

			// Get root serializer
			var rootSerializer = (Func<T, ScatterTreeBuffer>) GetEncoder<T>();

			// Invoke root serializer
			var output = rootSerializer(value);

			// Grow buffer with sufficient room
			buffer.Grow(Math.Max(0, output.Length + output.Count * 10 - buffer.Writable));

			// Squash scatter tree into buffer
			Squash(output, buffer);
		}

		public T Decode<T>(Buffer<Byte> input) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}
#endif

			// Get root serializer
			var root = GetDecoder<T>();

			// Invoke root serializer
			return (T) root.DynamicInvoke(input);
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

		private void Squash(ScatterTreeBuffer output, Buffer<Byte> buffer) {
			if (output.Payload != null) {
				buffer.EnqueueArray(output.Payload);
			} else {
				foreach (var child in output.Children) {
					VLQ.CompressUnsigned((UInt64) child.Length, buffer);
					Squash(child, buffer);
				}
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

		private Func<Boolean, ScatterTreeBuffer> GenerateBoolEncoder() {
			return value => {
				if (!value) {
					// TODO: Shouldn't this be? return ScatterTreeBuffer.Empty;
					return new ScatterTreeBuffer(new Byte[] { });
				}

				return new ScatterTreeBuffer(new Byte[] {0x00});
			};
		}

		private Func<Buffer<Byte>, Boolean> GenerateBoolDecoder() {
			return buffer => {
				if (buffer.Readable == 0) {
					return false;
				}
#if DEBUG
				if (buffer.Readable > 1) {
					throw new DataFormatException("Boolean values can be no more than 1 byte long.");
				}
#endif
				if (buffer.Dequeue() != 0x00) {
					throw new DataFormatException("Boolean values cannot be anything other than 0x00.");
				}

				return true;
			};
		}

		private Func<SByte, ScatterTreeBuffer> GenerateSInt8Encoder() {
			return value => {
				if (value == 0) {
					return ScatterTreeBuffer.Empty;
				}

				return new ScatterTreeBuffer(new[] {(Byte) value});
			};
		}

		private Func<Buffer<Byte>, SByte> GenerateSInt8Decoder() {
			return buffer => {
				switch (buffer.Readable) {
					case 0: return 0;
					case 1: return (SByte) buffer.Dequeue();
					default: throw new DataFormatException("SInt64 values can be 0 or 1 bytes.");
				}
			};
		}

		private Func<Int16, ScatterTreeBuffer> GenerateSInt16Encoder() {
			var smaller = GenerateSInt8Encoder();
			return value => {
				if (value <= SByte.MaxValue && value >= SByte.MinValue) {
					return smaller((SByte) value);
				}

				return new ScatterTreeBuffer(BitConverter.GetBytes(value));
			};
		}

		private Func<Buffer<Byte>, Int16> GenerateSInt16Decoder() {
			return buffer => {
				switch (buffer.Readable) {
					case 0: return 0;
					case 1: return buffer.Dequeue();
					case 2: return BitConverter.ToInt16(buffer.GetUnderlying(), buffer.Start);
					default: throw new DataFormatException("SInt64 values can be 0, 1 or 2 bytes.");
				}
			};
		}

		private Func<Int32, ScatterTreeBuffer> GenerateSInt32Encoder() {
			var smaller = GenerateSInt16Encoder();
			return value => {
				if (value <= Int16.MaxValue && value >= Int16.MinValue) {
					return smaller((Int16) value);
				}

				// TODO: 3byte encoding
				return new ScatterTreeBuffer(BitConverter.GetBytes(value));
			};
		}

		private Func<Buffer<Byte>, Int32> GenerateSInt32Decoder() {
			return buffer => {
				switch (buffer.Readable) {
					case 0: return 0;
					case 1: return buffer.Dequeue();
					case 2: return BitConverter.ToInt16(buffer.GetUnderlying(), buffer.Start);
					// TODO: 3byte decoding
					case 4: return BitConverter.ToInt32(buffer.GetUnderlying(), buffer.Start);
					default: throw new DataFormatException("SInt32 values can be 0, 1, 2 or 4 bytes.");
				}
			};
		}

		private Func<Int64, ScatterTreeBuffer> GenerateSInt64Encoder() {
			var smaller = GenerateSInt32Encoder();
			return value => {
				if (value <= Int32.MaxValue && value >= Int32.MinValue) {
					return smaller((Int32) value);
				}

				// TODO: 5, 6, 7 byte encoding
				return new ScatterTreeBuffer(BitConverter.GetBytes(value));
			};
		}

		private Func<Buffer<Byte>, Int64> GenerateSInt64Decoder() {
			return buffer => {
				switch (buffer.Readable) {
					case 0: return 0;
					case 1: return buffer.Dequeue();
					case 2: return BitConverter.ToInt16(buffer.GetUnderlying(), buffer.Start);
					// TODO: 3byte decoding
					case 4: return BitConverter.ToInt32(buffer.GetUnderlying(), buffer.Start);
					// TODO 5,6,7byte decoding
					case 8: return BitConverter.ToInt64(buffer.GetUnderlying(), buffer.Start);
					default: throw new DataFormatException("SInt64 values can be 0, 1, 2, 4 or 8 bytes.");
				}
			};
		}

		private Func<Byte, ScatterTreeBuffer> GenerateUInt8Encoder() {
			return value => {
				if (value == 0) {
					return ScatterTreeBuffer.Empty;
				}

				return new ScatterTreeBuffer(new[] {value});
			};
		}

		private Func<Buffer<Byte>, Byte> GenerateUInt8Decoder() {
			return buffer => {
				switch (buffer.Readable) {
					case 0: return 0;
					case 1: return buffer.Dequeue();
					default: throw new DataFormatException("UInt64 values can be 0 or 1 bytes.");
				}
			};
		}

		private Func<UInt16, ScatterTreeBuffer> GenerateUInt16Encoder() {
			var smaller = GenerateUInt8Encoder();
			return value => {
				if (value <= Byte.MaxValue) {
					return smaller((Byte) value);
				}

				return new ScatterTreeBuffer(BitConverter.GetBytes(value));
			};
		}

		private Func<Buffer<Byte>, UInt16> GenerateUInt16Decoder() {
			return buffer => {
				switch (buffer.Readable) {
					case 0: return 0;
					case 1: return buffer.Dequeue();
					case 2: return BitConverter.ToUInt16(buffer.GetUnderlying(), buffer.Start);
					default: throw new DataFormatException("UInt64 values can be 0, 1 or 2 bytes.");
				}
			};
		}

		private Func<UInt32, ScatterTreeBuffer> GenerateUInt32Encoder() {
			var smaller = GenerateUInt16Encoder();
			return value => {
				if (value <= UInt16.MaxValue) {
					return smaller((UInt16) value);
				}

				// TODO: 3byte encoding
				return new ScatterTreeBuffer(BitConverter.GetBytes(value));
			};
		}

		private Func<Buffer<Byte>, UInt32> GenerateUInt32Decoder() {
			return buffer => {
				switch (buffer.Readable) {
					case 0: return 0;
					case 1: return buffer.Dequeue();
					case 2: return BitConverter.ToUInt16(buffer.GetUnderlying(), buffer.Start);
					// TODO: 3byte decoding
					case 4: return BitConverter.ToUInt32(buffer.GetUnderlying(), buffer.Start);
					default: throw new DataFormatException("UInt32 values can be 0, 1, 2 or 4 bytes.");
				}
			};
		}

		private Func<UInt64, ScatterTreeBuffer> GenerateUInt64Encoder() {
			var smaller = GenerateUInt32Encoder();
			return value => {
				if (value <= UInt32.MaxValue) {
					return smaller((UInt32) value);
				}

				// TODO: 5, 6, 7 byte encoding
				return new ScatterTreeBuffer(BitConverter.GetBytes(value));
			};
		}

		private Func<Buffer<Byte>, UInt64> GenerateUInt64Decoder() {
			return buffer => {
				switch (buffer.Readable) {
					case 0: return 0;
					case 1: return buffer.Dequeue();
					case 2: return BitConverter.ToUInt16(buffer.GetUnderlying(), buffer.Start);
					// TODO: 3byte decoding
					case 4: return BitConverter.ToUInt32(buffer.GetUnderlying(), buffer.Start);
					// TODO 5,6,7byte decoding
					case 8: return BitConverter.ToUInt64(buffer.GetUnderlying(), buffer.Start);
					default: throw new DataFormatException("UInt64 values can be 0, 1, 2, 4 or 8 bytes.");
				}
			};
		}

		private Func<String, ScatterTreeBuffer> GenerateStringEncoder() {
			return value => {
				if (null == value) {
					return ScatterTreeBuffer.Empty;
				}

				return new ScatterTreeBuffer(Encoding.UTF8.GetBytes(value));
			};
		}

		private Func<Buffer<Byte>, String> GenerateStringDecoder() {
			return buffer => { return Encoding.UTF8.GetString(buffer.GetUnderlying(), buffer.Start, buffer.Readable); };
		}

		private Func<Array, ScatterTreeBuffer> GenerateArrayEncoder<T>() {
			// Get serializer for sub items
			var valueEncoder = GetEncoder(typeof(T).GetElementType());

			return value => {
				// Handle nulls
				if (null == value) {
					return ScatterTreeBuffer.Empty;
				}

				// Serialize elements
				var pos = 0;
				var result = new ScatterTreeBuffer[value.Length];
				foreach (var element in value) {
					result[pos++] = (ScatterTreeBuffer) valueEncoder.DynamicInvoke(element);
				}

				return new ScatterTreeBuffer(result);
			};
		}

		private Func<Buffer<Byte>, Array> GenerateArrayDecoder<T>() {
			// Get deserializer for sub items
			var valueDecoder = GetDecoder(typeof(T).GetElementType());

			return buffer => {
				// Instantiate list
				var container = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(typeof(T).GetElementType()));

				// Deserialize until we reach length limit
				while (buffer.IsReadable) {
					// Extract length
					var length = (Int32) VLQ.DecompressUnsigned(buffer);

					// Extract sub-buffer
					var subBuffer = buffer.DequeueBuffer(length);

					// Deserialize element
					var element = valueDecoder.DynamicInvoke(subBuffer);

					// Add to output
					container.Add(element);
				}

				// Convert to array and return
				var output = Array.CreateInstance(typeof(T).GetElementType(), container.Count);
				container.CopyTo(output, 0);

				return output;
			};
		}

		private Func<IList, ScatterTreeBuffer> GenerateListEncoder<T>() {
			// Get serializer for sub items
			var valueEncoder = GetEncoder(typeof(T).GenericTypeArguments[0]);

			return value => {
				// Handle nulls
				if (null == value) {
					return ScatterTreeBuffer.Empty;
				}

				// Serialize elements
				var pos = 0;
				var result = new ScatterTreeBuffer[value.Count];
				foreach (var element in value) {
					result[pos++] = (ScatterTreeBuffer) valueEncoder.DynamicInvoke(element);
				}

				return new ScatterTreeBuffer(result);
			};
		}

		private Func<Buffer<Byte>, T> GenerateListDecoder<T>() {
			// Get deserializer for sub items
			var valueDecoder = GetDecoder(typeof(T).GenericTypeArguments[0]);

			return buffer => {
				// Instantiate list
				var output = (IList) Activator.CreateInstance(typeof(T)); //typeof(List<>).MakeGenericType(type.GenericTypeArguments)

				// Deserialize until we reach length limit
				while (buffer.IsReadable) {
					// Extract length
					var length = (Int32) VLQ.DecompressUnsigned(buffer);

					// Extract subbuffer
					var subBuffer = buffer.DequeueBuffer(length);

					// Deserialize element
					var element = valueDecoder.DynamicInvoke(subBuffer);

					// Add to output
					output.Add(element);
				}

				return (T) output;
			};
		}

		private Func<IDictionary, ScatterTreeBuffer> GenerateDictionaryEncoder<T>() {
			// Get serializer for sub items
			var keyEncoder = GetEncoder(typeof(T).GenericTypeArguments[0]);
			var valueEncoder = GetEncoder(typeof(T).GenericTypeArguments[1]);

			return value => {
				// Handle nulls
				if (null == value) {
					return ScatterTreeBuffer.Empty;
				}

				// Serialize elements   
				var pos = 0;
				var result = new ScatterTreeBuffer[value.Count * 2];
				var e = value.GetEnumerator();
				while (e.MoveNext()) {
					result[pos++] = (ScatterTreeBuffer) keyEncoder.DynamicInvoke(e.Key);
					result[pos++] = (ScatterTreeBuffer) valueEncoder.DynamicInvoke(e.Value);
				}

				return new ScatterTreeBuffer(result);
			};
		}

		private Func<Buffer<Byte>, IDictionary> GenerateDictionaryDecoder<T>() {
			// Get deserializer for sub items
			var keyDecoder = GetDecoder(typeof(T).GenericTypeArguments[0]);
			var valueDecoder = GetDecoder(typeof(T).GenericTypeArguments[1]);

			return buffer => {
				// Instantiate dictionary
				var output = (IDictionary) Activator.CreateInstance(typeof(T));

				// Loop through input buffer until depleted
				while (buffer.IsReadable) {
					// Deserialize key
					var keyLength = (Int32) VLQ.DecompressUnsigned(buffer);
					var keyBuffer = buffer.DequeueBuffer(keyLength);
					var keyValue = keyDecoder.DynamicInvoke(keyBuffer);

					// Deserialize value
					var valueLength = (Int32) VLQ.DecompressUnsigned(buffer);
					var valueBuffer = buffer.DequeueBuffer(valueLength);
					var valueValue = valueDecoder.DynamicInvoke(valueBuffer);

					// Add to output
					output[keyValue] = valueValue;
				}

				return output;
			};
		}

		private Func<T, ScatterTreeBuffer> GeneratePOCOEncoder<T>() {
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
				return value => ScatterTreeBuffer.Empty;
			}

			return value => {
				// Handle nulls
				if (null == value) {
					return ScatterTreeBuffer.Empty;
				}

				var result = new ScatterTreeBuffer[maxIndex + 1];

				for (Byte i = 0; i <= maxIndex; i++) {
					var field = fields[i];
					var encoder = encoders[i];

					if (null == field) {
						result[i] = ScatterTreeBuffer.Empty;
					} else {
						// Get the serializer for the sub-item
						var subType = GetEncoder(field.FieldType);

						// Get it's method info
						var subMethodInfo = subType.GetMethodInfo();

						// Extract value
						var v = field.GetValue(value);

						// Add to output
						result[i] = (ScatterTreeBuffer) encoder.DynamicInvoke(v);
					}
				}

				return new ScatterTreeBuffer(result);
			};

			/*
			// Create method
			var name = typeof(T).Name + "_Serializer";
			var newType = DynamicModule.DefineType(name, TypeAttributes.Public);
			var newMethod = newType.DefineMethod(name, MethodAttributes.Static | MethodAttributes.Public, typeof(ScatterTreeBuffer), new[] {typeof(T)});

			// Add  IL
			var il = newMethod.GetILGenerator();

			il.DeclareLocal(typeof(Int32)); //index
			il.DeclareLocal(typeof(Int64)); //pre-length

			var notNullLabel = il.DefineLabel();
			il.Emit(OpCodes.Ldarg_0); // value
			il.Emit(OpCodes.Brtrue, notNullLabel);
			il.Emit(OpCodes.Ldarg_1); // output
			il.Emit(OpCodes.Ldc_I4_S, 0x80);
			//il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.AddRaw), new Type[] { typeof(byte) }));
			il.Emit(OpCodes.Ret);

			il.MarkLabel(notNullLabel);
			il.Emit(OpCodes.Ldarg_1); // output
			//il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.Allocate), new Type[] { }));
			il.Emit(OpCodes.Stloc_0);
			il.Emit(OpCodes.Ldarg_1); // output
			//il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeProperty(nameof(SerializationOutput.Length)).GetMethod);
			il.Emit(OpCodes.Stloc_1);

			var properties = new Dictionary<Byte, FieldInfo>();
			foreach (var itm in typeof(T).GetRuntimeFields()) {
				// Get property attribute which tells us the properties' index
				var attribute = (LightWeightPropertyAttribute) itm.GetCustomAttribute(typeof(LightWeightPropertyAttribute), false);
				if (null == attribute) {
					// No attribute found, skip
					continue;
				}

				properties.Add(attribute.Index, itm);
			}

			if (properties.Count > 0) {
				var maxValue = properties.Keys.Max();
				for (Byte i = 0; i <= maxValue; i++) {
					if (properties.TryGetValue(i, out var itm)) {
						// Get the serializer for the sub-item
						var subType = GetEncoder(itm.FieldType);

						// Get it's method info
						var subMethodInfo = subType.GetMethodInfo();

						il.Emit(OpCodes.Ldarg_0); // value
						il.Emit(OpCodes.Ldfld, itm);
						il.Emit(OpCodes.Ldarg_1); // output
						il.Emit(OpCodes.Call, subMethodInfo);
					} else {
						//output.AddRaw(0x80);
						il.Emit(OpCodes.Ldarg_1); // P1: output
						il.Emit(OpCodes.Ldc_I4_S, 0x80); // P2: 0x80
						//il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.AddRaw), new Type[] { typeof(byte) }));
					}
				}
			}

			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Ldloc_0);
			il.Emit(OpCodes.Ldarg_1);
			//il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeProperty(nameof(SerializationOutput.Length)).GetMethod);
			il.Emit(OpCodes.Ldloc_1);
			il.Emit(OpCodes.Sub);
			//il.Emit(OpCodes.Call, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.SetVLQ), new Type[] { typeof(int), typeof(ulong) }));
			il.Emit(OpCodes.Ret);

			// Add to serializers
			var methodInfo = newType.CreateTypeInfo().GetMethod(name);
			return (Func<T, ScatterTreeBuffer>) methodInfo.CreateDelegate(typeof(Func<T, ScatterTreeBuffer>));
			*/
		}

		private Func<Buffer<Byte>, T> GeneratePOCODecoder<T>() {
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

			return buffer => {
				// Instantiate output
				var output = (T) Activator.CreateInstance(typeof(T));

				// Prepare for object deserialization
				var index = -1;

				// Attempt to read field length, if we've reached the end of the payload, abort
				while (buffer.IsReadable) {
					// Get the length in a usable format
					var length = (Int32) VLQ.DecompressUnsigned(buffer);
					var subBuffer = buffer.DequeueBuffer(length);

					// Increment the index
					index++;

					// Get field, and skip if it doesn't exist (it'll be ignored)
					var field = fields[index];
					if (null == field) {
						continue;
					}

					// Deserialize value
					var value = decoders[index].DynamicInvoke(subBuffer);

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
			var buffer = new Buffer<Byte>(0);
			var lw = new LightWeight();
			lw.Encode(value, buffer);
			return buffer.ToArray();
		}

		/// <summary>
		/// Deserialize an object from a byte array.
		/// </summary>
		public static T Deserialize<T>(Byte[] payload) {
			var buffer = new Buffer<Byte>(payload);
			var lw = new LightWeight();
			return lw.Decode<T>(buffer);
		}
	}
}