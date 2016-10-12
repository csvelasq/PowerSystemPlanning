using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios;
using PowerSystemPlanning.Solvers.ScenarioTEP;
using PowerSystemPlanning.Solvers.ScenarioTEP.BruteForcePareto;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide.ViewModels
{
    public class ScenarioTepSetupAndParetoViewModel : BaseDocumentOneFolderViewModel
    {
        public PowerSystem MyPowerSystem { get; protected set; }

        public ScenarioTepSetupViewModel SetupVm { get; protected set; }

        #region Commands
        public DelegateCommand GoToParetoBuilderCommand { get; protected set; }
        #endregion

        #region For Docking Manager
        public override string Title => SetupVm.Title;

        public override void SaveToFolder()
        {
            SetupVm.SaveToFolder();
        }
        #endregion

        public ScenarioTepSetupAndParetoViewModel(PowerSystem system)
        {
            MyPowerSystem = system;
            SetupVm = new ScenarioTepSetupViewModel(system);
            //Commands
            GoToParetoBuilderCommand = new DelegateCommand(GoToParetoBuilder);
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
