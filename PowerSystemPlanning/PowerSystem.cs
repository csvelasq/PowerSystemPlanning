using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PowerSystemPlanning
{
    /// <summary>
    /// Represents the technical and economic model of a Power System for the objectives of medium to long term planning.
    /// </summary>
    public class PowerSystem
    {
        // TODO Linear DC OPF
        // TODO Linear DC OPF LDC
        // TODO Linear DC OPF LDC with generation and transmission binary parameters


        private string name;

        /// <summary>
        /// Name of the power system.
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
        /// Nodes of the power system.
        /// </summary>
        public BindingList<Node> nodes;

        /// <summary>
        /// Number of nodes in the power system.
        /// </summary>
        public int NumberOfNodes
        {
            get
            {
                return nodes.Count;
            }
        }

        /// <summary>
        /// All Generating units in the power system.
        /// </summary>
        public BindingList<GeneratingUnit> generatingUnits;

        public int NumberOfGeneratingUnits
        {
            get
            {
                return this.generatingUnits.Count;
            }
        }

        /// <summary>
        /// All Inelastic loads in the power system.
        /// </summary>
        public BindingList<InelasticLoad> inelasticLoads;

        public int NumberOfInelasticLoads
        {
            get
            {
                return this.inelasticLoads.Count;
            }
        }

        /// <summary>
        /// Total MW of inelastic loads.
        /// </summary>
        public double TotalMWInelasticLoads
        {
            get
            {
                return (from load in this.inelasticLoads select load.ConsumptionMW).Sum();
            }
        }

        /// <summary>
        /// All Transmission lines in the power sytem.
        /// </summary>
        public BindingList<TransmissionLine> transmissionLines;

        public int NumberOfTransmissionLines
        {
            get
            {
                return this.transmissionLines.Count;
            }
        }

        public PowerSystem()
        {
            this.nodes = new BindingList<Node>();
            this.generatingUnits = new BindingList<GeneratingUnit>();
            this.inelasticLoads = new BindingList<InelasticLoad>();
            this.transmissionLines = new BindingList<TransmissionLine>();
        }

        public PowerSystem(string name) : this()
        {
            this.Name = name;
        }

        public PowerSystem(string name, BindingList<GeneratingUnit> generatingUnits, BindingList<InelasticLoad> inelasticLoads,
            BindingList<TransmissionLine> transmissionLines)
            : this(name)
        {
            this.generatingUnits = generatingUnits;
            this.inelasticLoads = inelasticLoads;
            this.transmissionLines = transmissionLines;
        }
        
        /// <summary>
        /// Saves the current power sysem to an XML file, provided the stream.
        /// The provided stream must is not closed within this method.
        /// IOException and other exceptions are not managed.
        /// </summary>
        /// <param name="saveStream"></param>
        public void saveToXMLFile(TextWriter saveStream)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(PowerSystem));
            writer.Serialize(saveStream, this);
        }

        /// <summary>
        /// Creates a Power System by reading an XML file, provided the stream.
        /// The provided stream must is not closed within this method.
        /// IOException and other exceptions are not managed.
        /// </summary>
        /// <param name="xmlStream">Full file path of the XML file where the power system was stored.</param>
        /// <returns>A new power system object with the contents serialized in the XML file.</returns>
        public static PowerSystem readFromXMLFile(StreamReader xmlStream)
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PowerSystem));
            PowerSystem retval = (PowerSystem)reader.Deserialize(xmlStream);
            return retval;
        }
    }
}
