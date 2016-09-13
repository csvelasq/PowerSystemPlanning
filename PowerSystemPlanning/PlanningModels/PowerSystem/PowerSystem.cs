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
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        
        /// <summary>
        /// Nodes of the power system, bindable to GUI.
        /// </summary>
        public BindingList<Node> _Nodes { get; protected set; }

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
        public BindingList<GeneratingUnit> _GeneratingUnits { get; protected set; }

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
        public BindingList<InelasticLoad> _InelasticLoads { get; protected set; }

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
        public BindingList<TransmissionLine> _TransmissionLines { get; protected set; }

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
            LoadSheddingCost = 10000; //default (arbitrary) value
            //new objects are added directly by the GUI
            _Nodes = new BindingList<Node>();
            _Nodes.AddingNew += (sender, e) => { e.NewObject = new Node(this); };
            _GeneratingUnits = new BindingList<GeneratingUnit>();
            _GeneratingUnits.AddingNew += (sender, e) => { e.NewObject = new GeneratingUnit(this); };
            _InelasticLoads = new BindingList<InelasticLoad>();
            _InelasticLoads.AddingNew += (sender, e) => { e.NewObject = new InelasticLoad(this); };
            _TransmissionLines = new BindingList<TransmissionLine>();
            _TransmissionLines.AddingNew += (sender, e) => { e.NewObject = new TransmissionLine(this); };
        }
    }
}
