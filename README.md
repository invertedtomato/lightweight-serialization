# LightWeight
## Introduction
_LightWeight is a work in progress and is by no means stable currently._

LightWeight takes a different approach to serialization than most mainstream algorithms. It focuses on producing the smallest
possible output (even at the expense of functionality). The current design outperforms JSON and ProtoBuf.

## How do I make it go?
```c#
static void Main(String[] args) {
    // Create your object
    var input = new POCO {
        Cake = true,
        Vegetable = false,
        Sub = new SubPOCO {
            SubValue = true
        }
    };

    // Serialize like you normally would
    var payload = LightWeight.Serialize(input);

    // Use the outputted byte array
    Console.WriteLine(BitConverter.ToString(payload));

    // Deserialize like you normally would
    var output = LightWeight.Deserialize<POCO>(payload);
}

public class POCO {
    [LightWeightProperty(0)] // Every property to be serialized is decorated with this attribute
    public Boolean Cake;

    public Boolean Ignored; // If it has no attribute it will be ignored

    [LightWeightProperty(2)] // The index is used to identify the field, not the name
    public SubPOCO Sub;

    [LightWeightProperty(1)] // For best performance, start with index 0 and increment, though they don't need to be in order
    public Boolean Vegetable;
}

public class SubPOCO {
    [LightWeightProperty(2)] // Skipping indexes like this may waste bits, but does work
    public Boolean SubValue;
}
```

## What is the protocol?
Have a look a the [protocol documentation](https://github.com/invertedtomato/lightweight-serialization/blob/master/PROTOCOL.md).