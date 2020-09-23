using System;
using System.Linq;

namespace Tests
{
    public static class Extensions
    {
        public static Byte[] ParseAsHex(this String target)
        {
            return Enumerable.Range(0, target.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(target.Substring(x, 2), 16))
                .ToArray();
        }
        public static String ToHexString(this UInt16 target, Int32 minBytes = 0)
        {
            var output = target.ToString("X").Replace("-", "");
            output = new String('0', Math.Max(0, minBytes * 2 - output.Length)) + output;
            return output;
        }

        public static String ToHexString(this UInt32 target, Int32 minBytes = 0)
        {
            var output = target.ToString("X").Replace("-", "");
            output = new String('0', Math.Max(0, minBytes * 2 - output.Length)) + output;
            return output;
        }

        public static String ToHexString(this UInt64 target, Int32 minBytes = 0)
        {
            var output = target.ToString("X").Replace("-", "");
            output = new String('0', Math.Max(0, minBytes * 2 - output.Length)) + output;
            return output;
        }

        public static String ToHexString(this Byte[] target, Int32 minBytes = 0)
        {
            var output = BitConverter.ToString(target).Replace("-", "");
            output = new String('0', Math.Max(0, minBytes * 2 - output.Length)) + output;
            return output;
        }
    }
}