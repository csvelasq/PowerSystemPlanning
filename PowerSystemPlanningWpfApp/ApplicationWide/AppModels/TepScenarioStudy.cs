using PowerSystemPlanning.BindingModels;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios;
using PowerSystemPlanning.BindingModels.StateCollectionDataTable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PowerSystemPlanningWpfApp.ApplicationWide.AppModels
{
    public class TepScenarioStudy : StudyInLocalFolder
    {
        public override string GenericName => "TEP-Scenarios";

        public BindingTepScenarios MyTepStudy { get; set; }

        #region filenames
        public string TepXmlStatesDefinitionAbsolutePath => Path.Combine(FolderAbsolutePath, InstanceName + ".xml");
        public string TepCsvStatesDataAbsolutePath => Path.Combine(FolderAbsolutePath, InstanceName + ".csv");
        #endregion

        public TepScenarioStudy(PowerSystemInLocalFolder owner) : base(owner)
        {
        }

        public override void SaveStudy()
        {
            /*
             * Save input data
             */
            MyTepStudy.Save(TepXmlStatesDefinitionAbsolutePath,
                TepCsvStatesDataAbsolutePath);
            /*
             * Save results
             */
        }

        public override void Open()
        {
            /*
             * Open input data
             */
            MyTepStudy = BindingTepScenarios.Load(MyOwnerPowerSys.MyPowerSystem, TepXmlStatesDefinitionAbsolutePath, TepCsvStatesDataAbsolutePath);
            /*
             * Open results
             */
        }
    }
}
