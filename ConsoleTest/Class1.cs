﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class Class1
    {
        int count;
        int sum;
        int squareSum;

        public void Add(int num)
        {
            sum += num;
            squareSum += num * num;
        }

        public int GetResult()
        {
            return (squareSum / count) - (sum * sum) / (count * count);
        }

        //public int Variance()
        //{

        //}

            /// <summary>
            /// 计算方差。
            /// </summary>
            /// <param name="numbers"></param>
            /// <returns></returns>
        public double Variance(List<double> numbers)
        {
            var average = numbers.Average();

            for(int i = 0; i < numbers.Count; i++)
            {
                var distance = numbers[i] - average;
                numbers[i] = distance * distance;
            }

            return numbers.Average();
        }

        public double Variance2(IEnumerable<double> numbers)
        {
            int count=0;
            double sum=0.0;
            double squareSum=0.0;

            foreach(var num in numbers)
            {
                count++;
                sum += num;
                squareSum += num * num;
            }

            return (squareSum / count) - (sum * sum) / (count * count);
        }

        public static double f(double x)
        {
            return 10 + Math.Sin(x) + 2 * Math.Sin(2 * x) + 3 * Math.Sin(3 * x) + 4 * Math.Sin(4 * x);
        }

        public static Complex[] DFT(double[] input)
        {
            Complex[] output = new Complex[input.Length];
            for(int u = 0; u < output.Length; u++)
            {
                Complex temp = (Complex)0.0;
                for(int x = 0; x < input.Length; x++)
                {
                    var a = -2 * Math.PI * u * x / input.Length;
                    var real = Math.Cos(a);
                    var imga = Math.Sin(a);

                    temp += (1.0 / input.Length) * input[x] * new Complex(real,imga); // (-j2*Math.PI*u*x/input.Length);
                }
            }

            return output;
        }
    }
}
