using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversalConverter
{

    public partial class CalculatorForm : Form
    {
        //объект, через который можно обращаться к процессору, памяти и редактору
        Calculator.TCtrl ctl = new Calculator.TCtrl();

        public CalculatorForm()
        {
            InitializeComponent();
        }

        public enum TMode { PNumber, Frac, Complex }
        public TMode Mode = TMode.PNumber;
        //инициализация лейблов
        private void CalculatorForm_Load(object sender, EventArgs e)
        {
            //инициализация лейблов
            ToolTip t = new ToolTip();
            t.SetToolTip(EnterLabel, "Введите выражение");
            
            ToolTip t1 = new ToolTip();
            t1.SetToolTip(NumSystemTrackBar, "Изменить систему счисления");
            
            ToolTip t2 = new ToolTip();
            t2.SetToolTip(PrecisionTrackBar, "Изменить точность вычислений");

            ToolTip tMS = new ToolTip();
            tMS.SetToolTip(button18, "Сохранить в память");

            ToolTip tMR = new ToolTip();
            tMR.SetToolTip(MRButton, "Прочитать из памяти");

            ToolTip tMPlus = new ToolTip();
            tMPlus.SetToolTip(button24, "Прибавить к числу в памяти");

            ToolTip tMC = new ToolTip();
            tMC.SetToolTip(MCbutton, "Очистить память");
            
            ToolTip tC = new ToolTip();
            tC.SetToolTip(rButton, "Стереть всё");

            ToolTip tBS = new ToolTip();
            tBS.SetToolTip(BackSpaceButton, "Удалить один символ");

            ToolTip tCE = new ToolTip();
            tCE.SetToolTip(CEButton, "Очистить операнд");

            EnterLabel.Text = "0";
            radioButtonPNumber.Checked = true;
            ctl.Num = new PNumber(0);
            Activate_PNum();

            //Основание с.сч. исходного числа р1.
            NumSystemTrackBar.Value = 10;
            radioButtonIm.Enabled = false;
            radioButtonRe.Enabled = false;
            NumSystemLabel.Text = String.Format("{0}", NumSystemTrackBar.Value);
            
            //Основание с.сч. результата р2.
            PrecisionTrackBar.Value = 1;
            PrecisionLabel.Text = String.Format("{0}", PrecisionTrackBar.Value);
            UpdateButtons();
        }

        
        //деактивация недоступных для данной с.с кнопок
        private void UpdateButtons()
        {
            //EnterLabel.Text = "0";
            //просмотреть все компоненты формы
            foreach (Control i in panel2.Controls)
            {
                if (i is Button)//текущий компонент - командная кнопка 
                {
                    int j = Convert.ToInt16(i.Tag.ToString());
                    if (j < NumSystemTrackBar.Value)
                    {
                        //сделать кнопку доступной
                        i.Enabled = true;
                    }
                    if ((j >= NumSystemTrackBar.Value) && (j <= 15))
                    {
                        //сделать кнопку недоступной
                        i.Enabled = false;
                    }
                }
            }
        }


        //реакция на ввод с клавиатуры
        private void CalcFormKeyPress(object sender, KeyPressEventArgs e)
        {
            int i = -1;
            if (e.KeyChar >= 'A' && e.KeyChar <= 'F') i = (int)e.KeyChar - 'A' + 10;
            if (e.KeyChar >= 'a' && e.KeyChar <= 'f') i = (int)e.KeyChar - 'a' + 10;
            if (e.KeyChar >= '0' && e.KeyChar <= '9') i = (int)e.KeyChar - '0';
            if (e.KeyChar == '.' || e.KeyChar == ',') i = 16;
            if ((int)e.KeyChar == 8) i = 20;
            if ((int)e.KeyChar == 13) i = 27;
            if (( Mode == TMode.PNumber && i < (ctl.Proc.Lop_Res as PNumber).b) || i < 10 || (i >= 16)) DoCmnd(i);

        }

        private void DoCmnd(int j)
       {
            try
            {
                if (j < 0) return;

                //обработка команд редактора
                if (j <= 20)
                {
                    if (j < 16 && ((this.Mode != TMode.Complex && EnterLabel.Text.Length > 10) && !(ctl.CtrlState == Calculator.TCtrl.TCtrlState.cOpChange) || (this.Mode == TMode.Complex && EnterLabel.Text.Length > 19)))
                    {
                        MessageBox.Show("Достигнута максимальная длина", "Ошибка", MessageBoxButtons.OK);
                    }

                    else
                    {
                        EnterLabel.Text = ctl.RunEditorCommand(j);

                        //если не было установлено никакой операции
                        if (ctl.CtrlState == Calculator.TCtrl.TCtrlState.cEditing || ctl.CtrlState == Calculator.TCtrl.TCtrlState.cStart || ctl.CtrlState == Calculator.TCtrl.TCtrlState.cExpDone)
                        {
                            switch (Mode)
                            {
                                case (TMode.Complex):
                                    //преобразование из строки в редакторе в операнды (и проверки)
                                    string a = (ctl.Editor as Calculator.CEditor).re;
                                    string b = (ctl.Editor as Calculator.CEditor).im;
                                    if (a.Length == 0) a = "0";
                                    if (b.Length == 0) b = "0";
                                    ctl.Proc.Lop_Res = new Comp(Convert.ToDouble(a), Convert.ToDouble(b));
                                    break;
                                case (TMode.Frac):
                                    string num = (ctl.Editor as Calculator.FEditor).num;
                                    string dnom = (ctl.Editor as Calculator.FEditor).dnom;
                                    if (num.Length == 0) num = "0";
                                    if (dnom.Length == 0) dnom = "0";
                                    ctl.Proc.Lop_Res = new Frac(Convert.ToInt32(num), Convert.ToInt32(dnom));
                                    break;
                                case (TMode.PNumber):
                                    int basement = (ctl.Proc.Lop_Res as PNumber).b;
                                    int prec = (ctl.Proc.Lop_Res as PNumber).c;
                                    ctl.Proc.Lop_Res = new PNumber((ctl.Editor as Calculator.REditor).ToDouble(basement, prec), basement, prec);
                                    break;
                            }

                         //   ctl.Proc.Lop_Res = ctl.Editor.number;
                            ctl.CtrlState = Calculator.TCtrl.TCtrlState.cEditing;
                        }

                        //вводится правый операнд
                        else if (ctl.CtrlState == Calculator.TCtrl.TCtrlState.cOpChange || ctl.CtrlState == Calculator.TCtrl.TCtrlState.cValDone)
                        {
                            switch (Mode)
                            {
                                case (TMode.Complex):
                                   string a = (ctl.Editor as Calculator.CEditor).re;
                                    string b = (ctl.Editor as Calculator.CEditor).im;
                                    if (a.Length == 0) a = "0";
                                    if (b.Length == 0) b = "0";
                                    ctl.Proc.Rop = new Comp(Convert.ToDouble(a), Convert.ToDouble(b));
                                    break;
                                case (TMode.Frac):
                                    string num = (ctl.Editor as Calculator.FEditor).num;
                                    string dnom = (ctl.Editor as Calculator.FEditor).dnom;
                                    if (num.Length == 0) num = "0";
                                    if (dnom.Length == 0) dnom = "0";
                                    ctl.Proc.Rop = new Frac(Convert.ToInt32(num), Convert.ToInt32(dnom));
                                    break;
                                case (TMode.PNumber):
                                    int basement = (ctl.Proc.Rop as PNumber).b;
                                    int prec = (ctl.Proc.Rop as PNumber).c;
                                    ctl.Proc.Rop = new PNumber((ctl.Editor as Calculator.REditor).ToDouble(basement, prec), basement, prec);
                                    break;
                            }
                            ctl.CtrlState = Calculator.TCtrl.TCtrlState.cValDone;
                        }
                    }
                    return;
                }

                //обработка команд процессора: если редактирование происходит сейчас || хотим вычислить ф-ию для правого операнда
                else if ((j < 27 && (ctl.CtrlState == Calculator.TCtrl.TCtrlState.cEditing)) || (j>24 && j<27 && (ctl.CtrlState == Calculator.TCtrl.TCtrlState.cValDone)))
                {
                    if (this.Mode == TMode.Frac)
                    {
                        if ((ctl.Editor as Calculator.FEditor).dnom == "" || (ctl.Editor as Calculator.FEditor).dnom == "0")
                        {
                            throw new Exception("Знаменатель не может быть нулевым");
                        }
                    }
                    //устанавливаем соответствующую операцию или функцию в процессоре
                    ctl.SetOprtnFunc(j);
                    //добавляем в строку знак операции
                    EnterLabel.Text += ctl.Editor.AddOprtnSign(j);
                    //очищаем строку числа в редакторе
                    ctl.Editor.Clear();
                    return;
                }

                else if (j == 27)
                {
                    if (this.Mode == TMode.Frac)
                    {
                        if ((ctl.Editor as Calculator.FEditor).dnom == "" || (ctl.Editor as Calculator.FEditor).dnom == "0")
                        {
                            throw new Exception("Знаменатель не может быть нулевым");
                        }
                    }
                    //функция Calculate() обновляет значения операндов и возвращает 
                    //строку результата вычисления установленой функции/операции
                    if (ctl.CtrlState == Calculator.TCtrl.TCtrlState.cOpChange)
                        ctl.Proc.Rop = ctl.Proc.Lop_Res.Copy();
                    // && (ctl.CtrlState == Calculator.TCtrl.TCtrlState.cValDone || ctl.CtrlState == Calculator.TCtrl.TCtrlState.FunDone || ctl.CtrlState == Calculator.TCtrl.TCtrlState.cOpChange)
                    ctl.Editor.number = ctl.Calculate();
                    EnterLabel.Text = ctl.Editor.number;
                    //устанавливаем контроллеру исходный статус
                    ctl.CtrlState = Calculator.TCtrl.TCtrlState.cEditing;
                    return;
                }
                //обработка команд памяти
                else if (j > 27)
                {
                    if (j == 28)
                    {
                        ctl.RunMemoryCommand(j);
                        switch (Mode)
                        {
                            case TMode.Complex:
                                ctl.Memory.FNumber = new Comp(0, 0, Convert.ToInt32(PrecisionLabel.Text));
                                break;

                            case TMode.Frac:
                                ctl.Memory.FNumber = new Frac(0, 1);
                                break;

                            case TMode.PNumber:
                                ctl.Memory.FNumber = new PNumber(0, Convert.ToInt32(NumSystemLabel.Text), Convert.ToInt32(PrecisionLabel.Text));
                                break;
                        }
                        
                    }
                    else ctl.Editor.number = ctl.RunMemoryCommand(j);
                    EnterLabel.Text = ctl.Editor.number;
                    return;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK);
            }
        }

        ///обработка трекбаров
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            NumSystemLabel.Text = String.Format("{0}", tb.Value);

            (ctl.Proc.Lop_Res as PNumber).b = tb.Value;
            (ctl.Proc.Rop as PNumber).b = tb.Value;
            (ctl.Memory.FNumber as PNumber).b = tb.Value;
            if (ctl.Proc.Lop_Res.ToString() != "-0,0")
                ctl.Editor.number = ctl.Proc.Lop_Res.ToString();
            else
                ctl.Editor.number = "0";
            EnterLabel.Text = ctl.Editor.number;
            //ctl.CtrlState = Calculator.TCtrl.TCtrlState.cStart;
            UpdateButtons();
            
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            PrecisionLabel.Text = String.Format("{0}", tb.Value);

            switch (Mode)
            {
                case TMode.PNumber:
                    (ctl.Proc.Lop_Res as PNumber).c = tb.Value;
                    (ctl.Proc.Rop as PNumber).c = tb.Value;
                    break;
                case TMode.Complex:
                    (ctl.Proc.Lop_Res as Comp).c = tb.Value;
                    (ctl.Proc.Rop as Comp).c = tb.Value;
                    break;
            }
        }

            //////////////////

            ///обработка всех кнопок
        private void button1_Click(object sender, EventArgs e)
        {
            //ссылка на компонент, на котором кликнули мышью
            Button but = (Button)sender;
            //номер выбранной команды
            int j = Convert.ToInt16(but.Tag.ToString());
            DoCmnd(j);

        }

        ///запуск других форм
        private void EnterLabel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true && e.KeyCode == Keys.C)
            {
                string s = EnterLabel.Text;
                Clipboard.SetData(DataFormats.StringFormat, s);
            }
        }

        private void историяToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            HistoryForm hisForm = new HistoryForm(ctl.his);
            hisForm.Show();
        }

        private void справкаStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ReferenceForm refForm = new ReferenceForm();
            refForm.Show();
        }

        private void правкаToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AppForm convForm = new AppForm();
            convForm.Show();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        //работа с буфером
        private void копироватьStripMenuItem_Click(object sender, EventArgs e)
        {
            //если вводили левый операнд
            if (ctl.CtrlState != Calculator.TCtrl.TCtrlState.cValDone || ctl.Proc.Oprtn == Calculator.TProc.TOprtn.None)
                Calculator.TClipBoard.BUFFER = ctl.Proc.Lop_Res.Copy();
            //если вводили правый операнд
            else
                Calculator.TClipBoard.BUFFER = ctl.Proc.Rop.Copy();
        }

        private void вставитьStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Mode != TMode.PNumber || (Calculator.TClipBoard.BUFFER as PNumber).b != 0)
            {
                //если вводили левый операнд ctl.Proc.Oprtn == Calculator.TProc.TOprtn.None && (ctl.CtrlState == Calculator.TCtrl.TCtrlState.cEditing || ctl.CtrlState == Calculator.TCtrl.TCtrlState.cStart)
                if (ctl.CtrlState == Calculator.TCtrl.TCtrlState.cStart || ctl.CtrlState == Calculator.TCtrl.TCtrlState.cEditing)
                {
                    ctl.Proc.Lop_Res = Calculator.TClipBoard.BUFFER.Copy();
                    EnterLabel.Text = ctl.Proc.Lop_Res.ToString();
                    ctl.CtrlState = Calculator.TCtrl.TCtrlState.cEditing;
                }
                //если вводили правый операнд  if (ctl.CtrlState == Calculator.TCtrl.TCtrlState.cValDone || ctl.CtrlState == Calculator.TCtrl.TCtrlState.FunDone)
                else
                {
                    ctl.Proc.Rop = Calculator.TClipBoard.BUFFER.Copy();
                    EnterLabel.Text = ctl.Proc.Rop.ToString();
                    ctl.CtrlState = Calculator.TCtrl.TCtrlState.cValDone;
                }
            }
        }


        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        //обработка режимов калькулятора
        private void radioButtonPNumber_CheckedChanged(object sender, EventArgs e)
        {
            Activate_PNum();
            //деактивируем все нужные кнопки
            radioButtonIm.Enabled = false;
            radioButtonRe.Enabled = false;
            radioButtonNum.Enabled = false;
            radioButtonDnom.Enabled = false;
            PrecisionTrackBar.Enabled = true;

            //заботимся обо всех лишних кнопках
            radioButtonCompex.Checked = false;
            radioButtonFrac.Checked = false;
            NumSystemTrackBar.Enabled = true;
            CommaButton.Enabled = true;

            //Обновляем все поля процессора
            ctl.Proc.Lop_Res = new PNumber(0);
            ctl.Proc.Rop = new PNumber(0);
            ctl.Num = new PNumber(0);
            ctl.Editor = new Calculator.REditor();
            
            //меняем состояния
            Mode = TMode.PNumber;
            ctl.CtrlState = Calculator.TCtrl.TCtrlState.cStart;

        }
        private void radioButtonCompex_CheckedChanged(object sender, EventArgs e)
        {
            Activate_Comp();
            //активируем все кнопки для комплексного числа
            radioButtonIm.Enabled = true;
            radioButtonRe.Enabled = true;
            radioButtonNum.Enabled = false;
            radioButtonDnom.Enabled = false;
            CommaButton.Enabled = true;
            PrecisionTrackBar.Enabled = true;

            //переводим с.с. в 10-чную, чтобы отображались все кнопки
            NumSystemTrackBar.Value = 10;
            UpdateButtons();

            //заботимся обо всех лишних кнопках
            radioButtonPNumber.Checked = false;
            radioButtonFrac.Checked = false;
            NumSystemTrackBar.Enabled = false;

            //Обновляем все поля процессора
            ctl.Proc.Lop_Res = new Comp(0,0);
            ctl.Proc.Rop = new Comp(0,0);
            ctl.Num = new Comp(0, 0);
            ctl.Editor = new Calculator.CEditor();
            radioButtonRe.Checked = true;

            //меняем состояния
            Mode = TMode.Complex;
            ctl.CtrlState = Calculator.TCtrl.TCtrlState.cStart;
        }
        private void radioButtonFrac_CheckedChanged(object sender, EventArgs e)
        {
            Activate_Frac();
            //деактивируем все нужные кнопки
            radioButtonIm.Enabled = false;
            radioButtonRe.Enabled = false;
            radioButtonNum.Enabled = true;
            radioButtonDnom.Enabled = true;
            PrecisionTrackBar.Enabled = false;

            //переводим с.с. в 10-чную, чтобы отображались все кнопки
            NumSystemTrackBar.Value = 10;
            UpdateButtons();

            //заботимся обо всех лишних кнопках
            CommaButton.Enabled = false;
            radioButtonCompex.Checked = false;
            radioButtonPNumber.Checked = false;
            NumSystemTrackBar.Enabled = false;

            //Обновляем все поля процессора
            ctl.Proc.Lop_Res = new Frac(0,1);
            ctl.Proc.Rop = new Frac(0, 1);
            ctl.Num = new Frac(0, 1);
            ctl.Editor = new Calculator.FEditor();

            //меняем состояния
            Mode = TMode.Frac;
            radioButtonNum.Checked = true;
            ctl.CtrlState = Calculator.TCtrl.TCtrlState.cStart;
        }

        //обработка режимов комплексного калькулятора
        private void radioButtonRe_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonIm.Checked = false;
            (ctl.Editor as Calculator.CEditor).Mode = Calculator.CEditor.mode.re;
        }
        private void radioButtonIm_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonRe.Checked = false;
            (ctl.Editor as Calculator.CEditor).Mode = Calculator.CEditor.mode.im;
        }

        //обработка режимов калькулятора дробей
        private void radioButtonNum_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonDnom.Checked = false;
            (ctl.Editor as Calculator.FEditor).Mode = Calculator.FEditor.mode.num;
        }
        private void radioButtonDnom_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonNum.Checked = false;
            (ctl.Editor as Calculator.FEditor).Mode = Calculator.FEditor.mode.dnom;
        }

        //смена цветов панелей
        private void Activate_Frac()
        {
            panelPNum.BackColor = Color.LightGray;
            panelComp.BackColor = Color.LightGray;
            panelFrac.BackColor = Color.FromArgb(192, 255, 192);
            NumSystemTrackBar.BackColor = Color.LightGray;
        }
        private void Activate_Comp()
        {
            panelPNum.BackColor = Color.LightGray;
            panelFrac.BackColor = Color.LightGray;
            panelComp.BackColor = Color.FromArgb(192, 255, 192);
            NumSystemTrackBar.BackColor = Color.LightGray;         
        }
        private void Activate_PNum()
        {
            panelComp.BackColor = Color.LightGray;
            panelFrac.BackColor = Color.LightGray;
            panelPNum.BackColor = Color.FromArgb(192, 255, 192);
            NumSystemTrackBar.BackColor = Color.FromArgb(192, 255, 192);
        }

    }
}
