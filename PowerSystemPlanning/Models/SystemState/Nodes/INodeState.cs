using System.Collections.Generic;
using PowerSystemPlanning.Models.SystemBaseData.Nodes;
using PowerSystemPlanning.Models.SystemState.Branch;
using PowerSystemPlanning.Models.SystemState.Generator;
using PowerSystemPlanning.Models.SystemState.Load;

namespace PowerSystemPlanning.Models.SystemState.Nodes
{
    /// <summary>
    /// A particular state of a node (bus) within a power system.
    /// </summary>
    public interface INodeState : IPowerSystemElementState
    {
        INode UnderlyingNode { get; }

        /// <summary>
        /// State of all generating units connected to this node.
        /// </summary>
        IList<IGeneratingUnitState> GeneratingUnitsStates { get; }

        /// <summary>
        /// State of all active (available) generating units connected to this node.
        /// </summary>
        IList<IGeneratingUnitState> ActiveGeneratingUnitStates { get; }

        /// <summary>
        /// State of all inelastic loads connected to this node.
        /// </summary>
        IList<IInelasticLoadState> InelasticLoadsStates { get; }

        /// <summary>
        /// Total consumption (MW) of all inelastic loads connected to this node.
        /// </summary>
        double TotalInelasticLoad { get; }

        /// <summary>
        /// State of all transmission lines connected to this node (incoming and outgoing).
        /// </summary>
        IList<ISimpleTransmissionLineState> TransmissionLinesStates { get; }

        /// <summary>
        /// State of active (available) transmission lines connected to this node (incoming and outgoing).
        /// </summary>
        IList<ISimpleTransmissionLineState> ActiveTransmissionLinesStates { get; }

        /// <summary>
        /// State of all transmission lines incoming to this node.
        /// </summary>
        IList<ISimpleTransmissionLineState> IncomingTransmissionLinesStates { get; }

        /// <summary>
        /// State of active (available) transmission lines incoming to this node.
        /// </summary>
        IList<ISimpleTransmissionLineState> ActiveIncomingTransmissionLinesStates { get; }

        /// <summary>
        /// State of all transmission lines outgoing from this node.
        /// </summary>
        IList<ISimpleTransmissionLineState> OutgoingTransmissionLinesStates { get; }

        /// <summary>
        /// State of active (available) transmission lines outgoing from this node.
        /// </summary>
        IList<ISimpleTransmissionLineState> ActiveOutgoingTransmissionLinesStates { get; }
    }
}
