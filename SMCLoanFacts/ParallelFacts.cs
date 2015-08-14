using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SMCLoanFacts
{    
    [Collection("Demo")]
    public class ParallelFacts1
    {
        [Fact]
        [Trait("category", "para1")]
        public void ThreeSecs()
        {
            System.Threading.Thread.Sleep(3000);
        }        
    }

    [Collection("Demo")]
    public class ParallelFacts2
    {
        [Fact]
        [Trait("category", "para1")]
        public void FiveSecs()
        {
            System.Threading.Thread.Sleep(5000);
        }
    }
}
