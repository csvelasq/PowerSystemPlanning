using PowerSystemPlanning.BindingModels;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide.AppModels
{
    [DataContract()]
    public class PowerSystemDefinition : SerializableBindableBase
    {
        public PowerSystem MyPowerSystem { get; set; }

        string _MyPowerSystemName;
        [DataMember()]
        public string MyPowerSystemName
        {
            get
            {
                return _MyPowerSystemName;
            }

            set
            {
                SetProperty<string>(ref _MyPowerSystemName, value);
            }
        }
    }
}
