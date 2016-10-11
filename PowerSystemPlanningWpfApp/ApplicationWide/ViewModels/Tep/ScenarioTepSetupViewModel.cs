using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanningWpfApp.ApplicationWide.ViewModels;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    public class ScenarioTepSetupViewModel : BaseDocumentOneFolderViewModel
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PowerSystem MyPowerSystem { get; protected set; }
        public ScenarioEditorViewModel ScenarioVm { get; protected set; }
        public CandidateTransmissionLinesEditorViewModel CandidatesVm { get; protected set; }

        public override string Title => "TEP under Scenarios";

        public ScenarioTepSetupViewModel(PowerSystem system)
        {
            MyPowerSystem = system;
            ScenarioVm = new ScenarioEditorViewModel(system);
            CandidatesVm = new CandidateTransmissionLinesEditorViewModel(system.BindingTransmissionLines);
        }

        public override void SaveToFolder()
        {
            //Save scenario definition
            ScenarioVm.SaveToFolder();
            //Save candidate transmission lines definition

            logger.Info($"Tep under scenarios saved to '{FolderAbsolutePath}'.");
        }
    }
}
