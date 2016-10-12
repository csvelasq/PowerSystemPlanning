using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;
using System.IO;

namespace PowerSystemPlanningWpfApp.ApplicationWide.ViewModels
{
    public class ScenarioTepSetupViewModel : BaseDocumentOneFolderViewModel
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PowerSystem MyPowerSystem { get; protected set; }

        public BindableTepModel MyTepModel { get; protected set; }

        public ScenarioEditorViewModel ScenarioVm { get; protected set; }
        public CandidateTransmissionLinesEditorViewModel CandidatesVm { get; protected set; }

        public override string Title => "TEP under Scenarios";

        public ScenarioTepSetupViewModel(PowerSystem system)
        {
            MyPowerSystem = system;
            //Create TEP model and add default scenarios (s1 & s2) and states (peak & valley)
            MyTepModel = new BindableTepModel(MyPowerSystem);
            var defaultScenarios =
                BindableStaticScenarioCollection.CreateDefaultScenarios(MyPowerSystem);
            MyTepModel.MyBindableScenarios.MyScenarios = defaultScenarios;
            //Create scenario view-model
            ScenarioVm = new ScenarioEditorViewModel(MyPowerSystem, MyTepModel.MyBindableScenarios);
            //Create Candidate Transmission lines view-model
            CandidatesVm = new CandidateTransmissionLinesEditorViewModel(MyPowerSystem.BindingTransmissionLines);
            CandidatesVm.PropertyChanged += CandidatesVm_PropertyChanged;
        }

        private void CandidatesVm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CandidatesVm.CandidateLines))
            {
                MyTepModel.MyBindableCandidateTransmissionLines = CandidatesVm.CandidateLines;
            }
        }

        public override void SaveToFolder()
        {
            //save full tep model
            var xmlpath = Path.Combine(FolderAbsolutePath, "tep_model.xml");
            MyTepModel.SaveToXml(xmlpath);

            logger.Info($"Tep under scenarios saved to '{FolderAbsolutePath}'.");
        }
    }
}
