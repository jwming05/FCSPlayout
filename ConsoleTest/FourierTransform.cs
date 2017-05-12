using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class FourierTransform
    {
        public static Complex[] DFT(double[] input)
        {
            Complex[] output = new Complex[input.Length];
            int M = input.Length;

            for (int m = 0; m < output.Length; m++)
            {
                Complex temp = (Complex)0.0;
                for (int n = 0; n < input.Length; n++)
                {
                    double a = -2.0 * Math.PI * m * n / M;
                    double real = Math.Cos(a);
                    double imag = Math.Sin(a);

                    // Math.Pow(-1,n)*
                    temp += input[n] * new Complex(real, imag); // (-j2*Math.PI*u*x/input.Length);
                }
                output[m] = temp;
            }

            return output;
        }

        public static Complex[] DFT2(double[] input)
        {
            Complex[] output = new Complex[input.Length];
            int M = input.Length;

            for (int m = 0; m < output.Length; m++)
            {
                Complex temp = (Complex)0.0;
                for (int n = 0; n < input.Length; n++)
                {
                    double a = -2.0 * Math.PI * m * n / M;
                    double real = Math.Cos(a);
                    double imag = Math.Sin(a);

                    // 
                    temp += Math.Pow(-1, n) * input[n] * new Complex(real, imag); // (-j2*Math.PI*u*x/input.Length);
                }
                output[m] = temp;
            }

            return output;
        }

        public static Complex[] IDFT(Complex[] input)
        {
            Complex[] output = new Complex[input.Length];
            int M = input.Length;

            for (int m = 0; m < output.Length; m++)
            {
                Complex temp = (Complex)0.0;
                for (int n = 0; n < input.Length; n++)
                {
                    double a = 2.0 * Math.PI * m * n / M;
                    double real = Math.Cos(a);
                    double imag = Math.Sin(a);

                    temp += (1.0/M)*input[n] * new Complex(real, imag); // (-j2*Math.PI*u*x/input.Length);
                }
                output[m] = temp;
            }

            return output;
        }

        public static Complex[] IDFT2(Complex[] input)
        {
            Complex[] output = new Complex[input.Length];
            int M = input.Length;

            for (int m = 0; m < output.Length; m++)
            {
                Complex temp = (Complex)0.0;
                for (int n = 0; n < input.Length; n++)
                {
                    double a = 2.0 * Math.PI * m * n / M;
                    double real = Math.Cos(a);
                    double imag = Math.Sin(a);

                    temp += Math.Pow(-1, m) * (1.0 / M) * input[n] * new Complex(real, imag); // (-j2*Math.PI*u*x/input.Length);
                }
                output[m] = temp;
            }

            return output;
        }

        public static void DFT3(double[] input, out double[] rex, out double[] imx)
        {
            // The discrete fourier transform

            int n = input.Length;
            rex = new double[input.Length/2+1];
            imx = new double[input.Length / 2 + 1];

            for(int k = 0; k < rex.Length; k++)
            {
                for(int i = 0; i < input.Length; i++)
                {
                    rex[k] += input[i] * Math.Cos(2.0 * Math.PI * k * i / n);
                    imx[k] += -1 * input[i] * Math.Sin(2.0 * Math.PI * k * i / n);
                }
            }
        }

        public static double[] IDFT3(double[] rex, double[] imx)
        {
            Debug.Assert(rex.Length == imx.Length);
            int n = (rex.Length - 1) * 2;
            double[] result = new double[n];

            for(int k = 0; k < rex.Length; k++)
            {
                rex[k] /= n / 2.0;
                imx[k] /= -1.0 * (n/2.0);
            }

            rex[0] /= 2.0;
            rex[rex.Length - 1] /= 2.0;

            for(int k = 0; k < rex.Length; k++)
            {
                for(int i = 0; i < n; i++)
                {
                    result[i] += rex[k] * Math.Cos(2.0 * Math.PI * k * i / n);
                    result[i] += imx[k] * Math.Sin(2.0 * Math.PI * k * i / n);
                }
            }

            return result;
        }
    }
}
