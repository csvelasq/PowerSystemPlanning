using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning
{
    /// <summary>
    /// An inelastic load connected to a given power system.
    /// </summary>
    /// <remarks>
    /// An inelastic load has fixed consumption (in MW) regardless of the price of electricity.
    /// A high cost is incurred if the load is to be shed.
    /// </remarks>
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
            this.Id = this._PowerSystem.InelasticLoads.Count;
        }
    }
}
