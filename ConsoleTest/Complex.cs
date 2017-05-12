using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    public class Complex
    {
        public Complex(double real):this(real,0.0)
        {
        }

        public Complex(double real,double imaginary)
        {
            this.Real = real;
            this.Imaginary = imaginary;
        }

        public double Real { get; private set; }
        public double Imaginary { get; private set; }

        public double Module
        {
            get { return Math.Sqrt(this.Real*this.Real+this.Imaginary*this.Imaginary); }
        }

        public Complex Conjugate
        {
            get { return new Complex(this.Real, -1 * this.Imaginary); }
        }

        public override string ToString()
        {
            return string.Format("{0} + {1}*i", Math.Round(this.Real,3), Math.Round(this.Imaginary,3));
        }

        public static explicit operator Complex(double real)
        {
            return new Complex(real);
        }

        public static Complex operator *(Complex c, double d)
        {
            return new Complex(c.Real * d, c.Imaginary * d);
        }

        public static Complex operator *(double d, Complex c)
        {
            return c * d;
        }

        public static Complex operator +(Complex left,Complex right)
        {
            return new Complex(left.Real + right.Real, left.Imaginary + right.Imaginary);
        }

        public static Complex operator -(Complex left, Complex right)
        {
            return new Complex(left.Real - right.Real, left.Imaginary - right.Imaginary);
        }

        public static Complex operator *(Complex left, Complex right)
        {
            return new Complex(left.Real*right.Real-left.Imaginary*right.Imaginary, 
                left.Imaginary*right.Real + left.Real*right.Imaginary);
        }
    }
}
