using System;
using System.Runtime.Serialization;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Branch;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Branch;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Models.SystemState.Branch;
using Prism.Mvvm;

namespace PowerSystemPlanning.BindingModels.StateBinding.Branch
{
    [DataContract()]
    public class SimpleTransmissionLineState : PowerSystemElementState, ISimpleTransmissionLineState
    {
        [DataMember()]
        public ISimpleTransmissionLine UnderlyingTransmissionLine { get; protected set; }

        public override IPowerSystemElement MyPowerSystemElement => UnderlyingTransmissionLine;

        [DataMember()]
        protected bool _IsAvailable = true;
        public bool IsAvailable
        {
            get { return _IsAvailable; }
            set { SetProperty<bool>(ref _IsAvailable, value); }
        }

        public SimpleTransmissionLineState(IPowerSystemState state, ISimpleTransmissionLine tl)
            :base(state)
        {
            UnderlyingTransmissionLine = tl;
        }
    }
}
