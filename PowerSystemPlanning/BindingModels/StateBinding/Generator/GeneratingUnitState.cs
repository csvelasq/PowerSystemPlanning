using Prism.Mvvm;
using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData.Generator;
using PowerSystemPlanning.Models.SystemState.Generator;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Generator;

namespace PowerSystemPlanning.BindingModels.StateBinding.Generator
{
    /// <summary>
    /// Describes a particular state of some generating unit (in order to alter its marginal cost or available capacity).
    /// </summary>
    [DataContract()]
    public class GeneratingUnitState : PowerSystemElementState, IGeneratingUnitState
    {
        [DataMember()]
        public GeneratingUnit BindingUnderlyingGeneratingUnit { get; protected set; }

        public IGeneratingUnit UnderlyingGeneratingUnit => BindingUnderlyingGeneratingUnit;

        public override IPowerSystemElement MyPowerSystemElement => BindingUnderlyingGeneratingUnit;

        protected double _AvailableCapacity;
        [DataMember()]
        public double AvailableCapacity
        {
            get { return _AvailableCapacity; }
            set { SetProperty<double>(ref _AvailableCapacity, value); }
        }

        protected double _MarginalCost;
        [DataMember()]
        public double MarginalCost
        {
            get { return _MarginalCost; }
            set { SetProperty<double>(ref _MarginalCost, value); }
        }

        protected bool _IsAvailable = true;
        [DataMember()]
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
        public GeneratingUnitState(PowerSystemState state, GeneratingUnit gen)
            : this(state, gen, gen.InstalledCapacity, gen.MarginalCost)
        { }

        public GeneratingUnitState(PowerSystemState state, GeneratingUnit gen, double availableCapacity,
            double marginalCost)
            : base(state)
        {
            BindingUnderlyingGeneratingUnit = gen;
            AvailableCapacity = availableCapacity;
            MarginalCost = marginalCost;
        }
    }
}
