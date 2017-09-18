using System;

namespace InvertedTomato.Serialization.LightWeightSerialization {
    class Program {
        static void Main(string[] args) {
            var b = new POCO() {
                Cake = true,
                Vegetable = false,
                Sub = new SubPOCO() {
                    SubValue = true
                }
            };

            var ret = LightWeight.Serialize(b);

            foreach (var c in ret) {
                Console.WriteLine(c);
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }

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


}

