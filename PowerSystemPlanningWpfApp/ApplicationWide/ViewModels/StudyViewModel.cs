using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanningWpfApp.ApplicationWide.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    public abstract class StudyViewModel : BaseDocumentViewModel
    {
        public PowerSystem MyPowerSystem { get; set; }

        public virtual StudyInLocalFolder MyStudy { get; set; }

        public override string Title => MyStudy.MyTepScenarioStudy.InstanceName;
    }
}
