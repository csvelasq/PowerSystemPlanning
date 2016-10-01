using Prism.Mvvm;
using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData.Generator;
using PowerSystemPlanning.Models.SystemState.Generator;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemState;

namespace PowerSystemPlanning.BindingModels.StateBinding.Generator
{
    /// <summary>
    /// Describes a particular state of some generating unit (in order to alter its marginal cost or available capacity).
    /// </summary>
    [DataContract()]
    public class GeneratingUnitState : PowerSystemElementState, IGeneratingUnitState
    {
        [DataMember()]
        public IGeneratingUnit UnderlyingGeneratingUnit { get; protected set; }

        public override IPowerSystemElement MyPowerSystemElement => UnderlyingGeneratingUnit;

        [DataMember()]
        protected double _AvailableCapacity;
        public double AvailableCapacity
        {
            get { return _AvailableCapacity; }
            set { SetProperty<double>(ref _AvailableCapacity, value); }
        }

        [DataMember()]
        protected double _MarginalCost;
        public double MarginalCost
        {
            get { return _MarginalCost; }
            set { SetProperty<double>(ref _MarginalCost, value); }
        }

        [DataMember()]
        protected bool _IsAvailable = true;
        public bool IsAvailable
        {
            get { return _IsAvailable; }
            set { SetProperty<bool>(ref _IsAvailable, value); }
        }

        /// <summary>
        /// Creates a new state for a given generating unit, with default parameters.
        /// </summary>
        /// <param name="state">The power system state to which this object belongs.</param>
        /// <param name="gen">The generating unit whose state this object describes. <see cref="AvailableCapacity"/> is set equal to <see cref="IGeneratingUnit.InstalledCapacity"/>, while <see cref="MarginalCost"/> is set to <see cref="IGeneratingUnit.MarginalCost"/>.</param>
        public GeneratingUnitState(IPowerSystemState state, IGeneratingUnit gen)
            : this(state, gen, gen.InstalledCapacity, gen.MarginalCost)
        { }

        public GeneratingUnitState(IPowerSystemState state, IGeneratingUnit gen, double availableCapacity,
            double marginalCost)
            : base(state)
        {
            UnderlyingGeneratingUnit = gen;
            AvailableCapacity = availableCapacity;
            MarginalCost = marginalCost;
        }
    }
}
