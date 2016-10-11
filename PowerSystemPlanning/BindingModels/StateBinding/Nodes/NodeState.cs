using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using PowerSystemPlanning.BindingModels.StateBinding.Generator;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Nodes;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Models.SystemState.Branch;
using PowerSystemPlanning.Models.SystemState.Generator;
using PowerSystemPlanning.Models.SystemState.Load;
using PowerSystemPlanning.Models.SystemState.Nodes;
using Prism.Mvvm;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Nodes;

namespace PowerSystemPlanning.BindingModels.StateBinding.Nodes
{
    [DataContract()]
    public class NodeState : PowerSystemElementState, INodeState
    {
        [DataMember()]
        public Node BindingUnderlyingNode { get; protected set; }
        public INode UnderlyingNode => BindingUnderlyingNode;
        public override IPowerSystemElement MyPowerSystemElement => BindingUnderlyingNode;

        public IList<IInelasticLoadState> InelasticLoadsStates =>
            (from load in MyPowerSystemState.InelasticLoadStates
            where load.UnderlyingInelasticLoad.ConnectionNode == UnderlyingNode
            select load).ToList();

        public double TotalInelasticLoad =>
            (from load in MyPowerSystemState.InelasticLoadStates
             where load.UnderlyingInelasticLoad.ConnectionNode == UnderlyingNode
             select load.Consumption)
            .Sum();

        public IList<IGeneratingUnitState> GeneratingUnitsStates
            =>
            (from gen in MyPowerSystemState.GeneratingUnitStates
            where gen.UnderlyingGeneratingUnit.ConnectionNode == UnderlyingNode
            select gen).ToList();

        public IList<IGeneratingUnitState> ActiveGeneratingUnitStates =>
            (from gen in GeneratingUnitsStates
            where gen.IsAvailable
            select gen).ToList();

        public IList<ISimpleTransmissionLineState> TransmissionLinesStates =>
            IncomingTransmissionLinesStates.Concat(OutgoingTransmissionLinesStates).ToList();

        public IList<ISimpleTransmissionLineState> ActiveTransmissionLinesStates =>
            ActiveIncomingTransmissionLinesStates.Concat(ActiveOutgoingTransmissionLinesStates).ToList();

        public IList<ISimpleTransmissionLineState> IncomingTransmissionLinesStates =>
            (from tl in MyPowerSystemState.SimpleTransmissionLineStates
            where tl.UnderlyingTransmissionLine.NodeTo == UnderlyingNode
            select tl).ToList();

        public IList<ISimpleTransmissionLineState> ActiveIncomingTransmissionLinesStates =>
            (from tl in IncomingTransmissionLinesStates
            where tl.IsAvailable
            select tl).ToList();

        public IList<ISimpleTransmissionLineState> OutgoingTransmissionLinesStates =>
            (from tl in MyPowerSystemState.SimpleTransmissionLineStates
            where tl.UnderlyingTransmissionLine.NodeFrom == UnderlyingNode
            select tl).ToList();

        public IList<ISimpleTransmissionLineState> ActiveOutgoingTransmissionLinesStates =>
            (from tl in OutgoingTransmissionLinesStates
            where tl.IsAvailable
            select tl).ToList();

        public NodeState(PowerSystemState state, Node wrappedInelasticLoad)
            : base(state)
        {
            BindingUnderlyingNode = wrappedInelasticLoad;
        }
    }
}
