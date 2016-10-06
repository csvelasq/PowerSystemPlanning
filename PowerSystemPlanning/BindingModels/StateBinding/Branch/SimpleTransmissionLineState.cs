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
        public SimpleTransmissionLine BindingUnderlyingTransmissionLine { get; protected set; }

        public ISimpleTransmissionLine UnderlyingTransmissionLine => BindingUnderlyingTransmissionLine;

        public override IPowerSystemElement MyPowerSystemElement => UnderlyingTransmissionLine;

        [DataMember()]
        protected bool _IsAvailable = true;
        public bool IsAvailable
        {
            get { return _IsAvailable; }
            set
            {
                SetProperty<bool>(ref _IsAvailable, value);
                /*
                if (_IsAvailable != value)
                {
                    _IsAvailable = value;
                    OnPropertyChanged();
                    //If transmission line is disabled, no thermal capacity
                    if (!IsAvailable)
                        AvailableThermalCapacity = 0;
                }
                */
            }
        }

        [DataMember()]
        protected double _AvailableThermalCapacity;
        public double AvailableThermalCapacity
        {
            get { return _AvailableThermalCapacity; }
            set { SetProperty<double>(ref _AvailableThermalCapacity, value); }
        }

        public SimpleTransmissionLineState(PowerSystemState state, SimpleTransmissionLine tl)
            : base(state)
        {
            BindingUnderlyingTransmissionLine = tl;
            AvailableThermalCapacity = tl.ThermalCapacity;
        }
    }
}
