using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalConverter.Calculator
{
    abstract class TAEditor
    {
        public string number = "0";
        public abstract string Copy();
        public abstract string addDigit(int i);
        public abstract string AddOprtnSign(int i);
        public abstract void ChangeSign();
        public abstract string Clear();
        public abstract string BackSpace();
        public abstract bool IsZero();
        public abstract string Edit(int i);
    }

    class REditor : TAEditor
    {
        public REditor()
        {
            number="0";
        }
        public override string AddOprtnSign(int i)
        {
            switch (i)
            {
                case 21:
                    return "+";

                case 22:
                    return "-";

                case 23:
                    return "×";

                case 24:
                    return "/";

                case 25:
                    Clear();
                    return "^(-1)";

                case 26:
                    Clear();
                    return "^2";

                default:
                    return "";
            }
        }
        public override void ChangeSign()
        {
            if (number.Length == 0) return;
            if (number.First() == '-')
            {
                number = number.Remove(0, 1);
            }
            else
            {
                number = number.Insert(0, "-");
            }
        }
        public override bool IsZero()
        {
            return (number == "0");
        }
        public override string Copy()
        {
            string st = this.number;
            return st;
        }
        public override string addDigit(int i)
        {
            if (number.StartsWith("0") && number.Length == 1)
                addDelimeter();
            if (i > 9)
                number += ((char)(i + 55)).ToString();
            else
                number += Convert.ToString(i);

            return number;
        }
        public void addDelimeter()
        {
            //если запятая уже есть, удаляем её и ставим новую в конце
            if (number.Contains(Const.Sep))
            {
                number = number.Remove(number.IndexOf(Const.Sep), 1);
                while (number.StartsWith("0") && !IsZero())
                    number = number.Remove(0, 1);
            }
            number += Const.Sep;
        }

        public override string Clear()
        {
            this.number = "";
            return this.number;
        }
        public override string BackSpace()
        {
            if (number.Length > 0)
                number = number.Remove(number.Length - 1, 1);
            return number;
        }
        public override string Edit(int j)
        {

            if (j < 16)
            {
                number = addDigit(j);
            }
            else
                switch (j)
                {
                    case 16:
                        addDelimeter();
                        break;

                    case 17:
                        ChangeSign();
                        break;

                    case 19:
                        number = Clear();
                        break;

                    case 20:
                        number = BackSpace();
                        break;
                }

            return number;
        }
        public double ToDouble(int b, int c)
        {
            if (number == "" || number == "-0,0" || number == "0,0" || number == "0") return 0;
            if (b != 10)
            {

                if (number.StartsWith("-"))
                {
                    string new_str = number.Remove(0, 1);
                    double res = Converter.Conver_P_10.Do(new_str, b, c);
                    return -res;
                }
                else
                {
                    return Converter.Conver_P_10.Do(number, b, c);
                }
            }
            else
            {
                if (number.StartsWith("-"))
                {
                    string new_str = number.Remove(0, 1);
                    return -Convert.ToDouble(new_str);
                }
                else
                {
                    return Convert.ToDouble(number);
                }
            }
        }
    }
    class FEditor : TAEditor
    {
        public string num { get; set; }
        public string dnom { get; set; }
        public enum mode { num, dnom }
        public mode Mode = mode.num;

        public FEditor()
        {
            num = "0";
            dnom = "1";
        }

        public override void ChangeSign()
        {
            if (num.Length == 0) return;
            if (number.First() == '-' || num.First() == '-')
            {
                number = number.Remove(0, 1);
                num = num.Remove(0, 1);
            }
            else
            {
                number = number.Insert(0, "-");
                num = num.Insert(0, "-");
            }
        }
        public override string AddOprtnSign(int i)
        {
            switch (i)
            {
                case 21:
                    return "+";

                case 22:
                    return "-";

                case 23:
                    return "×";

                case 24:
                    return "/";

                case 25:
                    Clear();
                    return "^(-1)";

                case 26:
                    Clear();
                    return "^2";

                default:
                    return "";
            }

        }
        public override bool IsZero()
        {
            return (num == "0"|| num == "" || number == "0|" || number == "0");
        }
        public override string Copy()
        {
            string st = this.num + "|" + this.dnom;
            return st;
        }
        public override string addDigit(int i)
        {
            switch (Mode)
            {
                case mode.dnom:
                    if (dnom == "0") dnom = "";
                    this.dnom += i;
                    break;

                case mode.num:
                    if (IsZero()) num = "";
                    this.num += i;
                    break;
            }

            return num + "|" + dnom;
        }
        public override string Clear()
        {
            this.num = "0";
            this.dnom = "1";

            return num + "|" + dnom;
        }
        public override string BackSpace()
        {
            switch (Mode)
            {
                case mode.dnom:
                    if (dnom.Length > 0)
                        dnom = dnom.Remove(dnom.Length - 1, 1);
                    break;
                case mode.num:
                    if (num.Length > 0)
                        num = num.Remove(num.Length - 1, 1);
                    break;
            }

            return num + "|" + dnom;
        }
        public override string Edit(int i)
        {
            if (i < 16)
            {
                this.number = addDigit(i);
            }
            else
                switch (i)
                {
                    case 16:
                        // addDelimeter();
                        break;

                    case 17:
                        ChangeSign();
                        break;

                    case 19:
                        this.number = Clear();
                        break;

                    case 20:
                        this.number = BackSpace();
                        break;
                }

            return number;
        }
        
}
    class CEditor : TAEditor
    {
        public string re;
        public string im;
        public enum mode { re, im};
        public mode Mode = mode.re;
        public CEditor()
        {
            number = "0";
            re = "";
            im = "";
        }

        public override void ChangeSign()
        {
            if (Mode == mode.im && im.Length != 0)
            {
                if (im.First() == '-')
                {
                    im = im.Remove(0, 1);
                }
                else
                {
                    im = im.Insert(0, "-");
                }
                //return;
            }

            if (Mode == mode.re && re.Length != 0)
            {
                if (re.First() == '-')
                {
                    re = re.Remove(0, 1);
                }
                else
                {
                    re = re.Insert(0, "-");
                }
            }
            number = re + " + i*" + im;
        }
        public override string AddOprtnSign(int i)
        {
            switch (i)
            {
                case 21:
                    return "+";

                case 22:
                    return "-";

                case 23:
                    return "×";

                case 24:
                    return "/";

                case 25:
                    Clear();
                    return "^(-1)";

                case 26:
                    Clear();
                    return "^2";

                default:
                    return "";
            }

        }
        public void addDelimeter()
        {
            //если запятая уже есть, удаляем её и ставим новую в конце
            if (Mode == mode.re)
            {
                if (re.Contains(Const.Sep))
                {
                    re = re.Remove(re.IndexOf(Const.Sep), 1);
                    while (re.StartsWith("0") && !IsZero())
                        re = re.Remove(0, 1);
                }
                if (re.Length == 0) re += "0";
                re += Const.Sep;
            }
            else
            {
                if (im.Contains(Const.Sep))
                {
                    im = im.Remove(im.IndexOf(Const.Sep), 1);
                    while (im.StartsWith("0") && !IsZero())
                        im = im.Remove(0, 1);
                }
                if (im.Length == 0) im += "0";
                im += Const.Sep;
            }
            number = re + " + i*" + im;
        }

        public override bool IsZero()
        {
            return (number == "0" || (re == "" && im == ""));
        }
        public override string Copy()
        {
            string st = re + " + i*" + im;
            return st;
        }
        public override string addDigit(int i)
        {
            switch (Mode)
            {
                case mode.re:
                    if (re == "0") re = "";
                    this.re += i;
                    break;
                case mode.im:
                    if (im == "0") im = "";
                    this.im += i;
                    break;
            }
            
            return re + " + i*" + im;
        }
        public override string Clear()
        {
            this.re = "0";
            this.im = "0";

            return re + " + i*" + im;
        }
        public override string BackSpace()
        {
            switch (Mode)
            {
                case mode.re:
                    if (re.Length > 0)
                        re = re.Remove(re.Length - 1, 1);
                    break;

                case mode.im:
                    if (im.Length > 0)
                        im = im.Remove(im.Length - 1, 1);
                    break;
            }

            return re + " + i*" + im;
        }
        public override string Edit(int j)
        {
            if (j < 16)
            {
                number = addDigit(j);
            }
            else
                switch (j)
                {
                    case 16:
                        addDelimeter();
                        break;

                    case 17:
                        ChangeSign();
                        break;

                    case 19:
                        number = Clear();
                        break;

                    case 20:
                        number = BackSpace();
                        break;
                }

            return this.number;
        }
        /*public double[] parse()
        {
            //0 - re, 1 - im
            //if (number.EndsWith("+")|| number.EndsWith("-")|| number.EndsWith("/")|| number.EndsWith("*"))
            //{ }
            double[] RES = new double[2];
            if (number == "") return RES;
            if (number.Contains("i"))
            {
                number = number.Remove(number.IndexOf('i'), 2);
                string[] res_str = number.Split('+');
                RES[0] = Convert.ToDouble(res_str[0]);
                
                RES[1] = Convert.ToDouble(res_str[1]);

            }
            else
            {
                RES[0] = Convert.ToInt32(number);
                RES[1] = 0;
            }
            return RES;
        }*/
    }
    //класс "Редактор p-ичных чисел"
    /*    class TEditor
        {
            public string number { set; get; }

            public TEditor() { number = "0,"; }

            public void changeSign()
            {
                if (number.Length == 0) return;
                if (number.First() == '-')
                {
                    number = number.Remove(0, 1);
                }
                else
                {
                    number = number.Insert(0, "-");
                }
            }

            public bool isZero()
            {
                if (number.Equals("0,")) return true; 
                else return false;
            }

            public string addDigit(int digit)
            {

                if (number.StartsWith("0") && number.Length == 1)
                    addDelimeter();
                if (digit > 9)
                    number += ((char)(digit+55)).ToString();
                else
                    number += Convert.ToString(digit);

                return number;

            }

            public void addDelimeter()
            {
                //если запятая уже есть, удаляем её и ставим новую в конце
                if (number.Contains(Const.Sep))
                {
                    number = number.Remove(number.IndexOf(Const.Sep), 1);
                    while (number.StartsWith("0") && !isZero()) 
                        number = number.Remove(0, 1);
                }
                number += Const.Sep;    

            }

            public string Edit(int j)
            {
                if (j < 16)
                {
                    number = addDigit(j);
                }
                else
                switch(j)
                {
                    case 16:
                        addDelimeter();
                        break;

                    case 17:
                        changeSign();
                        break;

                    case 19:
                        Clear();
                        break;

                    case 20:
                        backSpace();
                        break;
                    }

                return number;
            }

            public string AddOprtnSign(int j)
            {
                switch (j)
                {
                    case 21:
                        return "+";

                    case 22:
                        return "-";

                    case 23:
                        return "×";

                    case 24:
                        return "/";

                    case 25:
                        Clear();
                        return "^(-1)";

                    case 26:
                        Clear();
                        return "^2";
                }
                return "";
            }

            public void backSpace()
            {
                if (number.Length>0)
                    number = number.Remove(number.Length - 1, 1);
            }

            public string Clear()
            {
                number = "";
                return number;
            }

        }*/
}
