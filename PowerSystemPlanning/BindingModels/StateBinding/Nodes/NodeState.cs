using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PowerSystemPlanning.BindingModels.StateBinding.Generator;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Nodes;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Models.SystemState.Branch;
using PowerSystemPlanning.Models.SystemState.Generator;
using PowerSystemPlanning.Models.SystemState.Load;
using PowerSystemPlanning.Models.SystemState.Nodes;
using Prism.Mvvm;

namespace PowerSystemPlanning.BindingModels.StateBinding.Nodes
{
    public class NodeState : PowerSystemElementState, INodeState
    {
        public INode UnderlyingNode { get; protected set; }

        public override IPowerSystemElement MyPowerSystemElement => UnderlyingNode;

        public IEnumerable<IGeneratingUnitState> GeneratingUnitsStates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IGeneratingUnitState> ActiveGeneratingUnitStates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<IInelasticLoadState> InelasticLoadsStates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public double TotalInelasticLoad
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ISimpleTransmissionLineState> TransmissionLinesStates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ISimpleTransmissionLineState> ActiveTransmissionLinesStates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ISimpleTransmissionLineState> IncomingTransmissionLinesStates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ISimpleTransmissionLineState> ActiveIncomingTransmissionLinesStates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ISimpleTransmissionLineState> OutgoingTransmissionLinesStates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<ISimpleTransmissionLineState> ActiveOutgoingTransmissionLinesStates
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public NodeState(IPowerSystemState state, INode wrappedInelasticLoad)
            : base(state)
        {
            UnderlyingNode = wrappedInelasticLoad;
        }
    }
}
