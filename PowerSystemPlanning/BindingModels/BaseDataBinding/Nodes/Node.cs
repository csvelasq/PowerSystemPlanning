using PowerSystemPlanning.Models.SystemBaseData.Generator;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Branch;
using PowerSystemPlanning.Models.SystemBaseData.Load;
using PowerSystemPlanning.Models.SystemBaseData.Nodes;

namespace PowerSystemPlanning.BindingModels.BaseDataBinding.Nodes
{
    /// <summary>
    /// A node in a given power system.
    /// A node is an element to which generators and loads are connected.
    /// Transmission lines are connected between two nodes.
    /// </summary>
    [DataContract()]
    public class Node : PowerSystemElement, INode
    {
        /// <summary>
        /// Generating units connected to this node.
        /// </summary>
        public IEnumerable<IGeneratingUnit> GeneratingUnits
        {
            get
            {
                return (from gen in this.MyPowerSystem.GeneratingUnits
                        where gen.ConnectionNode.Id == this.Id
                        select gen);
            }
        }

        /// <summary>
        /// Inelastic loads connected to this node.
        /// </summary>
        public IEnumerable<IInelasticLoad> InelasticLoads
        {
            get
            {
                return (from load in this.MyPowerSystem.InelasticLoads
                        where load.ConnectionNode.Id == this.Id
                        select load);
            }
        }

        /// <summary>
        /// Total consumption (in MW) of all inelastic loads connected to this node.
        /// </summary>
        public double TotalInelasticLoad
        {
            get
            {
                return (from load in this.MyPowerSystem.InelasticLoads
                        where load.ConnectionNode.Id == this.Id
                        select load.Consumption).Sum();
            }
        }

        /// <summary>
        /// Transmission lines connected to this node (incoming and outgoing).
        /// </summary>
        public IEnumerable<ISimpleTransmissionLine> TransmissionLines
        {
            get
            {
                var tls = this.IncomingTransmissionLines;
                tls.Concat(this.OutgoingTransmissionLines);
                return tls;
            }
        }

        /// <summary>
        /// Transmission lines incoming to this node.
        /// </summary>
        public IEnumerable<ISimpleTransmissionLine> IncomingTransmissionLines
        {
            get
            {
                return (from tl in this.MyPowerSystem.TransmissionLines
                        where tl.NodeTo.Id == this.Id
                        select tl);
            }
        }

        /// <summary>
        /// Transmission lines outgoing from this node.
        /// </summary>
        public IEnumerable<ISimpleTransmissionLine> OutgoingTransmissionLines
        {
            get
            {
                return (from tl in this.MyPowerSystem.TransmissionLines
                        where tl.NodeFrom.Id == this.Id
                        select tl);
            }
        }

        /// <summary>
        /// Creates a new node in the given power system
        /// </summary>
        /// <param name="pws">The power system to which this node is being added</param>
        /// <remarks>The ID of the new node is set to the number of nodes in the given power system. However, the nely created node is not added to the list of nodes in the provided power system.</remarks>
        public Node(PowerSystem pws)
            : base(pws, pws.Nodes.Count)
        { }
    }
}
