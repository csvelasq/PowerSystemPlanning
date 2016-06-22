using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels
{
    /// <summary>
    /// Represents a candidate generating unit that may be built in the future.
    /// </summary>
    public class GeneratingUnitCandidate : GeneratingUnit
    {
        private double investmentCost;

        public GeneratingUnitCandidate(PowerSystem power_system) : base(power_system)
        {
        }

        /// <summary>
        /// Total investment cost in this new transmission line (in US$).
        /// </summary>
        public double InvestmentCost
        {
            get
            {
                return investmentCost;
            }

            set
            {
                investmentCost = value;
            }
        }
    }
}
