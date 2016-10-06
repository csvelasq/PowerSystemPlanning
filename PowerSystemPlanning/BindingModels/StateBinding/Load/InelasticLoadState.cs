using Prism.Mvvm;
using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Load;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Models.SystemState.Load;
using System;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Load;

namespace PowerSystemPlanning.BindingModels.StateBinding.Load
{
    /// <summary>
    /// Describes a particular state of a given inelastic load.
    /// </summary>
    [DataContract()]
    public class InelasticLoadState : PowerSystemElementState, IInelasticLoadState
    {
        [DataMember()]
        public InelasticLoad BindingUnderlyingInelasticLoad { get; protected set; }

        public IInelasticLoad UnderlyingInelasticLoad => BindingUnderlyingInelasticLoad;

        public override IPowerSystemElement MyPowerSystemElement => UnderlyingInelasticLoad;

        protected double _Consumption;
        /// <summary>
        /// The consumption (MW) of this load in this particular state of the power system (described by <see cref="PowerSystemElementState.MyPowerSystemState"/>).
        /// </summary>
        [DataMember()]
        public double Consumption
        {
            get { return _Consumption; }
            set { SetProperty<double>(ref _Consumption, value); }
        }

        protected double _LoadSheddingCost;
        [DataMember()]
        public double LoadSheddingCost
        {
            get {return _LoadSheddingCost; }
            set { SetProperty<double>(ref _LoadSheddingCost, value); }
        }

        public InelasticLoadState(PowerSystemState state, InelasticLoad wrappedInelasticLoad)
            : base(state)
        {
            _LoadSheddingCost = state.MyPowerSystem.LoadSheddingCost; //cost of LS defaults to the one defined in the power system
            BindingUnderlyingInelasticLoad = wrappedInelasticLoad;
            Consumption = wrappedInelasticLoad.Consumption;
        }
    }
}
