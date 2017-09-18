using InvertedTomato.IO.Buffers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using InvertedTomato.Compression.Integers;
using System.Collections;
using InvertedTomato.Serialization.LightWeightSerialization.Coders;
using System.Reflection.Emit;

namespace InvertedTomato.Serialization.LightWeightSerialization {
    public class LightWeight : ISerializer {

        public static byte[] Serialize<T>(T value) {
            return Serialize(value, new LightWeightOptions());
        }
        public static byte[] Serialize<T>(T value, LightWeightOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException(nameof(options));
            }
#endif

            var buffer = new Buffer<byte>(options.DefaultInitialBufferSize);
            var lw = new LightWeight(options);
            lw.Serialize(value, buffer);
            return buffer.ToArray();

        }

        public static T Deserialize<T>(byte[] payload) {
            return Deserialize<T>(payload, new LightWeightOptions());
        }
        public static T Deserialize<T>(byte[] payload, LightWeightOptions options) {
            /*
#if DEBUG
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }
#endif

            var lw = new LightWeight(options);
            return (T)lw.Deserialize(new Buffer<byte>(payload), typeof(T));*/
            throw new NotImplementedException();
        }





        private readonly LightWeightOptions Options;
        private readonly Dictionary<Type, Delegate> SerilizationCoders = new Dictionary<Type, Delegate>();
        private readonly ModuleBuilder DynamicModule;
        private readonly VLQCodec VLQ = new VLQCodec();

        public LightWeight(LightWeightOptions options) {
#if DEBUG
            if (null == options) {
                throw new ArgumentNullException("options");
            }
#endif

            Options = options;

            // Load primative serilizers
            SerilizationCoders.Add(typeof(bool), (Action<bool, SerializationOutput>)BoolCoder.Serialize);

            SerilizationCoders.Add(typeof(sbyte), (Action<sbyte, SerializationOutput>)SInt8Coder.Serialize);
            SerilizationCoders.Add(typeof(short), (Action<short, SerializationOutput>)SInt16Coder.Serialize);
            SerilizationCoders.Add(typeof(int), (Action<int, SerializationOutput>)SInt32Coder.Serialize);
            SerilizationCoders.Add(typeof(long), (Action<long, SerializationOutput>)SInt64Coder.Serialize);

            SerilizationCoders.Add(typeof(byte), (Action<byte, SerializationOutput>)UInt8Coder.Serialize);
            SerilizationCoders.Add(typeof(ushort), (Action<ushort, SerializationOutput>)UInt16Coder.Serialize);
            SerilizationCoders.Add(typeof(uint), (Action<uint, SerializationOutput>)UInt32Coder.Serialize);
            SerilizationCoders.Add(typeof(ulong), (Action<ulong, SerializationOutput>)UInt64Coder.Serialize);

            SerilizationCoders.Add(typeof(string), (Action<string, SerializationOutput>)StringCoder.Serialize);

            // Prepare assembly for dynamic serilizers/deserilizers
            var dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicCoders"), AssemblyBuilderAccess.Run);
            DynamicModule = dynamicAssembly.DefineDynamicModule("DynamicCoders");

        }


        public void Serialize<T>(T value, Buffer<byte> buffer) {
#if DEBUG
            if (null == buffer) {
                throw new ArgumentNullException(nameof(buffer));
            }
#endif

            // Create output
            var output = new SerializationOutput();

            // Get root serilizer
            var rootSerilizer = (Action<T, SerializationOutput>)GetSerilizer<T>();

            // Invoke root serilizer
            rootSerilizer(value, output);

            // Write to provided buffer
            output.Generate(buffer);
        }




        public void BuildSerilizer<T>() {
            // TODO: Add IList support
            // TODO: Add IDictionary support

            // Create method
            var name = typeof(T).Name + "_Serilizer";
            var newType = DynamicModule.DefineType(name, TypeAttributes.Public);
            var newMethod = newType.DefineMethod(name, MethodAttributes.Static | MethodAttributes.Public, null, new Type[] { typeof(T), typeof(SerializationOutput) });

            // Add  IL
            var il = newMethod.GetILGenerator();

            il.DeclareLocal(typeof(int)); //index
            il.DeclareLocal(typeof(long)); //pre-length

            var notNullLabel = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Brtrue, notNullLabel);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldc_I4_S, 0x80);
            il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.AddRaw), new Type[] { typeof(byte) }));
            il.Emit(OpCodes.Ret);

            il.MarkLabel(notNullLabel);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.Allocate), new Type[] { }));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeProperty(nameof(SerializationOutput.Length)).GetMethod);
            il.Emit(OpCodes.Stloc_1);

            var properties = new Dictionary<byte, FieldInfo>();
            foreach (var itm in typeof(T).GetRuntimeFields()) {
                // Get property attribute which tells us the properties' index
                var attribute = (LightWeightPropertyAttribute)itm.GetCustomAttribute(typeof(LightWeightPropertyAttribute), false);
                if (null == attribute) {
                    // No attribute found, skip
                    continue;
                }

                properties.Add(attribute.Index, itm);

            }
            if (properties.Count > 0) {
                var maxval = properties.Keys.Max();
                for (byte i = 0; i <= maxval; i++) {
                    if (properties.TryGetValue(i, out var itm)) {
                        // Call EnsureSerilizer generically
                        var method = typeof(LightWeight).GetRuntimeMethods().Single(a => a.Name == nameof(GetSerilizer)).MakeGenericMethod(itm.FieldType); // "typeof(LightWeight).GetRuntimeMethod(nameof(EnsureSerializer), new Type[] { })" returns NULL for generics - why?
                        var subtype = (Delegate)method.Invoke(this, null);

                        // Get methodinfo
                        var submethinfo = subtype.GetMethodInfo();

                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, itm);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, submethinfo);
                    } else {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldc_I4_S, 0x80);
                        il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.AddRaw), new Type[] { typeof(byte) }));
                    }
                }
            }

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, typeof(SerializationOutput).GetRuntimeProperty(nameof(SerializationOutput.Length)).GetMethod);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Sub);
            il.Emit(OpCodes.Call, typeof(SerializationOutput).GetRuntimeMethod(nameof(SerializationOutput.SetVLQ), new Type[] { typeof(int), typeof(ulong) }));
            il.Emit(OpCodes.Ret);

            // Add to serilizers
            var methodInfo = newType.CreateTypeInfo().GetMethod(name);
            SerilizationCoders[typeof(T)] = (Action<T, SerializationOutput>)methodInfo.CreateDelegate(typeof(Action<T, SerializationOutput>));
        }



        private Delegate GetSerilizer<T>() {
            // If there's no coder, build one
            if (!SerilizationCoders.TryGetValue(typeof(T), out var serilizer)) {
                BuildSerilizer<T>();
                serilizer = SerilizationCoders[typeof(T)];
            }

            // Return coder
            return serilizer;
        }


        public T Deserialize<T>(Buffer<byte> input) {
            /*
                if (null == payload) {
                    throw new ArgumentNullException("payload");
                }
                if (null == type) {
                    throw new ArgumentNullException("type");
                }



                if (type.IsArray) {
                    return DeserializeArray(type.GetElementType(), payload);
                }
                if (typeof(IList).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())) {
                    return DeserializeList(type.GenericTypeArguments[0], payload);
                }
                if (typeof(IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo())) {
                    return DeserializeDictionary(type.GenericTypeArguments[0], type.GenericTypeArguments[1], payload);
                }

                return DeserializePOCO(type, payload);*/

            throw new NotImplementedException();

        }
    }
}
