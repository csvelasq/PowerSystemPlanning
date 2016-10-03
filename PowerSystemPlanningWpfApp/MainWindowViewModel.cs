using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.Models.Planning.LDC;
using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using PowerSystemPlanning.Solvers.LDCOPF;
using PowerSystemPlanning.Solvers.OPF;
using PowerSystemPlanningWpfApp.Analysis.ScenarioTEP;
using PowerSystemPlanningWpfApp.Analysis.ScenarioTEP.BruteForcePareto;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerSystemPlanningWpfApp
{
    public class MainWindowViewModel : BindableBase
    {
        public ScenarioTEPModel _MyScenarioTEPModel;
        /// <summary>
        /// The TEP scenario model of the power system.
        /// </summary>
        public ScenarioTEPModel MyScenarioTEPModel
        {
            get { return _MyScenarioTEPModel; }
            set
            {
                SetProperty<ScenarioTEPModel>(ref _MyScenarioTEPModel, value);
                MyScenarioTepViewModel.MyScenarioTEPModel = MyScenarioTEPModel;
                MyScenarioTEPParetoViewModel.MyScenarioTEPModel = MyScenarioTEPModel;
            }
        }

        // TODO decouple view-models through Prism MVVM Mediator pattern
        ScenarioTepViewModel _MyScenarioTepViewModel;
        public ScenarioTepViewModel MyScenarioTepViewModel
        {
            get { return _MyScenarioTepViewModel; }
            set
            {
                SetProperty<ScenarioTepViewModel>(ref _MyScenarioTepViewModel, value);
            }
        }

        ScenarioTEPParetoViewModel _MyScenarioTEPParetoViewModel;
        public ScenarioTEPParetoViewModel MyScenarioTEPParetoViewModel
        {
            get { return _MyScenarioTEPParetoViewModel; }
            set
            {
                SetProperty<ScenarioTEPParetoViewModel>(ref _MyScenarioTEPParetoViewModel, value);
            }
        }

        public MainWindowViewModel() : this(new ScenarioTEPModel()) { }

        public MainWindowViewModel(ScenarioTEPModel myScenarioTEPModel)
        {
            MyScenarioTepViewModel = new ScenarioTepViewModel();
            MyScenarioTEPParetoViewModel = new ScenarioTEPParetoViewModel();
            MyScenarioTEPModel = myScenarioTEPModel;
        }

        public void OpenModelFile(string filename)
        {
            MyScenarioTEPModel = ScenarioTEPModel.readFromXMLFile(filename);
        }

        /// <summary>
        /// Creates a new scenario tep view model with default (arbitrary) parameters for the power system data model.
        /// </summary>
        /// <returns>A new (mostly empty) scenario TEP model.</returns>
        public static MainWindowViewModel CreateDefaultScenarioTEPModel()
        {
            MainWindowViewModel MyScenarioTEPViewModel = new MainWindowViewModel();
            ScenarioTEPModel MyScenarioTEPModel = MyScenarioTEPViewModel.MyScenarioTEPModel;
            //Default load duration curve
            LoadDurationCurveByBlocks defaultLoadDurationCurve = new LoadDurationCurveByBlocks();
            defaultLoadDurationCurve.DurationBlocks.Add(new LoadBlock(6000, 0.4));
            defaultLoadDurationCurve.DurationBlocks.Add(new LoadBlock(2000, 0.6));
            defaultLoadDurationCurve.DurationBlocks.Add(new LoadBlock(760, 1));
            //Default name and discount rate
            MyScenarioTEPModel.Name = "Unnamed power system model";
            MyScenarioTEPModel.YearlyDiscountRate = 0.07;
            MyScenarioTEPModel.MyLoadDurationCurve = defaultLoadDurationCurve;
            MyScenarioTEPModel.TargetPlanningYear = 10;
            MyScenarioTEPModel.YearsWithOperation = 10;
            //Adds an empty scenario
            MyScenarioTEPModel.MyScenarios.Add(new PowerSystemScenario("Unnamed scenario", new PowerSystem()));
            return MyScenarioTEPViewModel;
        }
    }
}
