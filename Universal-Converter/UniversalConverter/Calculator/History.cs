using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalConverter.Calculator
{
    //Структура для записи преобразования
    [Serializable]
    public struct Record
    {
        string operation;
        string number1;
        string number2;
        public string result;
        public Record(string operation, string n1, string n2, string result)
        {
            this.number1 = n1;
            this.number2 = n2;
            this.operation = operation;
            this.result = result;
        }

        public override string ToString()
        {
            return String.Format("{0} {2} {1} <=> {3}", number1, number2, operation, result);
        }
    }

    public class History
    {

        BinaryFormatter formatter = new BinaryFormatter();
        List<Record> L;

        //Чтение истории из файла
        public History()
        {

            using (FileStream fs = new
                FileStream("C:/Users/m_dze/source/repos/Universal-Converter/Universal-Converter/UniversalConverter/Data/ConverterHistory.dat",
                FileMode.OpenOrCreate))
            {
                try
                {
                    L = (List<Record>)formatter.Deserialize(fs);
                }
                catch (Exception e)
                {
                    L = new List<Record>();
                }
                //formatter.Serialize(fs, L);
            }

        }

        //Запись истории в файл
        ~History()
        {
            using (FileStream fs = new
                FileStream("C:/Users/m_dze/source/repos/Universal-Converter/Universal-Converter/UniversalConverter/Data/CalculatorHistory.dat",
                FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, L);
            }
        }

        //Геттер по индексу
        public Record GetRecord(int i)
        {
            if (i < L.Count && i >= 0)
                return L[i];
            else return new Record();
        }

        //Добавление записи
        public void AddRecord(int operation_id, string n1, string n2, string result)
        {
            string n2_actual = n2;
            string operation = "";
            switch (operation_id)
            {
                case 21:
                    operation = "+";
                    break;
                case 22:
                    operation = "-";
                    break;
                case 23:
                    operation = "x";
                    break;
                case 24:
                    operation = "/";
                    break;
                case 25:
                    operation = "^(-1)";
                    n2_actual = " ";
                    break;
                case 26:
                    operation = "^2";
                    n2_actual = " ";
                    break;
            }

            Record newRecord = new Record(operation, n1, n2_actual, result);
            L.Add(newRecord);
        }

        //Очитска Истории
        public void Clear()
        {
            L.Clear();
        }


        public int Count()
        {
            return L.Count;
        }
    }
}
