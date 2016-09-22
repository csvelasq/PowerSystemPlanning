using PowerSystemPlanning.MultiObjective;
using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.PlanningModels.Planning;
using PowerSystemPlanning.Solvers.ScenarioTEP;
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

namespace PowerSystemPlanningWpfApp.Analysis.ScenarioTEP.BruteForcePareto
{
    public class ScenarioTEPParetoViewModel : BindableBase
    {
        private ScenarioTEPModel _MyScenarioTEPModel;
        public ScenarioTEPModel MyScenarioTEPModel
        {
            get { return _MyScenarioTEPModel; }
            set
            {
                SetProperty<ScenarioTEPModel>(ref _MyScenarioTEPModel, value);
                MyParetoBuilder = new ScenarioTEPMOOParetoBruteForce(MyScenarioTEPModel);
                TepAlternativesInParetoFront = null;
                AllPossibleTEPAlternatives = null;
                ObjectiveFunctionsName = (from function in MyParetoBuilder.MyTEPMOOProblem.MyObjectiveFunctionsDefinition
                                          select function.MyObjectiveName);
            }
        }

        private IEnumerable<string> _ObjectiveFunctionsName;
        public IEnumerable<string> ObjectiveFunctionsName
        {
            get { return _ObjectiveFunctionsName; }
            set
            {
                SetProperty<IEnumerable<string>>(ref _ObjectiveFunctionsName, value);
            }
        }

        private ScenarioTEPMOOParetoBruteForce _MyParetoBuilder;
        public ScenarioTEPMOOParetoBruteForce MyParetoBuilder
        {
            get
            {
                return _MyParetoBuilder;
            }

            set
            {
                SetProperty<ScenarioTEPMOOParetoBruteForce>(ref _MyParetoBuilder, value);
            }
        }

        private BaseMultiObjectiveIndividualList _TepAlternativesInParetoFront;
        public BaseMultiObjectiveIndividualList TepAlternativesInParetoFront
        {
            get
            {
                return _TepAlternativesInParetoFront;
            }

            set
            {
                SetProperty<BaseMultiObjectiveIndividualList>(ref _TepAlternativesInParetoFront, value);
                if (TepAlternativesInParetoFront != null && TepAlternativesInParetoFront.Count > 0)
                    ParetoAlternativesViewModel.TepAlternatives =
                        (from alt in TepAlternativesInParetoFront
                         select (TransmissionExpansionPlan)alt);
            }
        }

        public ScenarioTepAlternativesEnumControlViewModel ParetoAlternativesViewModel { get; set; }

        private List<TransmissionExpansionPlan> _AllPossibleTEPAlternatives;
        public List<TransmissionExpansionPlan> AllPossibleTEPAlternatives
        {
            get { return _AllPossibleTEPAlternatives; }
            set
            {
                SetProperty<List<TransmissionExpansionPlan>>(ref _AllPossibleTEPAlternatives, value);
                AllAlternativesViewModel.TepAlternatives = AllPossibleTEPAlternatives;
            }
        }

        public ScenarioTepAlternativesEnumControlViewModel AllAlternativesViewModel { get; set; }

        public ICommand BuildParetoFrontier { get; private set; }
        public ICommand DgTepPareto_DoubleClick { get; private set; }

        public ScenarioTEPParetoViewModel()
        {
            BuildParetoFrontier = new DelegateCommand(RunBuildParetoFrontier);
            DgTepPareto_DoubleClick = new DelegateCommand(RunDgTepPareto_DoubleClick);
            AllAlternativesViewModel = new ScenarioTepAlternativesEnumControlViewModel();
            ParetoAlternativesViewModel = new ScenarioTepAlternativesEnumControlViewModel();
        }

        private void RunBuildParetoFrontier()
        {
            MyParetoBuilder.Solve();
            TepAlternativesInParetoFront = MyParetoBuilder.ParetoFront;
            AllPossibleTEPAlternatives = MyParetoBuilder.AllPossibleTEPAlternatives;
        }

        private void RunDgTepPareto_DoubleClick()
        {
            //Display window with detailed information of selected transmission expansion plan
            string caption = "Power system planning - MOO TEP under Scenarios";
            string messageBoxText = "Detailed inspection of a single Transmission Expansion Plan must currently be conducted manually (selecting transmission lines and solving the scenario LDC in the main window).";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;// Display message box
            MessageBox.Show(messageBoxText, caption, button, icon);
        }
    }
}
