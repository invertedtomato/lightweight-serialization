# LightWeight
## Introduction
_LightWeight is a work in progress and is by no means stable currently._

LightWeight takes a different approach to serialization than most mainstream algorithms. It focuses on producing the smallest
possible output (even at the expense of functionality). The current design outperforms JSON and ProtoBuf.

## How do I make it go?
```c#
static void Main(String[] args) {
    var input = new POCO {
        Cake = true,
        Vegetable = false,
        Sub = new SubPOCO {
            SubValue = true
        }
    };
    
    var output = LightWeight.Serialize(input);

    foreach (var c in output) {
        Console.WriteLine(c);
    }

    Console.WriteLine("Done");
    Console.ReadKey();
}   

public class POCO {
    [LightWeightProperty(0)] 
    public Boolean Cake;

    public Boolean Ignored;

    [LightWeightProperty(2)] 
    public SubPOCO Sub;

    [LightWeightProperty(1)] 
    public Boolean Vegetable;
}

public class SubPOCO {
    [LightWeightProperty(2)] public Boolean SubValue;
}
```