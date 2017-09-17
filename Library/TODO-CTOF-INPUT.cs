using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

namespace ConsoleApp3 {
    public class POCO {
        [LightWeightProperty(0)]
        public bool Cake;
        [LightWeightProperty(1)]
        public bool Vegetable;
        [LightWeightProperty(2)]
        public SubPOCO Sub;

        public bool Ignored;
    }
    public class SubPOCO {
        [LightWeightProperty(2)]
        public bool SubValue;
    }

    public class Program {
        /*
        static void Main(string[] args) {
            var a = new Program();


            var b = new POCO() {
                Cake = true,
                Vegetable = false,
                Sub = new SubPOCO() {
                    SubValue = true
                }
            };

            var ret = a.Serialize(b);

            foreach (var c in ret) {
                Console.WriteLine(c);
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }
        */

        private Dictionary<Type, Delegate> Serializers = new Dictionary<Type, Delegate>();

        private readonly ModuleBuilder module;
        public Program() {
            // Load primative coders
            Serializers.Add(typeof(bool), (Action<bool, SerializationBuffer>)BoolCoder.Serialize);

            // Prepare assembly for POCO coders
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("DynamicCoders"), AssemblyBuilderAccess.Run);
            module = assembly.DefineDynamicModule("DynamicCoders");
        }

        public byte[] Serialize<T>(T value) {
            var buffer = new SerializationBuffer();
            Serialize(value, buffer);
            return buffer.ToArray();
        }
        public void Serialize<T>(T value, SerializationBuffer buffer) {
            var serilizer = EnsureSerializer<T>();

            var typed = (Action<T, SerializationBuffer>)serilizer;
            typed(value, buffer);
        }

        private Delegate EnsureSerializer<T>() {
            var t = typeof(T);

            if (!Serializers.TryGetValue(t, out var serilizer)) {
                serilizer = Serializers[t] = CreateSerilizer<T>();
            }

            return serilizer;
        }
        private Delegate EnsureSerializer2(Type t) {
            var n = typeof(Program).GetRuntimeMethod(nameof(EnsureSerializer), new Type[] { }); //, BindingFlags.NonPublic | BindingFlags.Instance
            return (Delegate)n.MakeGenericMethod(t).Invoke(this, null);
        }

        private Action<T, SerializationBuffer> CreateSerilizer<T>() {
            // Create type
            var newType = module.DefineType(typeof(T).ToString(), TypeAttributes.Public);

            // Create method
            var newMethod = newType.DefineMethod(typeof(T).ToString(), MethodAttributes.Static | MethodAttributes.Public, null, new Type[] { typeof(T), typeof(SerializationBuffer) });

            // Create IL
            var il = newMethod.GetILGenerator();
            //il.DeclareLocal(typeof(Buffer<Byte>));

            il.DeclareLocal(typeof(int)); //index
            il.DeclareLocal(typeof(long)); //pre-length

            var isnotnull = il.DefineLabel();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Brtrue, isnotnull);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldc_I4_S, 0x80);
            il.Emit(OpCodes.Callvirt, typeof(SerializationBuffer).GetRuntimeMethod(nameof(SerializationBuffer.Add), new Type[] { }));
            il.Emit(OpCodes.Ret);

            il.MarkLabel(isnotnull);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, typeof(SerializationBuffer).GetRuntimeMethod(nameof(SerializationBuffer.Allocate), new Type[] { }));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, typeof(SerializationBuffer).GetRuntimeProperty(nameof(SerializationBuffer.TotalLength)).GetMethod);
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
                    if (!properties.TryGetValue(i, out var itm)) {
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Ldc_I4_S, 0x80);
                        il.Emit(OpCodes.Callvirt, typeof(SerializationBuffer).GetRuntimeMethod(nameof(SerializationBuffer.Add), new Type[] { }));
                    } else {
                        var subtype = EnsureSerializer2(itm.FieldType);
                        var submethinfo = subtype.GetMethodInfo();

                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Ldfld, itm);
                        il.Emit(OpCodes.Ldarg_1);
                        il.Emit(OpCodes.Call, submethinfo);
                    }
                }
            }

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Callvirt, typeof(SerializationBuffer).GetRuntimeProperty(nameof(SerializationBuffer.TotalLength)).GetMethod);
            il.Emit(OpCodes.Ldloc_1);
            il.Emit(OpCodes.Sub);
            il.Emit(OpCodes.Call, typeof(SerializationBuffer).GetRuntimeMethod(nameof(SerializationBuffer.SetVLQ), new Type[] { }));
            il.Emit(OpCodes.Ret);

            var methodInfo = newType.CreateTypeInfo().GetMethod(typeof(T).ToString());

            return (Action<T, SerializationBuffer>)methodInfo.CreateDelegate(typeof(Action<T, SerializationBuffer>));
        }
    }


    public static class BoolCoder {
        public static void Serialize(bool value, SerializationBuffer buffer) {
            if (value) {
                buffer.AddArray(new byte[] { 0x81, 0xff });
            } else {
                buffer.Add(0x80);
            }
        }

        public static object Deserialize(Type type, Buffer<byte> buffer) {
            throw new NotImplementedException();
        }
    }


    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class LightWeightPropertyAttribute : Attribute {
        public LightWeightPropertyAttribute(byte index) {
            Index = index;
        }

        public byte Index { get; private set; }
    }


    public class SerializationBuffer {
        private static readonly VLQCodec VLQ = new VLQCodec();
        private readonly List<byte[]> Underlying = new List<byte[]>();

        public long TotalLength { get; private set; }

        public int Allocate() {
            Underlying.Add(null);
            return Underlying.Count - 1;
        }

        public void SetVLQ(int index, ulong data) {
#if DEBUG
            if (Underlying[index] != null)
                throw new Exception("Bad");
#endif
            var buffer = new Buffer<byte>(10);
            VLQ.CompressUnsigned(data, buffer);
            var array = buffer.ToArray();
            Underlying[index] = array;
            TotalLength += array.Length;
        }

        public void AddArray(byte[] value) {
            Underlying.Add(value);
            TotalLength += value.Length;
        }

        public void Add(byte value) {
            Underlying.Add(new byte[] { value });
            TotalLength++;
        }

        public byte[] ToArray() {
            var output = new byte[TotalLength];
            var pos = 0;

            foreach (var a in Underlying) {
                Buffer.BlockCopy(a, 0, output, pos, a.Length);
                pos += a.Length;
            }

            return output;
        }
    }
}
