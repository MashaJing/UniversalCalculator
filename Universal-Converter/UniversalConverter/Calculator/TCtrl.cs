using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalConverter.Calculator
{
    //управление выполнением команд калькулятора
    class TCtrl
    {
        public enum TCtrlState { cStart, cEditing, FunDone, cValDone, cExpDone, cOpChange, cError };
        /* cStart (Начальное), 
         * cEditing (Ввод и редактирование), 
         * cExpDone (Выражение вычислено), 
         * cOpDone (Операция выполнена), 
         * cValDone (Значение введено), 
         * cOpChange (Операция изменена),
         * cError (Ошибка)
         */

        public TCtrlState CtrlState { get; set; }
        public TAEditor Editor;
        public TProc Proc;
        public TMemory Memory;
        public History his;
        public Number Num;

        public TCtrl()
        {
            CtrlState = TCtrlState.cStart;
            Proc = new TProc();
            Memory = new TMemory();
            his = new History();
            Num = new PNumber(0);
            Editor = new REditor();
        }

        ~TCtrl() {
            CtrlState = TCtrlState.cStart;
            Editor = null;
            Proc = null;
            Memory = null;
            Num = null;

        }

        public string RunEditorCommand(int a)
        {
            if (a == 18)
            {
                //если знак "С", стираем всё
                SetDefaultState();
                Editor.Clear();
                return Editor.Copy();
            }
            else if (a == 20 && CtrlState == TCtrlState.cOpChange || CtrlState == TCtrlState.FunDone)
            {
                //если хотят стереть знак операции
                Proc.Oprtn = TProc.TOprtn.None;
                return Proc.Lop_Res.ToString();
            }
            else
            {
                return Editor.Edit(a);
            }
        }
        
        public string RunMemoryCommand(int j)
        {
            switch (j)
            {
                case 28:
                    Memory.Clear();
                    return Editor.Copy();

                case 29:
                    //if (Memory.St == Calculator.TMemory.State._On)
                    if (CtrlState == TCtrlState.cValDone || CtrlState == TCtrlState.cOpChange)
                    {
                        Proc.Rop = Memory.FNumber.Copy();
                        CtrlState = TCtrlState.cValDone;
                    }
                    else
                    {
                        Proc.Lop_Res = Memory.FNumber.Copy();
                        CtrlState = TCtrlState.cEditing;
                    }
                    break;

                case 30:
                    {
                        Memory.Store(Proc.Lop_Res);
                        return Proc.Lop_Res.ToString();
                    }

                case 31:
                    Memory.Add(Proc.Lop_Res);
                    Proc.Lop_Res = Memory.FNumber.Copy();
                    break;
            }

            return Memory.FNumber.ToString();
        }

        /*public string RunClipBoardCommand(int a, string b)
        {

        }*/

        public string Calculate()
        {

            //если выполнили операцию
            if (Proc.Oprtn != TProc.TOprtn.None)
            {
                //создаем временные переменные для записи в историю левого операнда и id операции
                string Lop_temp = Proc.Lop_Res.ToString();
                int operation_id = (int)Proc.Oprtn;

                //производим операцию
                Proc.OprtnRun();

                //записываем операцию в историю
                his.AddRecord(operation_id, Lop_temp, Proc.Rop.ToString(),
                     Proc.Lop_Res.ToString());
                
                CtrlState = TCtrlState.cExpDone;
                //Proc.Rop.SetValue("0");
            }
            //если выполнили функцию
            else
            {
                //создаем временные переменные для записи в историю левого операнда и id функции
                string Lop_temp = Proc.Lop_Res.ToString();

                //выисляем функцию
                Proc.FuncRun();

                //записываем операцию в историю
                his.AddRecord((int)Proc.Func, Lop_temp, Proc.Rop.ToString(), 
                    Proc.Lop_Res.ToString());

                CtrlState = TCtrlState.FunDone;
                Proc.OprtnClean();
            }

            return Proc.GetLopCopy().ToString();
        }

        public void SetDefaultState() 
        {
            //TCtrl
            CtrlState = TCtrlState.cEditing;
            //TProc
            Proc.Oprtn = TProc.TOprtn.None;
         //   Proc.Lop_Res.SetValue("0,");
          //  Proc.Rop.SetValue("0");
            
        }

        public void SetOprtnFunc(int i)
        {
            
            if (i <= 24)
            {
                Proc.Oprtn = (TProc.TOprtn)i;
                //устанавливаем контроллеру статус "смена операции"
                CtrlState = TCtrlState.cOpChange;
            }
            else if (i <= 26)
            { 
                Proc.Func = (TProc.TFunc)i;

                //если хотим поменять в правом операнде
                if (CtrlState == TCtrlState.cValDone)
                {
                    switch (i)
                    {
                        case 25:
                            Proc.Rop = Proc.Rop.Inverse();
                            break;

                        case 26:
                            Proc.Rop = Proc.Rop.Squared();
                            break;
                    }
                }
                //устанавливаем контроллеру статус "функция установлена"
                CtrlState = TCtrlState.FunDone;

            }
        }
    }
}
