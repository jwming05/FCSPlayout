using ConsoleTest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

class MainClass
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct FloatUnion
    {
        [FieldOffset(0)]
        public float FloatValue;

        [FieldOffset(0)]
        public uint UintValue;
    }

    public static void Main()
    {
        double d = Math.Pow(2, -149);
        double d2 = float.Epsilon;

        Console.WriteLine("{0}, {1}, {2}", d, d2, d == d2);

        var floatUnion = default(FloatUnion);
        floatUnion.UintValue = 0x00800000;
        double d3 = floatUnion.FloatValue;
        Console.WriteLine(d3);

        floatUnion = default(FloatUnion);
        floatUnion.UintValue = 0x00800001;
        double d4 = floatUnion.FloatValue;
        Console.WriteLine(d4);

        var d5 = d4 - d3;
        //var d6= Math.Pow(2, -1);
        Console.WriteLine("{0}, {1}, {2}", d5, d2, d5 == d2);

        floatUnion.UintValue = 0x80000000;
        Console.WriteLine(floatUnion.FloatValue);
        Console.ReadKey();
    }

    private static void PrintMatrix(double[][] matrix)
    {
        foreach(var row in matrix)
        {
            Console.WriteLine("| " + string.Join(", ", row) + " |");
        }
    }

    public static string BytesToStr(byte[] bytes)
    {
        StringBuilder str = new StringBuilder();

        for (int i = 0; i < bytes.Length; i++)
            str.AppendFormat("{0:X2}", bytes[i]);

        return str.ToString();
    }

    public static void PrintHash(byte[] input)
    {
        SHA256Managed sha = new SHA256Managed();
        Console.WriteLine("ComputeHash  : {0}", BytesToStr(sha.ComputeHash(input)));
    }

    public static void PrintHashOneBlock(byte[] input)
    {
        SHA256Managed sha = new SHA256Managed();
        
        sha.TransformFinalBlock(input, 0, input.Length);
        Console.WriteLine("FinalBlock   : {0}", BytesToStr(sha.Hash));
    }

    public static void PrintHashMultiBlock(byte[] input, int size)
    {
        SHA256Managed sha = new SHA256Managed();
        int offset = 0;

        while (input.Length - offset >= size)
        {
            //offset += sha.TransformBlock(input, offset, size, input, offset);
            offset += sha.TransformBlock(input, offset, size, null, 0);
            Console.WriteLine(BytesToStr(input));
        }
            

        sha.TransformFinalBlock(input, offset, input.Length - offset);
        Console.WriteLine("MultiBlock {0:00}: {1}", size, BytesToStr(sha.Hash));
    }

    /// <summary>
    /// 对矩阵实施行变换转换成阶梯矩阵。
    /// </summary>
    /// <param name="matrix"></param>
    /// <returns></returns>
    public static double[][] RowTransform(double[][] matrix)
    {
        double[][] result = CopyMatrix(matrix);
        int rowCount = result.Length;
        int columnCount = result[0].Length;

        int row = 0,column=0;
        while (row < rowCount - 1 && column<columnCount)
        {
            if (result[row][column] == 0.0)
            {
                int firstNonZero = row + 1;
                for (; firstNonZero < rowCount; firstNonZero++)
                {
                    if (result[firstNonZero][column] != 0.0)
                    {
                        break;
                    }
                }

                if (firstNonZero < rowCount)
                {
                    // 交换row和firstNonZero行。

                    var temp = result[row];
                    result[row] = result[firstNonZero];
                    result[firstNonZero] = temp;
                }
                else
                {
                    // [row][column]- [rowCount-1][column]全为0
                    column++;
                    continue;
                }

            }

            Debug.Assert(result[row][column]!=0.0);

            // 使[row+1][column]- [rowCount-1][column]全为0
            for (int r = row + 1; r < rowCount; r++)
            {
                if (result[r][column] != 0.0)
                {
                    // 使[r][column]为0

                    // r行加上 row行*系数。
                    // 系数计算 [r][column]+k*[row][column]==0
                    // k = -([r][column]/[row][column])

                    double ratio = -(result[r][column] / result[row][column]);
                    result[r][column] = 0.0;
                    for (int c = column + 1; c < columnCount; c++)
                    {
                        result[r][c] += ratio * result[row][c];
                    }
                }
            }

            row++;
            column++;
        }

        return result;
    }

    /// <summary>
    /// 对矩阵实施行变换转换成阶梯矩阵。
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="uniform">指定是否转换成行最简型</param>
    /// <returns></returns>
    public static double[][] RowTransform(double[][] matrix,bool uniform)
    {
        double[][] result = CopyMatrix(matrix);
        int rowCount = result.Length;
        int columnCount = result[0].Length;

        int row = 0, column = 0;
        while (row < rowCount - 1 && column < columnCount)
        {
            if (result[row][column] == 0.0)
            {
                int firstNonZero = row + 1;
                for (; firstNonZero < rowCount; firstNonZero++)
                {
                    if (result[firstNonZero][column] != 0.0)
                    {
                        break;
                    }
                }

                if (firstNonZero < rowCount)
                {
                    // 交换row和firstNonZero行。

                    var temp = result[row];
                    result[row] = result[firstNonZero];
                    result[firstNonZero] = temp;
                }
                else
                {
                    // [row][column]- [rowCount-1][column]全为0
                    column++;
                    continue;
                }

            }

            Debug.Assert(result[row][column] != 0.0);

            // 使[row+1][column]- [rowCount-1][column]全为0
            for (int r = row + 1; r < rowCount; r++)
            {
                if (result[r][column] != 0.0)
                {
                    // 使[r][column]为0

                    // r行加上 row行*系数。
                    // 系数计算 [r][column]+k*[row][column]==0
                    // k = -([r][column]/[row][column])

                    double ratio = -(result[r][column] / result[row][column]);
                    result[r][column] = 0.0;
                    for (int c = column + 1; c < columnCount; c++)
                    {
                        result[r][c] += ratio * result[row][c];
                    }
                }
            }


            if (uniform)
            {
                // 使[row-1][column]-[0][column]全为0
                for (int r = row - 1; r >= 0; r--)
                {
                    if (result[r][column] != 0.0)
                    {
                        // 使[r][column]为0

                        // r行加上 row行*系数。
                        // 系数计算 [r][column]+k*[row][column]==0
                        // k = -([r][column]/[row][column])

                        double ratio = -(result[r][column] / result[row][column]);
                        result[r][column] = 0.0;
                        for (int c = column + 1; c < columnCount; c++)
                        {
                            result[r][c] += ratio * result[row][c];
                        }
                    }
                }

                // 使[row][column]==1
                if (result[row][column] != 1.0)
                {
                    // row行=row行*系数。
                    // 系数计算 k*[row][column]==1
                    // k=1/[row][column]

                    double ratio = 1.0 / result[row][column];
                    result[row][column] = 1.0;
                    for (int c = column + 1; c < columnCount; c++)
                    {
                        result[row][c] = ratio * result[row][c];
                    }
                }
            }
            
            row++;
            column++;
        }

        return result;
    }

    private static double[][] CopyMatrix(double[][] matrix)
    {
        double[][] copy = new double[matrix.Length][];
        for(int i = 0; i < copy.Length; i++)
        {
            double[] temp = new double[matrix[i].Length];
            matrix[i].CopyTo(temp, 0);
            copy[i] = temp;
        }
        return copy;
    }
    //

    /// <summary>
    /// 把窗口坐标映射为视口坐标。
    /// </summary>
    /// <param name="pt">窗口坐标系中要映射的点</param>
    /// <param name="xScale">x视口度量/x窗口度量</param>
    /// <param name="yScale">y视口度量/y窗口度量</param>
    /// <param name="origin">窗口坐标原点在视口中的坐标。</param>
    /// <returns></returns>
    private static PointF MapWindowToViewport(PointF pt, float xScale, float yScale, PointF origin)
    {
        return new PointF(pt.X * xScale + origin.X, pt.Y * yScale + origin.Y);
    }

    private static PointF MapViewportToWindow(PointF pt, float xScale, float yScale, PointF origin)
    {
        var x = pt.X - origin.X; // 视口度量
        var y = pt.Y - origin.Y; // 视口度量

        return new PointF(x / xScale, y / yScale);
    }
}