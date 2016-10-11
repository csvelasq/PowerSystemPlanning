using PowerSystemPlanning.Models.SystemState.Generator;
using System.Collections.Generic;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemState.Branch;
using PowerSystemPlanning.Models.SystemState.Load;
using PowerSystemPlanning.Models.SystemState.Nodes;

namespace PowerSystemPlanning.Models.SystemState
{
    /// <summary>
    /// Encapsulates one particular state of a power system.
    /// </summary>
    public interface IPowerSystemState : IHavePowerSystem
    {
        /// <summary>
        /// The duration (hours) this state is valid.
        /// </summary>
        double Duration { get; set; }
        string Name { get; set; }
        IList<INodeState> NodeStates { get; }
        /// <summary>
        /// The state of each generating unit in this power system.
        /// </summary>
        IList<IGeneratingUnitState> GeneratingUnitStates { get; }
        IList<IGeneratingUnitState> ActiveGeneratingUnitStates { get; }
        /// <summary>
        /// The current state of each inelastic load in this power system.
        /// </summary>
        IList<IInelasticLoadState> InelasticLoadStates { get; }

        IList<ISimpleTransmissionLineState> SimpleTransmissionLineStates { get; }
        IList<ISimpleTransmissionLineState> ActiveSimpleTransmissionLineStates { get; }
    }
}
