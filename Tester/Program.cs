using System;

namespace InvertedTomato.Serialization.LightWeightSerialization {
	internal class Program {
		private static void Main(String[] args) {
			var b = new POCO {
				Cake = true,
				Vegetable = false,
				Sub = new SubPOCO {
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
		[LightWeightProperty(0)] public Boolean Cake;

		public Boolean Ignored;

		[LightWeightProperty(2)] public SubPOCO Sub;

		[LightWeightProperty(1)] public Boolean Vegetable;
	}

	public class SubPOCO {
		[LightWeightProperty(2)] public Boolean SubValue;
	}
}