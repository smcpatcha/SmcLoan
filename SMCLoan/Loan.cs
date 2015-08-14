using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMCLoan
{
    public class Loan
    {
        public decimal Principal { get; set; }
        public decimal Outstanding { get; set; }
        public DateTime LastPayDate { get; set; }
        public Bank PreviousOwner { get; set; }
        public decimal Accrued { get; set; }
        public List<Payment> Payments { get; set; }

        public Loan()
            : this(0, 0, null, new DateTime())
        {
        }

        public Loan(decimal principal, decimal outstanding, Bank previousOwner, DateTime lastPayDate)
        {
            this.Principal = principal;
            this.Outstanding = outstanding;
            this.PreviousOwner = previousOwner;
            this.LastPayDate = lastPayDate;
            Payments = new List<Payment>();
        }

        public Payment Pay(DateTime date, decimal amount)
        {
            if (date < LastPayDate)
            {
                string s = String.Format("paid data can not before the last pay date, last pay date is {0}.", LastPayDate.ToString("yyyy-MM-dd"));
                throw new ArgumentException(s, "date");
            }
            Payment payment = new Payment();
            TimeSpan ts = date - LastPayDate;
            int days = ts.Days;
            decimal interestAmount = Math.Round(this.Outstanding * (decimal)PreviousOwner.InterestRate * days / 36500, 2, MidpointRounding.AwayFromZero);
            interestAmount += Accrued;
            payment.Days = days;
            payment.PaidAmount = amount;
            payment.PaidDate = date;

            if (amount >= interestAmount)
            {
                payment.InterestAmount = interestAmount;
                Accrued = 0;
            }
            else
            {
                payment.InterestAmount = amount;
                Accrued = interestAmount - amount;
            }

            payment.PaidPrincipalAmount = amount - payment.InterestAmount;
            payment.Outstanding = this.Outstanding - payment.PaidPrincipalAmount;
            this.LastPayDate = date;
            this.Outstanding = payment.Outstanding;


            Payments.Add(payment);

            return payment;
        }
    }
}
