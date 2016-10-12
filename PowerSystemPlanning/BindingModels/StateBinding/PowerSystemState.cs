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
    /// <summary>
    /// A particular state of a power system.
    /// </summary>
    [DataContract()]
    public class PowerSystemState : SerializableBindableBase, IPowerSystemState
    {
        protected string _Name;
        /// <summary>
        /// Name of this state.
        /// </summary>
        [DataMember()]
        public string Name
        {
            get { return _Name; }
            set { SetProperty<string>(ref _Name, value); }
        }

        protected double _Duration;
        /// <summary>
        /// The duration (hours) of this state.
        /// </summary>
        [DataMember()]
        public double Duration
        {
            get { return _Duration; }
            set { SetProperty<double>(ref _Duration, value); }
        }

        [DataMember()]
        public PowerSystem MyBindingPowerSystem { get; set; }

        [DataMember()]
        public BindingList<NodeState> BindingNodeStates { get; set; }

        [DataMember()]
        public BindingList<GeneratingUnitState> BindingGeneratingUnitStates { get; set; }

        [DataMember()]
        public BindingList<InelasticLoadState> BindingInelasticLoadStates { get; set; }

        [DataMember()]
        public BindingList<SimpleTransmissionLineState> BindingSimpleTransmissionLineStates { get; set; }

        #region Summary Properties
        public double PeakLoad => (from load in BindingInelasticLoadStates
                                   select load.Consumption).Max();
        public double TotalLoad => (from load in BindingInelasticLoadStates
                                    select load.Consumption * Duration / 1e3).Sum();
        public double AvailableGeneratingCapacity => (from gen in BindingGeneratingUnitStates
                                                      select gen.AvailableCapacity).Sum();
        #endregion

        #region IPowerSystemState implementation
        public IPowerSystem MyPowerSystem => MyBindingPowerSystem;

        public IList<INodeState> NodeStates => BindingNodeStates.Cast<INodeState>().ToList();

        public IList<IGeneratingUnitState> GeneratingUnitStates => BindingGeneratingUnitStates.Cast<IGeneratingUnitState>().ToList();

        public IList<IGeneratingUnitState> ActiveGeneratingUnitStates =>
            (from gen in BindingGeneratingUnitStates
             where gen.IsAvailable
             select (IGeneratingUnitState)gen).ToList();

        public IList<IInelasticLoadState> InelasticLoadStates => BindingInelasticLoadStates.Cast<IInelasticLoadState>().ToList();

        public IList<ISimpleTransmissionLineState> SimpleTransmissionLineStates => BindingSimpleTransmissionLineStates.Cast<ISimpleTransmissionLineState>().ToList();

        public IList<ISimpleTransmissionLineState> ActiveSimpleTransmissionLineStates =>
            (from tl in BindingSimpleTransmissionLineStates
             where tl.IsAvailable
             select (ISimpleTransmissionLineState)tl).ToList();
        #endregion

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
