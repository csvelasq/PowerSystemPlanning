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
        private double _ConsumptionMW;

        /// <summary>
        /// Consumption of the load (in MW).
        /// </summary>
        public double ConsumptionMW
        {
            get
            {
                return _ConsumptionMW;
            }

            set
            {
                _ConsumptionMW = value;
            }
        }
        
        /// <summary>
        /// Empty constructor, not meant to be used but rather included only to allow serialization.
        /// </summary>
        public InelasticLoad() : base() { }

        public InelasticLoad(PowerSystem power_system) : base(power_system)
        {
        }
    }
}
