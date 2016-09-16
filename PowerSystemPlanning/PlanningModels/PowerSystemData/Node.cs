using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning
{
    /// <summary>
    /// A node in a given power system.
    /// A node is an element to which generators and loads are connected.
    /// Transmission lines are connected between two nodes.
    /// </summary>
    public class Node
    {
        [System.Xml.Serialization.XmlIgnoreAttribute]
        protected PowerSystem _PowerSystem;

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public PowerSystem PowerSystem
        {
            get
            {
                return this._PowerSystem;
            }
            set
            {
                this._PowerSystem = value;
            }
        }

        protected int id;
        /// <summary>
        /// The unique ID of this node within the power system.
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        protected string name;
        /// <summary>
        /// Name of the node (arbitrarily assigned by the user).
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Generating units connected to this node.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public List<GeneratingUnit> GeneratingUnits
        {
            get
            {
                return (from gen in this._PowerSystem._GeneratingUnits where gen.ConnectionNodeId == this.Id select gen).ToList<GeneratingUnit>();
            }
        }

        /// <summary>
        /// Inelastic loads connected to this node.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public List<InelasticLoad> InelasticLoads
        {
            get
            {
                return (from load in this._PowerSystem._InelasticLoads where load.ConnectionNodeId == this.Id select load).ToList<InelasticLoad>();
            }
        }

        /// <summary>
        /// Total consumption (in MW) of all inelastic loads connected to this node.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public double TotalLoad
        {
            get
            {
                return (from load in this._PowerSystem._InelasticLoads where load.ConnectionNodeId == this.Id select load.ConsumptionMW).Sum();
            }
        }

        /// <summary>
        /// Transmission lines connected to this node (incoming and outgoing).
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public List<TransmissionLine> TransmissionLines
        {
            get
            {
                var tls = this.IncomingTransmissionLines;
                tls.AddRange(this.OutgoingTransmissionLines);
                return tls;
            }
        }

        /// <summary>
        /// Transmission lines incoming to this node.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public List<TransmissionLine> IncomingTransmissionLines
        {
            get
            {
                return (from tl in this._PowerSystem._TransmissionLines where tl.NodeToID == this.Id select tl).ToList<TransmissionLine>();
            }
        }

        /// <summary>
        /// Transmission lines outgoing from this node.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public List<TransmissionLine> OutgoingTransmissionLines
        {
            get
            {
                return (from tl in this._PowerSystem._TransmissionLines where tl.NodeFromID == this.Id select tl).ToList<TransmissionLine>();
            }
        }

        /// <summary>
        /// Empty constructor, not meant to be used but rather included only to allow serialization.
        /// </summary>
        public Node() { }

        /// <summary>
        /// Creates a new node in the given power system
        /// </summary>
        /// <param name="power_system">The power system to which this node is being added</param>
        /// <remarks>The ID of the new node is set to he number of nodes in the given power system. However, the nely created node is not added to the list of nodes in the provided power system.</remarks>
        public Node(PowerSystem power_system)
        {
            this._PowerSystem = power_system;
            this.id = this._PowerSystem.Nodes.Count;
        }

        public Node(PowerSystem power_system, Node node) : this(power_system)
        {
            this.Name = node.Name;
        }
    }
}
