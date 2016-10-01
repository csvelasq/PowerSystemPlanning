using System.Collections.Generic;
using PowerSystemPlanning.Models.SystemBaseData.Branch;
using PowerSystemPlanning.Models.SystemBaseData.Generator;
using PowerSystemPlanning.Models.SystemBaseData.Load;

namespace PowerSystemPlanning.Models.SystemBaseData.Nodes
{
    /// <summary>
    /// A node (bus) within a power system.
    /// </summary>
    public interface INode : IPowerSystemElement
    {
        /// <summary>
        /// Generating units connected to this node.
        /// </summary>
        IEnumerable<IGeneratingUnit> GeneratingUnits { get; }

        /// <summary>
        /// Inelastic loads connected to this node.
        /// </summary>
        IEnumerable<IInelasticLoad> InelasticLoads { get; }

        /// <summary>
        /// Total consumption (MW) of all inelastic loads connected to this node.
        /// </summary>
        double TotalInelasticLoad { get; }

        /// <summary>
        /// Transmission lines connected to this node (incoming and outgoing).
        /// </summary>
        IEnumerable<ISimpleTransmissionLine> TransmissionLines { get; }

        /// <summary>
        /// Transmission lines incoming to this node.
        /// </summary>
        IEnumerable<ISimpleTransmissionLine> IncomingTransmissionLines { get; }

        /// <summary>
        /// Transmission lines outgoing from this node.
        /// </summary>
        IEnumerable<ISimpleTransmissionLine> OutgoingTransmissionLines { get; }
    }
}