using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning
{
    /// <summary>
    /// Represents a single generating unit in a given power system.
    /// </summary>
    public class GeneratingUnit : NodeElement
    {
        private double _InstalledCapacityMW;

        /// <summary>
        /// Installed capacity of the generating unit, in MW.
        /// Represents the maximum possible output of the unit.
        /// </summary>
        public double InstalledCapacityMW
        {
            get
            {
                return _InstalledCapacityMW;
            }

            set
            {
                _InstalledCapacityMW = value;
            }
        }

        private double _MarginalCost;

        /// <summary>
        /// Marginal cost, in $ per MWh, of this generating unit.
        /// </summary>
        public double MarginalCost
        {
            get
            {
                return _MarginalCost;
            }

            set
            {
                _MarginalCost = value;
            }
        }

        /// <summary>
        /// Empty constructor, not meant to be used but rather included only to allow serialization.
        /// </summary>
        public GeneratingUnit():base() { }

        public GeneratingUnit(PowerSystem power_system) : base(power_system)
        {
            this.Id = this._PowerSystem.NumberOfGeneratingUnits;
        }
    }
}
