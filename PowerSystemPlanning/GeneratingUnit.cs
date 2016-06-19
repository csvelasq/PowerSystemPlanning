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
        private double installedCapacityMW;

        /// <summary>
        /// Installed capacity of the generating unit, in MW.
        /// Represents the maximum possible output of the unit.
        /// </summary>
        public double InstalledCapacityMW
        {
            get
            {
                return installedCapacityMW;
            }

            set
            {
                installedCapacityMW = value;
            }
        }

        private double marginalCost;

        /// <summary>
        /// Marginal cost, in $ per MWh, of this generating unit.
        /// </summary>
        public double MarginalCost
        {
            get
            {
                return marginalCost;
            }

            set
            {
                marginalCost = value;
            }
        }

        public GeneratingUnit()
        {

        }
    }
}
