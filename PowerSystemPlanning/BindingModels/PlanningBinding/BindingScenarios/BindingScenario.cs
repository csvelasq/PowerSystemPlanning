using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.Models.SystemState;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemPlanning.Models.SystemBaseData;
using System.ComponentModel;
using PowerSystemPlanning.BindingModels.StateBinding;
using System.Runtime.Serialization;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios
{
    /// <summary>
    /// Encapsulates power system data in a particular future scenario being modeled.
    /// </summary>
    [DataContract()]
    public class BindingScenario : SerializableBindableBase, IPowerSystemStateCollection
    {
        PowerSystem _BindablePowerSystem;
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

        public IPowerSystem MyPowerSystem => BindablePowerSystem;

        string _Name;
        /// <summary>
        /// The name of this scenario.
        /// </summary>
        [DataMember()]
        public string Name
        {
            get { return _Name; }
            set { SetProperty<string>(ref _Name, value); }
        }
        /// <summary>
        /// The probability that this scenario occurs
        /// </summary>
        [DataMember()]
        public double Probability { get; set; } = 0;

        BindingList<PowerSystemState> _BindableStates;
        /// <summary>
        /// The set of states this scenario is composed of.
        /// </summary>
        [DataMember()]
        public BindingList<PowerSystemState> BindableStates
        {
            get { return _BindableStates; }
            protected set { SetProperty<BindingList<PowerSystemState>>(ref _BindableStates, value); }
        }

        public IEnumerable<IPowerSystemState> MyPowerSystemStates => BindableStates;

        #region Summary Properties
        public double PeakLoad => (from state in BindableStates
                                   select state.PeakLoad).Max();
        public double TotalLoad => (from state in BindableStates
                                    select state.TotalLoad).Sum();
        public double AvailableGeneratingCapacity => (from state in BindableStates
                                                      select state.AvailableGeneratingCapacity).Sum();
        #endregion

        public BindingScenario()
        {
            BindableStates = new BindingList<PowerSystemState>();
            BindableStates.Add(new PowerSystemState() { Name = "Peak", Duration = 760 });
            BindableStates.Add(new PowerSystemState() { Name = "Valley", Duration = 8000 });
        }

        public BindingScenario(string name, PowerSystem powerSystem) : this()
        {
            Name = name;
            BindablePowerSystem = powerSystem;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
