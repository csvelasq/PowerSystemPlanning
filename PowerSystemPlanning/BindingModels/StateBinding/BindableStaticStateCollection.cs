using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemState;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.BindingModels.StateBinding
{
    [DataContract()]
    public class BindableStaticStateCollection : SerializableBindableBase, IStaticStateCollection
    {
        #region internal fields
        PowerSystem _BindablePowerSystem;
        BindingList<PowerSystemState> _BindableStates;
        #endregion

        #region interface implementation
        public IPowerSystem MyPowerSystem => BindablePowerSystem;
        public IEnumerable<IPowerSystemState> MyPowerSystemStates => BindableStates;
        public IList<IPowerSystemState> MyStaticStates => (from state in BindableStates select (IPowerSystemState)state).ToList();
        #endregion

        /// <summary>
        /// The underlying power system data for this scenario
        /// </summary>
        [DataMember()]
        public PowerSystem BindablePowerSystem
        {
            get { return _BindablePowerSystem; }
            set
            {
                if (_BindablePowerSystem != value)
                {
                    _BindablePowerSystem = value;
                    //foreach (var state in BindableStates)
                    //{
                    //    state.MyBindingPowerSystem = BindablePowerSystem;
                    //}
                }
            }
        }

        /// <summary>
        /// The set of states this scenario is composed of.
        /// </summary>
        [DataMember()]
        public BindingList<PowerSystemState> BindableStates
        {
            get { return _BindableStates; }
            protected set { SetProperty<BindingList<PowerSystemState>>(ref _BindableStates, value); }
        }
        
        #region Summary Properties
        public double PeakLoad => (from state in BindableStates
                                   select state.PeakLoad).Max();
        public double TotalLoad => (from state in BindableStates
                                    select state.TotalLoad).Sum();
        public double AvailableGeneratingCapacity =>
            (from state in BindableStates
             select state.AvailableGeneratingCapacity).Sum();
        #endregion

        public BindableStaticStateCollection()
        {
            BindableStates = new BindingList<PowerSystemState>();
        }

        public BindableStaticStateCollection(PowerSystem system) : this()
        {
            BindablePowerSystem = system;
        }

        public BindableStaticStateCollection(IList<PowerSystemState> states)
        {
            BindableStates = new BindingList<PowerSystemState>(states);
        }

        public static BindableStaticStateCollection CreateDefaultStateCollection(PowerSystem system)
        {
            var states = new BindableStaticStateCollection(system);
            states.BindableStates.Add(new PowerSystemState() { Name = "Peak", Duration = 760 });
            states.BindableStates.Add(new PowerSystemState() { Name = "Valley", Duration = 8000 });
            return states;
        }
    }
}
