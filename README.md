# LightWeight
## Introduction
LightWeight is a fresh take on serialization. By including only the bare minimum bytes it produces outputs smaller than ProtoBuff and MsgPack. This 
small output size is achieved using similar CPU overheads.

## How do I make it go?
```c#
private static void Main(String[] args) {
    // Figure out what you want to serialize
    var input = new POCO {
        Cake = true,
        Vegetable = false,
        Sub = new SubPOCO {
            SubValue = true
        }
    };

    // Serialize it!
    var payload = LightWeight.Serialize(input);

    // Store or transmit the binary output
    Console.WriteLine(BitConverter.ToString(payload));

    // Deserialise it
    var output = LightWeight.Deserialize<POCO>(payload);
}

public class POCO {
    [LightWeightProperty(0)] // Every property to be serialized is decorated with this attribute
    public Boolean Cake;

    public Boolean Ignored; // If it has no attribute it will be ignored

    [LightWeightProperty(2)] // The index is used to identify the field, not the name
    public SubPOCO Sub;

    [LightWeightProperty(1)] // Start with index of 0 and increment for each additional field (skipping number is fine)
    public Boolean Vegetable;
}

public class SubPOCO {
    [LightWeightProperty(0)]
    public Boolean SubValue;
}
```

## What is the protocol?
Have a look a the [protocol documentation](https://github.com/invertedtomato/lightweight-serialization/blob/master/PROTOCOL.md).