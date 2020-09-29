using System;
using System.IO;

namespace InvertedTomato.Serialization.LightWeightSerialization.TransformationAttributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FloorTransformationAttribute : TransformationAttribute
    {
        public FloorTransformationAttribute(Int64 floor)
        {
            Floor = floor;
        }

        public Int64 Floor { get; }

        public override Object ApplyEffect(Object input)
        {
            if (input is UInt64)
            {
                return (UInt64)input - (UInt64)Floor;
            }
            if (input is UInt32)
            {
                return (UInt32)input - (UInt32)Floor;
            }
            if (input is UInt16)
            {
                return (UInt16)((UInt16)input - (UInt16)Floor);
            }
            if (input is Byte)
            {
                return (Byte)((Byte)input - (Byte)Floor);
            }

            if (input is Int64)
            {
                return (Int64)input - (Int64)Floor;
            }
            if (input is Int32)
            {
                return (Int32)input - (Int32)Floor;
            }
            if (input is Int16)
            {
                return (Int16)((Int16)input - (Int16)Floor);
            }
            if (input is SByte)
            {
                return (SByte)((SByte)input - (SByte)Floor);
            }

            throw new UnsupportedDataTypeException($"{nameof(FloorTransformationAttribute)} does not support {input.GetType()}.");
        }

        public override Object RemoveEffect(Object input)
        {
            if (input is UInt64)
            {
                return (UInt64)input + (UInt64)Floor;
            }
            if (input is UInt32)
            {
                return (UInt32)input + (UInt32)Floor;
            }
            if (input is UInt16)
            {
                return (UInt16)((UInt16)input + (UInt16)Floor);
            }
            if (input is Byte)
            {
                return (Byte)((Byte)input + (Byte)Floor);
            }

            if (input is Int64)
            {
                return (Int64)input + (Int64)Floor;
            }
            if (input is Int32)
            {
                return (Int32)input + (Int32)Floor;
            }
            if (input is Int16)
            {
                return (Int16)((Int16)input + (Int16)Floor);
            }
            if (input is SByte)
            {
                return (SByte)((SByte)input + (SByte)Floor);
            }

            throw new UnsupportedDataTypeException($"{nameof(FloorTransformationAttribute)} does not support {input.GetType()}.");
        }
    }
}