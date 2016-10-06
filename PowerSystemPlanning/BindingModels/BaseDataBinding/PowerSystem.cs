using Prism.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Generator;
using PowerSystemPlanning.Models.SystemBaseData.Load;
using PowerSystemPlanning.Models.SystemBaseData.Nodes;
using PowerSystemPlanning.Models.SystemBaseData.Branch;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Branch;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Load;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Generator;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Nodes;
using System.Xml;
using System.IO;

namespace PowerSystemPlanning.BindingModels.BaseDataBinding
{
    /// <summary>
    /// Represents the technical and economic model of a Power System for medium to long term planning.
    /// </summary>
    /// <remarks>
    /// Basic object model of a power system for the purpose of solving planning models in the medium to long term, with DC power flow.
    /// The purpose of this class is to allow easy end-user manipulation and edition of the power system model.
    /// </remarks>
    [DataContract()]
    public class PowerSystem : SerializableBindableBase, IPowerSystem
    {
        string _Name;

        [DataMember()]
        public string Name
        {
            get { return _Name; }
            set { SetProperty<string>(ref _Name, value); }
        }

        /// <summary>
        /// Nodes of the power system, bindable to GUI.
        /// </summary>
        [DataMember()]
        public BindingList<Node> BindingNodes { get; set; }
        /// <summary>
        /// Nodes of the power system.
        /// </summary>
        public IList<INode> Nodes => this.BindingNodes.Cast<INode>().ToList();

        /// <summary>
        /// All Generating units in the power system, bindable to GUI.
        /// </summary>
        [DataMember()]
        public BindingList<GeneratingUnit> BindingGeneratingUnits { get; set; }
        /// <summary>
        /// All Generating units in the power system
        /// </summary>
        public IList<IGeneratingUnit> GeneratingUnits => this.BindingGeneratingUnits.Cast<IGeneratingUnit>().ToList();

        public double TotalGeneratingCapacity => (from gen in this.BindingGeneratingUnits select gen.InstalledCapacity).Sum();

        /// <summary>
        /// All Inelastic loads in the power system, bindable to GUI.
        /// </summary>
        [DataMember()]
        public BindingList<InelasticLoad> BindingInelasticLoads { get; set; }
        /// <summary>
        /// All inelastic loads in the power system.
        /// </summary>
        public IList<IInelasticLoad> InelasticLoads => this.BindingInelasticLoads.Cast<IInelasticLoad>().ToList();

        /// <summary>
        /// Total inelastic loads (MW).
        /// </summary>
        public double TotalInelasticLoad => (from load in this.BindingInelasticLoads select load.Consumption).Sum();

        private double _LoadSheddingCost = 2000; //default (arbitrary) value
        /// <summary>
        /// The cost (in US$) of shedding 1 MWh of any load.
        /// </summary>
        [DataMember()]
        public double LoadSheddingCost
        {
            get { return this._LoadSheddingCost; }
            set { SetProperty<double>(ref _LoadSheddingCost, value); }
        }

        /// <summary>
        /// All Transmission lines in the power sytem, bindable to GUI.
        /// </summary>
        [DataMember()]
        public BindingList<SimpleTransmissionLine> BindingTransmissionLines { get; set; }
        /// <summary>
        /// All Transmission lines in the power sytem.
        /// </summary>
        public IList<ISimpleTransmissionLine> TransmissionLines => this.BindingTransmissionLines.Cast<ISimpleTransmissionLine>().ToList();

        public List<PowerSystemElement> AllBindableElements
        {
            get
            {
                var elements = new List<PowerSystemElement>();
                elements.AddRange(BindingNodes);
                elements.AddRange(BindingGeneratingUnits);
                elements.AddRange(BindingInelasticLoads);
                elements.AddRange(BindingTransmissionLines);
                return elements;
            }
        }

        public PowerSystem()
        {
            //new objects are added directly by the GUI
            //catching the event allows for making new objects reference this power system
            // TODO attach addingnew handler directly in the setter for each of the following bindinglist (_Nodes, _GeneratingUnits, ...)
            BindingNodes = new BindingList<Node>();
            BindingGeneratingUnits = new BindingList<GeneratingUnit>();
            BindingInelasticLoads = new BindingList<InelasticLoad>();
            BindingTransmissionLines = new BindingList<SimpleTransmissionLine>();
            DefaultNewElementsInBindingLists();
        }

        /// <summary>
        /// Catches addingNew events in each binding list to automatically assign an ID to each element.
        /// </summary>
        public void DefaultNewElementsInBindingLists()
        {
            BindingNodes.AddingNew += (sender, e) => { e.NewObject = new Node(this); };
            BindingInelasticLoads.AddingNew += (sender, e) => { e.NewObject = new InelasticLoad(this); };
            BindingGeneratingUnits.AddingNew += (sender, e) => { e.NewObject = new GeneratingUnit(this); };
            BindingTransmissionLines.AddingNew += (sender, e) => { e.NewObject = new SimpleTransmissionLine(this); };
        }

        /// <summary>
        /// Serializes this power system to an XML file.
        /// </summary>
        /// <param name="xmlPath">The output path for the XML file with the serialized power system.</param>
        public void SaveToXml(string xmlPath)
        {
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            DataContractSerializer dcs = new DataContractSerializer(typeof(PowerSystem), dcsSettings);
            var xmlSettings = new XmlWriterSettings { Indent = true };
            using (var myXmlWriter = XmlWriter.Create(xmlPath, xmlSettings))
            {
                dcs.WriteObject(myXmlWriter, this);
            }
        }

        public static PowerSystem OpenFromXml(string xmlPath)
        {
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            DataContractSerializer dcs = new DataContractSerializer(typeof(PowerSystem), dcsSettings);
            FileStream fs = new FileStream(xmlPath, FileMode.Open);
            XmlDictionaryReader reader =
            XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

            //Deserialize the power system
            PowerSystem deserializedPowerSystem = (PowerSystem)dcs.ReadObject(reader);

            //Bind elements to the power system (nodes, gens, loads, and tls)
            foreach (var element in deserializedPowerSystem.AllBindableElements)
            {
                element.MyBindingPowerSystem = deserializedPowerSystem;
            }

            //Automatically assign IDs to new elements in the power system
            deserializedPowerSystem.DefaultNewElementsInBindingLists();

            return deserializedPowerSystem;
        }
    }
}
