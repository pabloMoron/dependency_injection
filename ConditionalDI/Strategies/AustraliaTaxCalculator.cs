using ConditionalDI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConditionalDI.Strategies
{
    public class AustraliaTaxCalculator : ITaxCalculator
    {
        public int CalculateTax()
        {
            return 10;
        }
    }
}
