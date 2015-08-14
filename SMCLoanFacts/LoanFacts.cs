using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SMCLoan;
using Xunit.Abstractions;
using Should;

namespace SMCLoanFacts
{
    public class LoanFacts
    {
        public class PayMethod
        {
            private ITestOutputHelper output;
            public PayMethod(ITestOutputHelper output)
            {
                this.output = output;
            }

            [Fact]
            public void DifferentYearPayment_HaveCorrectDay()
            {
                Bank bank = new Bank(name: "SMC", interestRate: 7.0);
                var loan = new Loan(principal: 1000000, outstanding: 1000000, previousOwner: bank, lastPayDate: new DateTime(2014, 12, 20));
                var p = loan.Pay(new DateTime(2015, 1, 20), 10000);

                outputPayment(p);

                //Assert.Equal(31, p.Days);
                p.Days.ShouldEqual(31);
            }

            private void outputPayment(Payment p)
            {
                output.WriteLine("{0:s}\t{1} days\r\nPaid: {2:n2}\r\nPrincipal: {3:n2}\r\nInterest: {4:n2}\r\nOutstanding: {5:2}",
                    p.PaidDate, p.Days, p.PaidAmount, p.PaidPrincipalAmount, p.InterestAmount, p.Outstanding);
            }

            [Fact]
            public void FirstPay()
            {
                //arrange
                Bank bank = new Bank(name: "SMC", interestRate: 7.0);

                Loan loan = new Loan(
                    principal: 1000000, 
                    outstanding: 1000000, 
                    previousOwner: bank, 
                    lastPayDate: new DateTime(2015, 7, 1));

                //act
                var d = new DateTime(2015, 8, 1);
                var payment = loan.Pay(d, 10000);

                //assert                
                //Assert.NotNull(payment);
                payment.ShouldNotBeNull();
                //Assert.Equal(31, payment.Days);
                payment.Days.ShouldEqual(31);
                Assert.Equal(10000, payment.PaidAmount);
                Assert.Equal(d, payment.PaidDate);

                Assert.Equal(5945.21m, payment.InterestAmount);
                Assert.Equal(4054.79m, payment.PaidPrincipalAmount);
                Assert.Equal(995945.21m, payment.Outstanding);

                Assert.Equal(d, loan.LastPayDate);
                Assert.Equal(995945.21m, loan.Outstanding);
            }

            [Fact]
            public void PayTwiceInTheSameDay()
            {
                // arrange       
                Loan loan = new Loan(
                  principal: 1000000,
                  outstanding: 1000000,
                  previousOwner: new Bank("TEST BANK", 7.0),
                  lastPayDate: new DateTime(2015, 7, 1));          

                // act
                var d = new DateTime(2015, 8, 1);
                var payment1 = loan.Pay(d, 10000);
                var payment2 = loan.Pay(d, 10000);

                // assert
                Assert.NotNull(payment2);
                Assert.Equal(0, payment2.Days);
                Assert.Equal(10000, payment2.PaidAmount);
                Assert.Equal(d, payment2.PaidDate);

                Assert.Equal(0m, payment2.InterestAmount);
                Assert.Equal(10000.00m, payment2.PaidPrincipalAmount);
                Assert.Equal(985945.21m, payment2.Outstanding);

                Assert.Equal(d, loan.LastPayDate);
                Assert.Equal(985945.21m, loan.Outstanding);
            }

            [Fact]
            public void PaidAmoutIsNotEnoughForInterest()
            {
                // arrange       
                Loan loan = new Loan(
                  principal: 1000000,
                  outstanding: 1000000,
                  previousOwner: new Bank("TEST BANK", 7.0),
                  lastPayDate: new DateTime(2015, 7, 1));

                // act
                var d1 = new DateTime(2015, 8, 1);
                var payment1 = loan.Pay(d1, 3000);
                
                // assert

                Assert.NotNull(d1);
                Assert.Equal(31, payment1.Days);
                Assert.Equal(3000m, payment1.PaidAmount);
                Assert.Equal(d1, payment1.PaidDate);

                Assert.Equal(3000m, payment1.InterestAmount);
                Assert.Equal(0m, payment1.PaidPrincipalAmount);
                Assert.Equal(1000000m, payment1.Outstanding);

                Assert.Equal(2945.21m, loan.Accrued);

                var d2 = new DateTime(2015, 9, 1);
                var payment2 = loan.Pay(d2, 10000);

                Assert.NotNull(payment2);
                Assert.Equal(31, payment2.Days);
                Assert.Equal(10000, payment2.PaidAmount);
                Assert.Equal(d2, payment2.PaidDate);

                Assert.Equal(8890.42m, payment2.InterestAmount);
                Assert.Equal(1109.58m, payment2.PaidPrincipalAmount);
                Assert.Equal(998890.42m, payment2.Outstanding);
                Assert.Equal(0m, loan.Accrued);

                Assert.Equal(d2, loan.LastPayDate);
                Assert.Equal(998890.42m, loan.Outstanding);
            }

            [Fact]
            public void PaidDateISBeforeLastPayDate_ThrowsException()
            {
                Loan loan = new Loan(
                    principal: 1000000,
                    outstanding: 1000000,
                    previousOwner: new Bank("TEST", 7.0),
                    lastPayDate: new DateTime(2015, 7, 1));

                var ex = Assert.ThrowsAny<Exception>(() =>
                {
                    loan.Pay(new DateTime(2015, 6, 15), 10000m);
                });

                Assert.Contains("last pay date is 2015-07-01", ex.Message);
            }

            [Fact]
            public void LoanShouldStorePaymentAfterPaid()
            {
                Loan loan = new Loan(
                       principal: 1000000,
                       outstanding: 1000000,
                       previousOwner: new Bank("TEST", 7.0),
                       lastPayDate: new DateTime(2015, 7, 1));
                Payment p = loan.Pay(new DateTime(2015, 8, 1), 10000m);

                loan.Payments.ShouldNotBeNull();
                loan.Payments.Count().ShouldEqual(1);
                loan.Payments.ShouldContain(p);
            } 
        }
    }
}
