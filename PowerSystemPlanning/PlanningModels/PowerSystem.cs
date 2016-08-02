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
    /// Represents the technical and economic model of a Power System for medium to long term planning.
    /// </summary>
    /// <remarks>
    /// Basic object model of a power system for the purpose of solving planning models in the medium to long term, with DC power flow.
    /// The purpose of this class is to provide for easy end-user manipulation and edition of the power system model.
    /// The decorator <see cref="PowerSystemDecorator"/> should be used instead of this class in order to build optimization models, so as to enable dynamic flexibility of the resulting optimization models (e.g. allowing to deactivate transmission lines, changing the maximum output of a generator, and so on).
    /// </remarks>
    public class PowerSystem : IPowerSystem
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
        /// Nodes of the power system, bindable to GUI.
        /// </summary>
        public BindingList<Node> _Nodes;

        /// <summary>
        /// Nodes of the power system.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public IList<Node> Nodes { get { return this._Nodes; } }

        /// <summary>
        /// Number of nodes in the power system.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public int NumberOfNodes
        {
            get
            {
                return _Nodes.Count;
            }
        }

        /// <summary>
        /// All Generating units in the power system, bindable to GUI.
        /// </summary>
        public BindingList<GeneratingUnit> _GeneratingUnits;

        /// <summary>
        /// All Generating units in the power system
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public IList<GeneratingUnit> GeneratingUnits { get { return this._GeneratingUnits; } }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public int NumberOfGeneratingUnits
        {
            get
            {
                return this._GeneratingUnits.Count;
            }
        }

        /// <summary>
        /// All Inelastic loads in the power system, bindable to GUI.
        /// </summary>
        public BindingList<InelasticLoad> _InelasticLoads;

        /// <summary>
        /// All inelastic loads in the power system.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public IList<InelasticLoad> InelasticLoads { get { return this._InelasticLoads; } }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public int NumberOfInelasticLoads
        {
            get
            {
                return this._InelasticLoads.Count;
            }
        }

        /// <summary>
        /// Total MW of inelastic loads.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public double TotalMWInelasticLoads
        {
            get
            {
                return (from load in this._InelasticLoads select load.ConsumptionMW).Sum();
            }
        }

        private double _LoadSheddingCost;

        /// <summary>
        /// The cost (in US$) of shedding 1 MW of any load.
        /// </summary>
        public double LoadSheddingCost
        {
            get
            {
                return this._LoadSheddingCost;
            }
            set
            {
                this._LoadSheddingCost = value;
            }
        }

        /// <summary>
        /// All Transmission lines in the power sytem, bindable to GUI.
        /// </summary>
        public BindingList<TransmissionLine> _TransmissionLines;

        /// <summary>
        /// All Transmission lines in the power sytem.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public IList<TransmissionLine> TransmissionLines { get { return this._TransmissionLines; } }

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public int NumberOfTransmissionLines
        {
            get
            {
                return this._TransmissionLines.Count;
            }
        }

        public PowerSystem()
        {
            this._Nodes = new BindingList<Node>();
            this._GeneratingUnits = new BindingList<GeneratingUnit>();
            this._InelasticLoads = new BindingList<InelasticLoad>();
            this._TransmissionLines = new BindingList<TransmissionLine>();
        }

        public PowerSystem(string name) : this()
        {
            this.Name = name;
        }

        public PowerSystem(string name, BindingList<GeneratingUnit> generatingUnits, BindingList<InelasticLoad> inelasticLoads,
            BindingList<TransmissionLine> transmissionLines)
            : this(name)
        {
            this._GeneratingUnits = generatingUnits;
            this._InelasticLoads = inelasticLoads;
            this._TransmissionLines = transmissionLines;
        }

        /// <summary>
        /// Saves the current power sysem to an XML file, provided the stream.
        /// </summary>
        /// <param name="saveStream"></param>
        /// <remarks>
        /// The provided stream must is not closed within this method.
        /// IOException and other exceptions are not managed.</remarks>
        public void saveToXMLFile(TextWriter saveStream)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(PowerSystem));
            writer.Serialize(saveStream, this);
        }

        /// <summary>
        /// Creates a Power System by reading an XML file, provided the stream.
        /// </summary>
        /// <param name="xmlStream">Full file path of the XML file where the power system was stored.</param>
        /// <returns>A new power system object with the contents serialized in the XML file.</returns>
        /// <remarks>
        /// The provided stream must is not closed within this method.
        /// IOException and other exceptions are not managed.</remarks>
        public static PowerSystem readFromXMLFile(StreamReader xmlStream)
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PowerSystem));
            PowerSystem retval = (PowerSystem)reader.Deserialize(xmlStream);
            foreach (Node node in retval.Nodes)
            {
                node.PowerSystem = retval;
            }
            return retval;
        }
    }
}
