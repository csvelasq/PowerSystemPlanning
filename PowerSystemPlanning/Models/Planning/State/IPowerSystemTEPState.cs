using System.Collections.Generic;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Models.SystemState.Branch;

namespace PowerSystemPlanning.Models.Planning.State
{
    /// <summary>
    /// A particular state of a power system in a transmission expansion planning problem.
    /// </summary>
    public interface IPowerSystemTepState : IPowerSystemState
    {
        /// <summary>
        /// The state of each candidate transmission line in a power system.
        /// </summary>
        IList<ICandidateTransmissionLineState> CandidateTransmissionLineStates { get; }
    }
}
