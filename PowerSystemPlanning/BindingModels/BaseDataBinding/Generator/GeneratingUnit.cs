using System.Runtime.Serialization;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Nodes;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Generator;

namespace PowerSystemPlanning.BindingModels.BaseDataBinding.Generator
{
    /// <summary>
    /// Represents a single generating unit in a given power system.
    /// </summary>
    [DataContract()]
    public class GeneratingUnit : NodeElement, IGeneratingUnit
    {
        protected double _InstalledCapacity;

        /// <summary>
        /// Installed capacity of the generating unit, in MW.
        /// Represents the maximum possible output of the unit.
        /// </summary>
        [DataMember()]
        public double InstalledCapacity
        {
            get
            {
                return _InstalledCapacity;
            }

            set
            {
                SetProperty<double>(ref _InstalledCapacity, value);
            }
        }

        protected double _MarginalCost;

        /// <summary>
        /// Marginal cost, in $ per MWh, of this generating unit.
        /// </summary>
        [DataMember()]
        public double MarginalCost
        {
            get
            {
                return _MarginalCost;
            }

            set
            {
                SetProperty<double>(ref _MarginalCost, value);
            }
        }

        /// <summary>
        /// Creates a new generating unit within the provided power system with an automatically assigned ID.
        /// </summary>
        /// <param name="pws">The power system to which this generator belongs.</param>
        public GeneratingUnit(IPowerSystem pws)
            : base(pws, pws.GeneratingUnits.Count)
        { }
    }
}
