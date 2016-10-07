using System.Collections.Generic;
using PowerSystemPlanning.Models.SystemBaseData;

namespace PowerSystemPlanning.Models.SystemState
{
    /// <summary>
    /// A collection of power system states (e.g. a particular scenario).
    /// </summary>
    /// <remarks>
    /// This interface is provided in order to allow a variety of state / scenario generation methods. For example, a simple LDC implementation might simply bind <see cref="MyPowerSystemStates"/> to a static List of IPowerSystemState objects, while a monte-carlo simulation might dynamically generate the states in <see cref="MyPowerSystemStates"/>.
    /// </remarks>
    public interface IPowerSystemStateCollection
    {
        /// <summary>
        /// The power system whose states this collection describes.
        /// </summary>
        IPowerSystem MyPowerSystem { get; }
        /// <summary>
        /// All the power system's states contained in this collection.
        /// </summary>
        IEnumerable<IPowerSystemState> MyPowerSystemStates { get; }
    }
}
