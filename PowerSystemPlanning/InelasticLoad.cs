using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning
{
    /// <summary>
    /// An inelastic load connected to a given power system (i.e. with fixed consumption regardless of the price of electricity).
    /// </summary>
    public class InelasticLoad : NodeElement
    {
        private double consumptionMW;

        /// <summary>
        /// Consumption of the load (in MW).
        /// </summary>
        public double ConsumptionMW
        {
            get
            {
                return consumptionMW;
            }

            set
            {
                consumptionMW = value;
            }
        }

        public InelasticLoad()
        {

        }
    }
}
