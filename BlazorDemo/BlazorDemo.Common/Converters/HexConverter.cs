using System;
using System.Linq;
using BlazorDemo.Common.Utils;

namespace BlazorDemo.Common.Converters
{
    public static class HexConverter
    {
        public static string HexToString(this byte[] value, bool prefix = false)
        {
            var strPrex = prefix ? "0x" : "";
            return strPrex + string.Concat(value.Select(b => b.ToStringInvariant("x2")).ToArray());
        }

        public static byte[] HexToByteArray(this string str)
        {
            byte[] bytes;
            if (string.IsNullOrEmpty(str))
                bytes = Array.Empty<byte>();
            else
            {
                var stringLength = str.Length;
                var charIndex = str.StartsWith("0x", StringComparison.Ordinal) ? 2 : 0;
                var noChars = stringLength - charIndex;
                var addLeadingZero = false;

                if (0 != noChars % 2)
                {
                    addLeadingZero = true;
                    noChars += 1;
                }

                bytes = new byte[noChars / 2];

                var writeIndex = 0;
                if (addLeadingZero)
                {
                    bytes[writeIndex++] = CharUtils.CharacterToByte(str[charIndex], charIndex);
                    charIndex += 1;
                }

                for (var readIndex = charIndex; readIndex < str.Length; readIndex += 2)
                {
                    var upper = CharUtils.CharacterToByte(str[readIndex], readIndex, 4);
                    var lower = CharUtils.CharacterToByte(str[readIndex + 1], readIndex + 1);

                    bytes[writeIndex++] = (byte)(upper | lower);
                }
            }

            return bytes;
        }
    }
}
