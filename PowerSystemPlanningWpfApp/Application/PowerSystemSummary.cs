using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.Models.SystemBaseData;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.Application
{
    public class PowerSystemSummary : BindableBase
    {
        public PowerSystem MyUnderlyingPowerSystem { get; protected set; }

        /// <summary>
        /// The name of this power system.
        /// </summary>
        public string Name => MyUnderlyingPowerSystem.Name;

        public string DirectoryRelativePath => Name;
    }
}
