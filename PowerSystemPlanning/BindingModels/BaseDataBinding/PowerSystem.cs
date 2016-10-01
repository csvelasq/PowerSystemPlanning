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
    public class PowerSystem : BindableBase, IPowerSystem
    {
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

        private double _LoadSheddingCost = 10000; //default (arbitrary) value
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
        public IList<ISimpleTransmissionLine> TransmissionLines =>this.BindingTransmissionLines.Cast<ISimpleTransmissionLine>().ToList();

        public PowerSystem()
        {
            //new objects are added directly by the GUI
            //catching the event allows for making new objects reference this power system
            // TODO attach addingnew handler directly in the setter for each of the following bindinglist (_Nodes, _GeneratingUnits, ...)
            _Nodes = new BindingList<Node>();
            _Nodes.AddingNew += (sender, e) => { e.NewObject = new Node(this); };
            BindingGeneratingUnits = new BindingList<GeneratingUnit>();
            BindingGeneratingUnits.AddingNew += (sender, e) => { e.NewObject = new GeneratingUnit(this); };
            BindingInelasticLoads = new BindingList<InelasticLoad>();
            BindingInelasticLoads.AddingNew += (sender, e) => { e.NewObject = new InelasticLoad(this); };
            _TransmissionLines = new BindingList<TransmissionLine>();
            _TransmissionLines.AddingNew += (sender, e) => { e.NewObject = new TransmissionLine(this); };
        }
    }
}
