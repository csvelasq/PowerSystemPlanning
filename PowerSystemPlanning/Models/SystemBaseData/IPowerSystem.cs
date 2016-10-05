using System.Collections.Generic;
using PowerSystemPlanning.Models.SystemBaseData.Branch;
using PowerSystemPlanning.Models.SystemBaseData.Generator;
using PowerSystemPlanning.Models.SystemBaseData.Load;
using PowerSystemPlanning.Models.SystemBaseData.Nodes;

namespace PowerSystemPlanning.Models.SystemBaseData
{
    /// <summary>
    /// Technical data defining a particular power system.
    /// </summary>
    public interface IPowerSystem
    {
        /// <summary>
        /// The name of this power system.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The (default) load shedding cost (US$/MWh).
        /// </summary>
        double LoadSheddingCost { get; }
        /// <summary>
        /// Set of nodes (buses) in this power system.
        /// </summary>
        IList<INode> Nodes { get; }
        /// <summary>
        /// Generating units in this power system.
        /// </summary>
        IList<IGeneratingUnit> GeneratingUnits { get; }
        /// <summary>
        /// Inelastic loads in this power system.
        /// </summary>
        IList<IInelasticLoad> InelasticLoads { get; }
        /// <summary>
        /// Transmission lines in this power system.
        /// </summary>
        IList<ISimpleTransmissionLine> TransmissionLines { get; }
    }
}