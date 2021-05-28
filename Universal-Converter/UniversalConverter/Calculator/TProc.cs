using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalConverter.Calculator
{
    class TProc
    {
        public enum TOprtn { None = 20, Add, Sub, Mul, Dvd }
        public enum TFunc { Rev = 25, Sqrt }
        public TOprtn Oprtn { get; set; }

        public TFunc Func { get; set; }
        public Number Lop_Res { get; set; }
        public Number Rop { get; set; }

        public TProc()
        {
            Oprtn = TOprtn.None;
        }

        public void OprtnClean()
        {
            Oprtn = TOprtn.None;
        }

        public void Reset()
        {
            Lop_Res = new PNumber(0, 10, 1);
            Rop = new PNumber(0, 10, 1);
            Oprtn = TOprtn.None;
        }

        public void OprtnRun()
        {
            switch (Oprtn)
            {
                case TOprtn.None:
                    break;

                case TOprtn.Add:
                    Lop_Res = Lop_Res.Add(Rop);
                    break;

                case TOprtn.Sub:
                    Lop_Res = Lop_Res.Substract(Rop);
                    break;

                case TOprtn.Mul:
                    Lop_Res = Lop_Res.Mul(Rop);
                    break;

                case TOprtn.Dvd:
                    Lop_Res = Lop_Res.Devide(Rop);
                    break;
            }
            
        }

        public void FuncRun()
        {
            switch (Func)
            {
                case TFunc.Sqrt:
                    Lop_Res = Lop_Res.Squared();
                    break;

                case TFunc.Rev:
                    Lop_Res = Lop_Res.Inverse();
                    break;
            }
        }

        public Number GetLopCopy()
        {
            return Lop_Res.Copy();
        }

        public Number GetRopCopy()
        {
            return Rop.Copy();
        }

        public void SetLopCopy(Number E)
        {
            Lop_Res = E.Copy();
        }

        public void SetRopCopy(Number E)
        {
            Rop = E.Copy();
        }
    }
}
