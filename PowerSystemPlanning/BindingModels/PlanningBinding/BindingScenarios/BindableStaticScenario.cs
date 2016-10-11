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
using PowerSystemPlanning.Models.Planning.Scenarios;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios
{
    /// <summary>
    /// Encapsulates power system data in a particular future scenario being modeled.
    /// </summary>
    [DataContract()]
    public class BindableStaticScenario : SerializableBindableBase, IStaticScenario
    {
        #region internal fields
        PowerSystem _BindablePowerSystem;
        string _Name;
        double _Probability;
        BindableStaticStateCollection _MyStaticStateCollection;
        #endregion

        #region interface implementation
        public IPowerSystem MyPowerSystem => BindablePowerSystem;
        public IPowerSystemStateCollection MyStateCollection => MyStaticStateCollection;
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
        public double Probability
        {
            get { return _Probability; }
            set { SetProperty<double>(ref _Probability, value); }
        }

        [DataMember()]
        public BindableStaticStateCollection MyStaticStateCollection
        {
            get { return _MyStaticStateCollection; }
            set { SetProperty<BindableStaticStateCollection>(ref _MyStaticStateCollection, value); }
        }
        
        public BindableStaticScenario()
        {
        }

        public BindableStaticScenario(PowerSystem powerSystem, string name, double probability,
            IList<PowerSystemState> states)
            : this()
        {
            BindablePowerSystem = powerSystem;
            Name = name;
            Probability = probability;
            MyStaticStateCollection = new BindableStaticStateCollection(states);
        }

        public BindableStaticScenario(PowerSystem powerSystem, string name, double probability)
            : this()
        {
            BindablePowerSystem = powerSystem;
            Name = name;
            Probability = probability;
            MyStaticStateCollection = new BindableStaticStateCollection();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
