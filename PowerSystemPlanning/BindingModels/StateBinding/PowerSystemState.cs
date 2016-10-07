using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.StateBinding.Branch;
using PowerSystemPlanning.BindingModels.StateBinding.Generator;
using PowerSystemPlanning.BindingModels.StateBinding.Load;
using PowerSystemPlanning.BindingModels.StateBinding.Nodes;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Models.SystemState.Branch;
using PowerSystemPlanning.Models.SystemState.Generator;
using PowerSystemPlanning.Models.SystemState.Load;
using PowerSystemPlanning.Models.SystemState.Nodes;
using Prism.Mvvm;

namespace PowerSystemPlanning.BindingModels.StateBinding
{
    [DataContract()]
    public class PowerSystemState : SerializableBindableBase, IPowerSystemState
    {
        protected string _Name;
        [DataMember()]
        public string Name
        {
            get { return _Name; }
            set { SetProperty<string>(ref _Name, value); }
        }

        protected double _Duration;
        [DataMember()]
        public double Duration
        {
            get { return _Duration; }
            set { SetProperty<double>(ref _Duration, value); }
        }

        public PowerSystem MyBindingPowerSystem { get; set; }
        public IPowerSystem MyPowerSystem => MyBindingPowerSystem;

        public BindingList<NodeState> BindingNodeStates { get; set; }
        public IList<INodeState> NodeStates => BindingNodeStates.Cast<INodeState>().ToList();

        public BindingList<GeneratingUnitState> BindingGeneratingUnitStates { get; set; }
        public IList<IGeneratingUnitState> GeneratingUnitStates => BindingGeneratingUnitStates.Cast<IGeneratingUnitState>().ToList();

        public IList<IGeneratingUnitState> ActiveGeneratingUnitStates =>
            (from gen in BindingGeneratingUnitStates
             where gen.IsAvailable
             select (IGeneratingUnitState)gen).ToList();

        public BindingList<InelasticLoadState> BindingInelasticLoadStates { get; set; }
        public IList<IInelasticLoadState> InelasticLoadStates => BindingInelasticLoadStates.Cast<IInelasticLoadState>().ToList();

        public BindingList<SimpleTransmissionLineState> BindingSimpleTransmissionLineStates { get; set; }
        public IList<ISimpleTransmissionLineState> SimpleTransmissionLineStates => BindingSimpleTransmissionLineStates.Cast<ISimpleTransmissionLineState>().ToList();

        public IList<ISimpleTransmissionLineState> ActiveSimpleTransmissionLineStates =>
            (from tl in BindingSimpleTransmissionLineStates
             where tl.IsAvailable
             select (ISimpleTransmissionLineState)tl).ToList();

        public PowerSystemState()
        {
        }

        public PowerSystemState(PowerSystem system) : this()
        {
            InitializeToPowerSystem(system);
        }

        /// <summary>
        /// Call upon initialization and after deserializing basic information (name and duration).
        /// </summary>
        /// <param name="system"></param>
        public void InitializeToPowerSystem(PowerSystem system)
        {
            MyBindingPowerSystem = system;
            //Add generator states
            BindingGeneratingUnitStates = new BindingList<GeneratingUnitState>();
            foreach (var gen in MyBindingPowerSystem.BindingGeneratingUnits)
            {
                BindingGeneratingUnitStates.Add(new GeneratingUnitState(this, gen));
            }
            //Add load states
            BindingInelasticLoadStates = new BindingList<InelasticLoadState>();
            foreach (var load in MyBindingPowerSystem.BindingInelasticLoads)
            {
                BindingInelasticLoadStates.Add(new InelasticLoadState(this, load));
            }
            //Add transmission lines states
            BindingSimpleTransmissionLineStates = new BindingList<SimpleTransmissionLineState>();
            foreach (var tl in MyBindingPowerSystem.BindingTransmissionLines)
            {
                BindingSimpleTransmissionLineStates.Add(new SimpleTransmissionLineState(this, tl));
            }
            //Add node states
            BindingNodeStates = new BindingList<NodeState>();
            foreach (var node in MyBindingPowerSystem.BindingNodes)
            {
                BindingNodeStates.Add(new NodeState(this, node));
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
