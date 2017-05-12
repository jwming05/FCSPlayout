using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class SamplePointGenerator
    {
        public static double[] Generate()
        {
            var start = 0.0;
            var stop = Math.PI * 2;
            var step = Math.PI/2;

            List<double> list = new List<double>();
            for (double i = start; i < stop; i += step)
            {
                list.Add(Math.Sin(Math.PI*2.0/8.0*i));
            }

            Debug.Assert(list.Count == 4);
            return list.ToArray();
        }

        public static double[] Generate(int n)
        {
            var rand = new Random();
            double[] result = new double[n];
            for(int i = 0; i < result.Length; i++)
            {
                result[i] = rand.NextDouble() * 5;
            }
            return result;
        }
    }
}
