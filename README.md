# LightWeight
## Introduction
Do you need your structured data to be as small as possible for storage or transport? Perhaps you're capturing stacks of data for archiving, or you have
a high performance message bus that is being overwealmed. LightWeight is a serialization algorithm just for this purpose. It packages data using barely any overheals, and with supprisingly little CPU. 

LightWeight outperforms both Google ProtoBuff and MsgPack in terms of file size, with similar performance in the libraries tested.

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