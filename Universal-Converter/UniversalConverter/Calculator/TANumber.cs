using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalConverter
{
    abstract class Number
    {
        public abstract Number Copy();
        public abstract Number Add(Number b);
        public abstract Number Substract(Number b);
        public abstract Number Mul(Number b);
        public abstract Number Devide(Number b);
        public abstract Number Squared();
        public abstract Number Inverse();
        public abstract bool IsEqual(Number b);
        public abstract bool IsZero();
        public abstract override string ToString();
    }
    class PNumber : Number
    {
        public double number { get; set; }
        public int b { get; set; }
        public int c { get; set; }

        public PNumber(double n)
        {
            b = 10;
            c = 5;
            number = n;
        }
        public PNumber(double n, int b, int c)
        {
            this.b = b;
            this.c = c;
            number = n;
        }
        public override Number Copy()
        {
            double a = this.number;
            return new PNumber(a, b, c);
        }
        public override Number Add(Number b)
        {
            double c = this.number + (b as PNumber).number; 
            return new PNumber(Math.Round(c, this.c), this.b, this.c);
        }
        public override Number Substract(Number b)
        {
            double c = this.number - (b as PNumber).number;
            return new PNumber(Math.Round(c, this.c), this.b, this.c);
        }
        public override Number Mul(Number b)
        {
            double c = this.number * (b as PNumber).number;
            return new PNumber(Math.Round(c, this.c), this.b, this.c);
        }
        public override Number Devide(Number b)
        {
            double c = this.number / (b as PNumber).number;
            return new PNumber(Math.Round(c, this.c), this.b, this.c);
        }
        public override Number Squared()
        {
            double c = this.number * this.number;
            return new PNumber(Math.Round(c, this.c), this.b, this.c);
        }
        public override Number Inverse()
        {
            double c = 1.0 / this.number;
            return new PNumber(Math.Round(c, this.c), this.b, this.c);
        }
        public override bool IsEqual(Number b)
        {
            return ((b as PNumber).number == this.number);
        }
        public override bool IsZero()
        {
            return (Math.Abs(this.number) < 1e-5); //
        }
        public override string ToString()
        {
            if (b != 10)
            {
                string str = Converter.Conver_10_P.Do(Math.Abs(number), b, c);
                if (number > 0)
                    return str;
                else
                    return "-" + str;
            }
            else return number.ToString();
        }

    }
    class Frac : Number
    {
        PNumber num;
        PNumber dnom;
        public Frac(double n, double dn)
        {
            num = new PNumber(n);
            dnom = new PNumber(dn);
        }
        public override Number Copy()
        {
            double c = this.num.number;
            double d = this.dnom.number;
            return new Frac(c, d);
        }
        public override Number Add(Number b)
        {
            if (this.dnom.number != (b as Frac).dnom.number)
            {
                if (this.dnom.number % (b as Frac).dnom.number != 0 && (b as Frac).dnom.number % this.dnom.number!= 0)
                {
                    double c = this.num.number * (b as Frac).dnom.number +
                 this.dnom.number * (b as Frac).num.number;
                    double d = this.num.number * (b as Frac).dnom.number;
                    return new Frac(c, d);
                }
                else
                {
                    if (this.dnom.number % (b as Frac).dnom.number == 0)
                    {
                        double ratio = this.dnom.number / (b as Frac).dnom.number;
                        double c = this.num.number + ratio * (b as Frac).num.number;
                        double d = this.dnom.number;
                        return new Frac(c, d);
                    }
                    else
                    {
                        double ratio = (b as Frac).dnom.number / this.dnom.number;
                        double c = (b as Frac).num.number + ratio * this.num.number;
                        double d = (b as Frac).dnom.number;
                        return new Frac(c, d);

                    }

                }
            }
            else
            {
                double c = this.num.number + (b as Frac).num.number;
                double d = dnom.number;
                return new Frac(c, d);
            }
        }
        public override Number Substract(Number b)
        {
            if (this.dnom.number != (b as Frac).dnom.number)
            {
                if (this.dnom.number % (b as Frac).dnom.number != 0 && (b as Frac).dnom.number % this.dnom.number != 0)
                {
                    double c = this.num.number * (b as Frac).dnom.number -
                 this.dnom.number * (b as Frac).num.number;
                    double d = this.num.number * (b as Frac).dnom.number;
                    return new Frac(c, d);
                }
                else
                {
                    if (this.dnom.number % (b as Frac).dnom.number == 0)
                    {
                        double ratio = this.dnom.number / (b as Frac).dnom.number;
                        double c = this.num.number - ratio * (b as Frac).num.number;
                        double d = this.dnom.number;
                        return new Frac(c, d);
                    }
                    else
                    {
                        double ratio = (b as Frac).dnom.number / this.dnom.number;
                        double c = (b as Frac).num.number - ratio * this.num.number;
                        double d = (b as Frac).dnom.number;
                        return new Frac(c, d);

                    }

                }
            }
            else
            {
                double c = this.num.number - (b as Frac).num.number;
                double d = dnom.number;
                return new Frac(c, d);
            }
        }
        public override Number Mul(Number b)
        {
            double c = this.num.number * (b as Frac).num.number;
            double d = this.dnom.number * (b as Frac).dnom.number;

            if (c % d == 0)
            {
                c /= d;
                d = 1;
            }
            else if (d % c == 0)
            {
                d /= c;
                c = 1;
            }


            return new Frac(c, d);
        }
        public override Number Devide(Number b)
        {
            double c = this.num.number * (b as Frac).dnom.number;
            double d = this.dnom.number * (b as Frac).num.number;

            if (c % d == 0)
            {
                c /= d;
                d = 1;
            }
            else if (d % c == 0)
            {
                d /= c;
                c = 1;
            }


            return new Frac(c, d);
        }
        public override Number Squared()
        {
            double c = this.num.number * this.num.number;
            double d = this.dnom.number * this.dnom.number;
            return new Frac(c, d);
        }
        public override Number Inverse()
        {
            double c = this.dnom.number;
            double d = this.num.number;
            return new Frac(c, d);
        }
        public override bool IsEqual(Number b)
        {
            return (Math.Abs((b as Frac).num.number / (b as Frac).dnom.number - this.num.number / this.dnom.number) < 1e-5); //!!
        }
        public override bool IsZero()
        {
            return (Math.Abs(this.num.number) < 1e-5 || Math.Abs(dnom.number) == double.MaxValue); //!!
        }
        public override string ToString()
        {
            return num + "|" + dnom;
        }
       
    }
    class Comp : Number
    {
        PNumber re;
        PNumber im;
        public int c = 5;

        public Comp(double n, double dn)
        {
            re = new PNumber(n);
            im = new PNumber(dn);
        }
        public Comp(double n, double dn, int c)
        {
            re = new PNumber(n);
            im = new PNumber(dn);
            this.c = c;
        }
        public override Number Copy()
        {
            double c = this.re.number;
            double d = this.im.number;
            return new Comp(c, d, this.c);
        }
        public double Mod()
        {
            return re.number * re.number + im.number * im.number;
        }
        public double ArgDeg()
        {
            return ArgRad() * 180 / Math.PI;
        }
        /*fi = (arcTg(b/a), a>0;
         * pi/2, a = 0, b > 0; 
         * arcTg(b/a) + pi, a < 0;
         * -pi/2, a = 0, b <0 )*/
        public double ArgRad()
        {
            if (re.number > 0) return Math.Atan(im.number / re.number);
            if (re.number == 0 && im.number > 0) return Math.PI / 2;
            if (re.number < 0) return Math.Atan(im.number / re.number) + Math.PI;
            if (re.number == 0 && im.number < 0) return -Math.PI / 2;
            else return 0;
        }

        //public Number Exponentiation(int a)
        //{
        //    double real = Math.Pow(Mod(), a) * Math.Cos(a * ArgRad());
        //    double imag = Math.Pow(Mod(), a) * Math.Sin(a * ArgRad());
        //    Comp res = new Comp(real, imag);
        //    return res;
        //}
        public override Number Add(Number b)
        {
            double c = this.re.number + (b as Comp).re.number;
            double d = this.im.number + (b as Comp).im.number;
            return new Comp(Math.Round(c, this.c), Math.Round(d, this.c), this.c);
        }
        public override Number Mul(Number b)
        {
            double c = this.re.number * (b as Comp).re.number - this.im.number * (b as Comp).im.number;
            double d = this.re.number * (b as Comp).im.number + this.im.number * (b as Comp).re.number;
            return new Comp(Math.Round(c, this.c), Math.Round(d, this.c), this.c);
        }
        public override Number Substract(Number b)
        {
            double c = this.re.number - (b as Comp).re.number;
            double d = this.im.number - (b as Comp).im.number;
            return new Comp(Math.Round(c, this.c), Math.Round(d, this.c), this.c);
        }
        public override Number Devide(Number b) //!!
        {
            double c = this.re.number * (b as Comp).re.number + this.im.number * (b as Comp).im.number;
            double d = this.im.number * (b as Comp).re.number - this.re.number * (b as Comp).im.number;
            double coef = ((b as Comp).re.number * (b as Comp).re.number + (b as Comp).im.number * (b as Comp).im.number);
            c /= coef;
            d /= coef;
            //return new Comp(c, d);
            //double c = this.Mod() / (b as Comp).Mod() * Math.Cos(this.ArgRad() - (b as Comp).ArgRad()); 
            //double d = this.Mod() / (b as Comp).Mod() * Math.Sin(this.ArgRad() - (b as Comp).ArgRad());
            return new Comp(Math.Round(c, this.c), Math.Round(d, this.c), this.c);
        }
        public override Number Squared()
        {
            double c = this.re.number * this.re.number - this.im.number * this.im.number;
            double d = 2 * this.re.number * this.im.number;
            return new Comp(Math.Round(c, this.c), Math.Round(d, this.c), this.c);
        }
        public override Number Inverse()
        {
            double c = this.re.number / (this.re.number * this.re.number + this.im.number * this.im.number);
            double d = this.im.number / (this.re.number * this.re.number - this.im.number * this.im.number);
            return new Comp(Math.Round(c, this.c), Math.Round(d, this.c), this.c);
        }
        public override bool IsEqual(Number b)
        {
            return (this.re.number == (b as Comp).re.number && this.im.number == (b as Comp).im.number);
        }
        public override bool IsZero()
        {
            return (Math.Abs(this.re.number) < 1e-5 && Math.Abs(this.im.number) < 1e-5); //!!
        }
        public override string ToString()
        {
            return re.ToString() + " + i*" + im.ToString();
        }
    }
}
