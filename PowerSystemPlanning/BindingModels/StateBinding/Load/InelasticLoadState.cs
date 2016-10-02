using Prism.Mvvm;
using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Load;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Models.SystemState.Load;
using System;

namespace PowerSystemPlanning.BindingModels.StateBinding.Load
{
    /// <summary>
    /// Describes a particular state of a given inelastic load.
    /// </summary>
    [DataContract()]
    public class InelasticLoadState : PowerSystemElementState, IInelasticLoadState
    {
        [DataMember()]
        public IInelasticLoad UnderlyingInelasticLoad { get; protected set; }

        public override IPowerSystemElement MyPowerSystemElement => UnderlyingInelasticLoad;

        [DataMember()]
        protected double _Consumption;
        /// <summary>
        /// The consumption (MW) of this load in this particular state of the power system (described by <see cref="PowerSystemElementState.MyPowerSystemState"/>).
        /// </summary>
        public double Consumption
        {
            get { return _Consumption; }
            set { SetProperty<double>(ref _Consumption, value); }
        }

        [DataMember()]
        protected double _LoadSheddingCost;
        public double LoadSheddingCost
        {
            get {return _LoadSheddingCost; }
            set { SetProperty<double>(ref _LoadSheddingCost, value); }
        }

        public InelasticLoadState(IPowerSystemState state, IInelasticLoad wrappedInelasticLoad)
            : base(state)
        {
            UnderlyingInelasticLoad = wrappedInelasticLoad;
            Consumption = wrappedInelasticLoad.Consumption;
        }
    }
}
