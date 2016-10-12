using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using PowerSystemPlanning.MultiObjective;
using PowerSystemPlanning.Solvers.ScenarioTEP;
using PowerSystemPlanning.Solvers.ScenarioTEP.BruteForcePareto;
using PowerSystemPlanningWpfApp.Analysis.ScenarioTEP.Enumerate;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PowerSystemPlanningWpfApp.ApplicationWide.ViewModels
{
    public class ScenarioTEPParetoViewModel : BaseDocumentOneFolderViewModel
    {
        #region Internal fields
        public override string Title => "Pareto front builder";
        ScenarioTepAlternativesEnumControlViewModel _AllAlternativesViewModel;
        ScenarioTepAlternativesEnumControlViewModel _ParetoAlternativesViewModel;
        #endregion

        public SolverTepScenariosParetoFrontBruteForceBuilder MyParetoFrontBuilder { get; protected set; }

        public ScenarioTepAlternativesEnumControlViewModel ParetoAlternativesViewModel
        {
            get
            {
                return _ParetoAlternativesViewModel;
            }

            set
            {
                SetProperty<ScenarioTepAlternativesEnumControlViewModel>(ref _ParetoAlternativesViewModel, value);
            }
        }

        public ScenarioTepAlternativesEnumControlViewModel AllAlternativesViewModel
        {
            get
            {
                return _AllAlternativesViewModel;
            }

            set
            {
                SetProperty<ScenarioTepAlternativesEnumControlViewModel>(ref _AllAlternativesViewModel, value);
            }
        }

        #region Commands
        public ICommand BuildParetoFrontier { get; private set; }

        private void RunBuildParetoFrontier()
        {
            MyParetoFrontBuilder.Solve();
            AllAlternativesViewModel = new ScenarioTepAlternativesEnumControlViewModel(MyParetoFrontBuilder.AllTransmissionExpansionAlternatives);
            ParetoAlternativesViewModel = new ScenarioTepAlternativesEnumControlViewModel(MyParetoFrontBuilder.EfficientTransmissionExpansionAlternatives);
        }
        #endregion

        public ScenarioTEPParetoViewModel(SolverTepScenariosParetoFrontBruteForceBuilder paretoSolver)
        {
            MyParetoFrontBuilder = paretoSolver;
            //Commands
            BuildParetoFrontier = new DelegateCommand(RunBuildParetoFrontier);
        }

        public override void SaveToFolder()
        {
            throw new NotImplementedException();
        }
    }
}
