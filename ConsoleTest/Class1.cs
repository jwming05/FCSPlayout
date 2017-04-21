using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Class1
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
    }
}
