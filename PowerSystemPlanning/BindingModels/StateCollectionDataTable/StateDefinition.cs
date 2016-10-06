using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.BindingModels.StateCollectionDataTable
{
    [DataContract()]
    public class StateDefinition : SerializableBindableBase
    {
        string _StateName;

        [DataMember()]
        public string StateName
        {
            get
            {
                return _StateName;
            }

            set
            {
                SetProperty<string>(ref _StateName, value);
            }
        }

        string _ScenarioName;

        [DataMember()]
        public string ScenarioName
        {
            get
            {
                return _ScenarioName;
            }

            set
            {
                SetProperty<string>(ref _ScenarioName, value);
            }
        }

        double _Duration;

        [DataMember()]
        public double Duration
        {
            get
            {
                return _Duration;
            }

            set
            {
                SetProperty<double>(ref _Duration, value);
            }
        }
    }
}
