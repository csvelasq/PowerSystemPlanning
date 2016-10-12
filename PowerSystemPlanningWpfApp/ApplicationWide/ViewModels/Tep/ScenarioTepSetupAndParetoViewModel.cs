using NLog;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios;
using PowerSystemPlanning.Solvers.ScenarioTEP;
using PowerSystemPlanning.Solvers.ScenarioTEP.BruteForcePareto;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide.ViewModels
{
    public class ScenarioTepSetupAndParetoViewModel : BaseDocumentOneFolderViewModel
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        PowerSystem _MyPowerSystem;
        public PowerSystem MyPowerSystem
        {
            get { return _MyPowerSystem; }
            protected set
            {
                if (_MyPowerSystem != value)
                {
                    _MyPowerSystem = value;
                    OnPropertyChanged();
                    SetupVm = new ScenarioTepSetupViewModel(MyPowerSystem);
                }
            }
        }

        public ScenarioTepSetupViewModel SetupVm { get; protected set; }

        #region Commands
        public DelegateCommand GoToParetoBuilderCommand { get; protected set; }
        #endregion

        #region For Docking Manager
        public override string Title => SetupVm.Title;

        public override void SaveToFolder()
        {
            //SetupVm.SaveToFolder();
            //save full tep model
            var xmlpath = Path.Combine(FolderAbsolutePath, "tep_model.xml");
            SetupVm.MyTepModel.SaveToXml(xmlpath);

            logger.Info($"Tep under scenarios saved to '{FolderAbsolutePath}'.");
        }
        #endregion

        public ScenarioTepSetupAndParetoViewModel()
        {
            //Commands
            GoToParetoBuilderCommand = new DelegateCommand(GoToParetoBuilder);
        }

        public ScenarioTepSetupAndParetoViewModel(PowerSystem system) : this()
        {
            MyPowerSystem = system;
        }

        public ScenarioTepSetupAndParetoViewModel(string xmlPath) : this()
        {
            var tepModel = BindableTepModel.LoadFromXml(xmlPath);
            MyPowerSystem = tepModel.BindablePowerSystem;
        }

        private void GoToParetoBuilder()
        {
            var mooTepModel = new MooStaticTepSimulationModel(SetupVm.MyTepModel);
            var paretoBuilder = new SolverTepScenariosParetoFrontBruteForceBuilder(mooTepModel);
            var paretoBuilderVm = new ScenarioTEPParetoViewModel(paretoBuilder);
            NotifyNewDocumentOpened(paretoBuilderVm);
        }
    }
}
