using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FloatExplorer
{
    static class FloatUtils
    {
        [StructLayout(LayoutKind.Explicit)]
        internal struct FloatUnion
        {
            [FieldOffset(0)]
            public float FloatValue;

            [FieldOffset(0)]
            public uint UintValue;
        }

        public static readonly uint SignMask =     0x80000000;
        public static readonly uint ExponentMask = 0x7F800000;
        public static readonly uint MantissaMask = 0x007FFFFF;

        public static byte GetRawExponent(FloatUnion floatUnion)
        {
            return Convert.ToByte((floatUnion.UintValue & ExponentMask)>>23);
        }

        public static uint GetRawMantissa(FloatUnion floatUnion)
        {
            return floatUnion.UintValue & MantissaMask;
        }

        public static bool IsPositive(FloatUnion floatUnion)
        {
            return ((floatUnion.UintValue & SignMask)>>31)==0;
        }

        public static bool IsNegative(FloatUnion floatUnion)
        {
            return ((floatUnion.UintValue & SignMask) >> 31) == 1;
        }

        public static bool IsZero(FloatUnion floatUnion)
        {
            return (floatUnion.UintValue & 0x7FFFFFFF) == 0; 
        }

        public static bool IsInfinity(FloatUnion floatUnion)
        {
            return GetRawExponent(floatUnion) == 255 && GetRawMantissa(floatUnion) == 0;
        }
    }
}
