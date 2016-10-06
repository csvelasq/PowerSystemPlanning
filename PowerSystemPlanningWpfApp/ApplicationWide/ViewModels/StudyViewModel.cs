using PowerSystemPlanningWpfApp.ApplicationWide.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide.ViewModels
{
    public class StudyViewModel : BaseDocumentViewModel
    {
        public StudyInLocalFolder MyStudy { get; set; }

        public override string Title => MyStudy.MyTepScenarioStudy.InstanceName;
    }
}
