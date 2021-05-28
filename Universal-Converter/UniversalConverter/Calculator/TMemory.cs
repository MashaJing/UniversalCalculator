using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalConverter.Calculator
{

    class TMemory
    {

        public Number FNumber;

        //текущее состояние памяти
        public enum State { _Off, _On };

        public State St { set; get; }

        public TMemory()
        {
            FNumber = new PNumber(0);
            St = State._Off;
        }

        public void Store(Number E)
        {
            FNumber = E.Copy();
            St = State._On;
        }

        public string GetNumberCopy()
        {
            St = State._On;
            return FNumber.ToString();
        }

        public void Add(Number E)
        {
            FNumber = FNumber.Add(E);
            St = State._On;
        }

        public string GetMemoryState()
        {
            return Convert.ToString(St);
        }

        public void Clear()
        {
            St = State._Off;
        }
    }
}
