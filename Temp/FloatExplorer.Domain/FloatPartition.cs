using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FloatExplorer.Domain
{
    public class FloatPartition
    {
        [StructLayout(LayoutKind.Explicit)]
        internal struct FloatUnion
        {
            [FieldOffset(0)]
            internal Single FloatValue;

            [FieldOffset(0)]
            internal UInt32 UintValue;
        }

        public FloatPartition(float f)
        {
            FloatUnion fUnion = default(FloatUnion);
            fUnion.FloatValue = f;
            this.Sign = GetSign(fUnion.UintValue);
            this.Exponent = GetExponent(fUnion.UintValue);
            this.Mantissa = GetMantissa(fUnion.UintValue);
        }

        public bool Sign { get; private set; }
        public byte Exponent { get; private set; }
        public uint Mantissa { get; private set; }

        /// <summary>
        /// 获取IEEE754单精度浮点数的符号位(第31位)。
        /// </summary>
        /// <param name="f">指定要获取符号位的浮点数。</param>
        /// <returns><c>true</c>表示符号位为1，否则返回<c>false</c>.</returns>
        private static bool GetSign(uint uintValue)
        {
            // 80 00 00 00
            uint mask = 0x80000000;
            return (uintValue & mask) != 0;
        }

        /// <summary>
        /// 获取IEEE754单精度浮点数的阶码(第23位到第30位共8位)。
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private static byte GetExponent(uint uintValue)
        {
            // 7F 80 00 00
            uint mask =0x7F800000;
            uint temp = uintValue & mask;
            return Convert.ToByte((temp >> 23));
        }

        /// <summary>
        /// 获取IEEE754单精度浮点数的尾数(第0位到第22位共23位)。
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private static uint GetMantissa(uint uintValue)
        {
            // 00 7F FF FF
            uint mask = 0x007FFFFF;
            return uintValue & mask;
        }
    }
}
