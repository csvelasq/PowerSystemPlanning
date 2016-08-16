using NLog;
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
    public class PowerSystem : IPowerSystem, INotifyPropertyChanged
    {
        // TODO Linear DC OPF
        // TODO Linear DC OPF LDC
        // TODO Linear DC OPF LDC with generation and transmission binary parameters

        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        private string _Name;

        /// <summary>
        /// Name of the power system.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                if (this._Name != value)
                {
                    this._Name = value;
                    NotifyPropertyChanged("Name");
                }
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
                if (this._LoadSheddingCost != value)
                {
                    this._LoadSheddingCost = value;
                    NotifyPropertyChanged("LoadSheddingCost");
                }
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

        private string _FullFileName;

        /// <summary>
        /// The full file name (including path) of the target XML file for saving this power system.
        /// </summary>
        public string FullFileName
        {
            get
            {
                return _FullFileName;
            }

            set
            {
                if (this._FullFileName != value)
                {
                    this._FullFileName = value;
                    NotifyPropertyChanged("FullFileName");
                }
            }
        }

        /// <summary>
        /// Indicates whether the target XML file for saving this power system exists.
        /// </summary>
        public bool IsSaved { get { return File.Exists(this.FullFileName); } }

        public PowerSystem()
        {
            //new objects are added directly by the GUI
            this._Nodes = new BindingList<Node>();
            this._Nodes.AddingNew += (sender, e) => { e.NewObject = new Node(this); };
            this._GeneratingUnits = new BindingList<GeneratingUnit>();
            this._GeneratingUnits.AddingNew += (sender, e) => { e.NewObject = new GeneratingUnit(this); };
            this._InelasticLoads = new BindingList<InelasticLoad>();
            this._InelasticLoads.AddingNew += (sender, e) => { e.NewObject = new InelasticLoad(this); };
            this._TransmissionLines = new BindingList<TransmissionLine>();
            this._TransmissionLines.AddingNew += (sender, e) => { e.NewObject = new TransmissionLine(this); };
        }

        public PowerSystem(string name) : this()
        {
            this.Name = name;
        }

        public void saveToXMLFile()
        {
            using (TextWriter saveStream = new StreamWriter(this.FullFileName))
            {
                // Save document
                System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(PowerSystem));
                writer.Serialize(saveStream, this);
            }
            logger.Info("Current power system (named '{0}') saved in {1}.", this.Name, this.FullFileName);
        }

        public void saveToXMLFile(string file_name)
        {
            this.FullFileName = file_name;
            this.saveToXMLFile();
        }

        /// <summary>
        /// Creates a Power System by reading an XML file, provided the stream.
        /// </summary>
        /// <param name="xmlStream">Full file path of the XML file where the power system was stored.</param>
        /// <returns>A new power system object with the contents serialized in the XML file.</returns>
        /// <remarks>
        /// The provided stream must is not closed within this method.
        /// IOException and other exceptions are not managed.</remarks>
        public static PowerSystem readFromXMLFile(string filename)
        {
            PowerSystem retval = null;
            using (StreamReader xmlStream = new StreamReader(filename))
            {
                System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PowerSystem));
                retval = (PowerSystem)reader.Deserialize(xmlStream);
                foreach (Node node in retval.Nodes)
                {
                    node.PowerSystem = retval;
                }
            }
            return retval;
        }
    }
}
