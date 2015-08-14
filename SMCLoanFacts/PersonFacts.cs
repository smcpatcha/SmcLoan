using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SMCLoanFacts
{
    public class Person
    {
        public double WeightKg { get; set; }
        public double HeightCm { get; set; }

        public double BMI
        {
            get
            {
                return Math.Round(WeightKg / Math.Pow(HeightCm / 100, 2), 2, MidpointRounding.AwayFromZero);
            }
        }
    }

    public class PersonFacts
    {
        public class BMIProperty
        {
            [Theory]
            [InlineData(50, 160, 19.53)]
            [InlineData(60, 160, 23.44)]
            [InlineData(70, 160, 27.34)]
            public void CalculateBmi(double kg, double cm, double bmi)
            {
                Person person = new Person();
                person.WeightKg = kg;
                person.HeightCm = cm;

                var actual = person.BMI;

                Assert.Equal(bmi, actual);
            }
        }
    }
}
