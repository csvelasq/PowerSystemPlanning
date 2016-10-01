using System.Collections.Generic;
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
        /// <summary>
        /// State of all generating units connected to this node.
        /// </summary>
        IEnumerable<IGeneratingUnitState> GeneratingUnitsStates { get; }

        /// <summary>
        /// State of all active (available) generating units connected to this node.
        /// </summary>
        IEnumerable<IGeneratingUnitState> ActiveGeneratingUnitStates { get; }

        /// <summary>
        /// State of all inelastic loads connected to this node.
        /// </summary>
        IEnumerable<IInelasticLoadState> InelasticLoadsStates { get; }

        /// <summary>
        /// Total consumption (MW) of all inelastic loads connected to this node.
        /// </summary>
        double TotalInelasticLoad { get; }

        /// <summary>
        /// State of all transmission lines connected to this node (incoming and outgoing).
        /// </summary>
        IEnumerable<ISimpleTransmissionLineState> TransmissionLinesStates { get; }

        /// <summary>
        /// State of active (available) transmission lines connected to this node (incoming and outgoing).
        /// </summary>
        IEnumerable<ISimpleTransmissionLineState> ActiveTransmissionLinesStates { get; }

        /// <summary>
        /// State of all transmission lines incoming to this node.
        /// </summary>
        IEnumerable<ISimpleTransmissionLineState> IncomingTransmissionLinesStates { get; }

        /// <summary>
        /// State of active (available) transmission lines incoming to this node.
        /// </summary>
        IEnumerable<ISimpleTransmissionLineState> ActiveIncomingTransmissionLinesStates { get; }

        /// <summary>
        /// State of all transmission lines outgoing from this node.
        /// </summary>
        IEnumerable<ISimpleTransmissionLineState> OutgoingTransmissionLinesStates { get; }

        /// <summary>
        /// State of active (available) transmission lines outgoing from this node.
        /// </summary>
        IEnumerable<ISimpleTransmissionLineState> ActiveOutgoingTransmissionLinesStates { get; }
    }
}
