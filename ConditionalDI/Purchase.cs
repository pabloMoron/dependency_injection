using ConditionalDI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConditionalDI
{
    internal class Purchase
    {
        private readonly Func<Locations, ITaxCalculator> _accessor;
        public Purchase(Func<Locations, ITaxCalculator> accessor) {
            this._accessor = accessor;
        }

        public int CheckOut(Locations location) {
            var tax = _accessor(location).CalculateTax();
            return tax + 100;
        }
    }
}
