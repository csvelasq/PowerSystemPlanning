using PowerSystemPlanning;
using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.PlanningModels.Planning;
using PowerSystemPlanning.Solvers.LDCOPF;
using PowerSystemPlanning.Solvers.OPF;
using PowerSystemPlanningWpfApp.Analysis.ScenarioTEP;
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
        /*
        public event PropertyChangedEventHandler PropertyChanged;

        // Example implementation from: https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        */

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
                MyScenarioTepViewModel.MyScenarioTEPModel = _MyScenarioTEPModel;
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
        /// <summary>
        /// Initializes a model with default (arbitrary) parameters
        /// </summary>
        public MainWindowViewModel() : this(new ScenarioTEPModel()) { }

        public MainWindowViewModel(ScenarioTEPModel myScenarioTEPModel)
        {
            MyScenarioTepViewModel = new ScenarioTepViewModel();
            MyScenarioTEPModel = myScenarioTEPModel;
        }

        public void OpenModelFile(string filename)
        {
            MyScenarioTEPModel = ScenarioTEPModel.readFromXMLFile(filename);
        }

        /// <summary>
        /// Creates a new scenario tep view model with default (arbitrary) parameters for the power system data model.
        /// </summary>
        /// <returns></returns>
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
            //Adds an empty scenario
            MyScenarioTEPModel.MyScenarios.Add(new PowerSystemScenario("Unnamed scenario", new PowerSystem()));
            return MyScenarioTEPViewModel;
        }
    }
}
