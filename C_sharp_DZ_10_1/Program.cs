using System;
using static System.Console;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
/*Разработать класс «Счет для оплаты». В классе предусмотреть следующие поля:
■ оплата за день;
■ количество дней;
■ штраф за один день задержки оплаты;
■ количество дней задержи оплаты;
■ сумма к оплате без штрафа (вычисляемое поле);
■ штраф (вычисляемое поле);
■ общая сумма к оплате (вычисляемое поле).
В классе объявить статическое свойство типа bool, значение которого влияет на процесс форматирования объектов этого класса.
Если значение этого свойства равно true, тогда сериализуются и десериализируются все поля,
если false — вычисляемые поля не сериализуются. Разработать приложение, в котором необходимо продемонстрировать
использование этого класса, результаты должны записываться и считываться из файла.*/
namespace C_sharp_DZ_10_1
{
    
    public class PropertyAttribute : Attribute
    {
        
        public static bool FormatSer { get; set; }

        public PropertyAttribute()
        { }

        //public PropertyAttribute(bool formatSer)
        //{
        //    FormatSer = formatSer;
        //}
    }




    [Serializable]
    public class Invoice : ISerializable
    {
        public decimal DayPayment { get; set; } //оплата за день (гр)
        public int NumbersOfDays { get; set; } //количество дней (шт)
        public double DayPenaltyForLate { get; set; } //штраф за один день задержки оплаты (%)
        public int NumbersOfDaysForLate { get; set; } //количество дней задержи оплаты (шт)
        public decimal PaymentWithoutPenalty //сумма к оплате без штрафа (гр)
        {
            get { return DayPayment * NumbersOfDays; }
            set { }
        }
        public decimal Penalty //штраф
        {
            get { return DayPayment * (decimal)DayPenaltyForLate / 100 * NumbersOfDaysForLate; }
            set { }
        }
        public decimal TotalPayment //общая сумма к оплате
        {
            get { return PaymentWithoutPenalty + Penalty; }
            set { }
        }

        public static bool formatSerializable;

        public Invoice() { }
        public Invoice(decimal dayPayment, int numbersOfDays, double dayPenaltyForLate, int numbersOfDaysForLate)
        {
            DayPayment = dayPayment;
            NumbersOfDays = numbersOfDays;
            DayPenaltyForLate = dayPenaltyForLate;
            NumbersOfDaysForLate = numbersOfDaysForLate;
        }

        private Invoice(SerializationInfo info, StreamingContext context)
        {
            DayPayment = info.GetDecimal("DayPayment");
            NumbersOfDays = info.GetInt32("NumbersOfDays");
            DayPenaltyForLate = info.GetDouble("DayPenaltyForLate");
            NumbersOfDaysForLate = info.GetInt32("NumbersOfDaysForLate");
            if (formatSerializable)
            {
                PaymentWithoutPenalty = info.GetDecimal("PaymentWithoutPenalty");
                Penalty = info.GetDecimal("Penalty");
                TotalPayment = info.GetDecimal("TotalPayment");
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DayPayment", DayPayment);
            info.AddValue("NumbersOfDays", NumbersOfDays);
            info.AddValue("DayPenaltyForLate", DayPenaltyForLate);
            info.AddValue("NumbersOfDaysForLate", NumbersOfDaysForLate);
            if (formatSerializable)
            {
                info.AddValue("PaymentWithoutPenalty", PaymentWithoutPenalty);
                info.AddValue("Penalty", Penalty);
                info.AddValue("TotalPayment", TotalPayment);
            }
        }


        
        public override string ToString()
        {
            return
                $"Оплата за день: {DayPayment:C2}\n" +
                $"Количество дней: {NumbersOfDays}\n" +
                $"Штраф за один день задержки оплаты: {DayPenaltyForLate} %\n" +
                $"Количество дней задержи оплаты: {NumbersOfDaysForLate}\n" +
                $"Сумма к оплате без штрафа: {PaymentWithoutPenalty:C2}\n" +
                $"Штраф: {Penalty:C2}\n" +
                $"ОБЩАЯ СУММА К ОПЛАТЕ: {TotalPayment:C2}\n";
        }

    }



    class Program
    {


        static void Main(string[] args)
        {
            Title = "C_sharp_DZ_10_1";
            string fileName = "invoices_serial.soap";
            List<Invoice> invoices = new List<Invoice>()
            {
                new Invoice(35.12m, 12, 3.5, 3),
                new Invoice(15.62m, 10, 2.5, 4),
                //new Invoice(56.16m, 15, 2.3, 2),
                //new Invoice(19.78m, 18, 4.1, 4)
            };
            foreach (Invoice inv in invoices) WriteLine(inv);


            SoapFormatter soapFormat = new SoapFormatter(/*typeof(List<Invoice>)*/);
            try
            {
                using (Stream fStream = File.Create(fileName))
                {
                    soapFormat.Serialize(fStream, invoices);
                }
                WriteLine("_______________________________________________SoapSerialize OK!\n");

                List<Invoice> invoicesAfterDeserialized = null;
                using (Stream fStream = File.OpenRead(fileName))
                {
                    invoicesAfterDeserialized = (List<Invoice>)soapFormat.Deserialize(fStream);
                }
                foreach (Invoice item in invoicesAfterDeserialized)
                {
                    WriteLine(item);
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex);
            }


            Invoice.formatSerializable = true;
            





            ReadKey();
        }
    }
}
