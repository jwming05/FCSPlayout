using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatExplorer.Domain
{
    public class Utils
    {
        private static readonly string[] _hexStrings =
        {
            "0000","0001","0010","0011",
            "0100","0101","0110","0111",
            "1000","1001","1010","1011",
            "1100","1101","1110","1111"
        };

        private static readonly byte _lowMask = 0x0F;
        private static readonly byte _highMask = 0xF0;
        public static string Bits(float f)
        {
            var bytes = BitConverter.GetBytes(f);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(Bits(b));
                sb.Append(' ');
            }

            sb.Remove(sb.Length - 1, 1); // remove last whitespace.
            return sb.ToString();
        }

        public static string Bits(int n)
        {
            // 数组顺序是从低字节到高字节
            var bytes = BitConverter.GetBytes(n);
            StringBuilder sb = new StringBuilder();

            // 输出应该是从高字节到低字节
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                sb.Append(Bits(bytes[i]));
                sb.Append(' ');
            }

            sb.Remove(sb.Length - 1, 1); // remove last whitespace.
            return sb.ToString();
        }

        public static string Bits(byte b)
        {
            var low = b & _lowMask;
            var high = (b & _highMask) >> 4;
            return _hexStrings[high]+" "+ _hexStrings[low];
        }

        /// <summary>
        /// 获取IEEE754单精度浮点数的符号位(第31位)。
        /// </summary>
        /// <param name="f">指定要获取符号位的浮点数。</param>
        /// <returns><c>true</c>表示符号位为1，否则返回<c>false</c>.</returns>
        public static bool GetSign(float f)
        {
            var bytes = BitConverter.GetBytes(f);
            Debug.Assert(bytes.Length == 4);
            var highestByte = bytes[bytes.Length-1];

            return (highestByte & 0x80) != 0;
        }

        /// <summary>
        /// 获取IEEE754单精度浮点数的阶码(第23位到第30位共8位)。
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static byte GetExponent(float f)
        {
            var bytes = BitConverter.GetBytes(f);
            var highestByte = bytes[bytes.Length - 1];

            byte result = Convert.ToByte(highestByte << 1);

            return Convert.ToByte(result + ((bytes[bytes.Length - 2] & 0x80) == 0 ? 0 : 1));
        }

        /// <summary>
        /// 获取IEEE754单精度浮点数的尾数(第0位到第22位共23位)。
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static uint GetMantissa(float f)
        {
            return 0;
        }
    }
}
