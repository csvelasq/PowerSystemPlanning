using PowerSystemPlanning;
using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.PlanningModels.Planning;
using PowerSystemPlanning.Solvers.OPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.Model
{
    public class ScenarioTEPViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Example implementation from: https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ScenarioTEPModel _MyScenarioTEPModel;
        /// <summary>
        /// The TEP scenario model of the power system.
        /// </summary>
        public ScenarioTEPModel MyScenarioTEPModel
        {
            get
            {
                return _MyScenarioTEPModel;
            }
            set
            {
                if (_MyScenarioTEPModel != value)
                {
                    _MyScenarioTEPModel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        PowerSystemScenario _selectedScenarioForEditing;
        /// <summary>
        /// Currently selected (in GUI) scenario for editing purposes.
        /// </summary>
        public PowerSystemScenario selectedScenarioForEditing
        {
            get { return _selectedScenarioForEditing; }
            set
            {
                if (_selectedScenarioForEditing != value)
                {
                    _selectedScenarioForEditing = value;
                    NotifyPropertyChanged();
                }
            }
        }

        OPFModelResultForLDC _selectedLoadBlockInLDCOPFResults;
        /// <summary>
        /// Currently selected load block (in LDCOPF Tab) for inspecting detailed OPF results.
        /// </summary>
        public OPFModelResultForLDC selectedLoadBlockInLDCOPFResults
        {
            get { return _selectedLoadBlockInLDCOPFResults; }
            set
            {
                if (_selectedLoadBlockInLDCOPFResults != value)
                {
                    _selectedLoadBlockInLDCOPFResults = value;
                    NotifyPropertyChanged();
                    //myOPFResultsControl.OPFResultsForLDC = value;
                    //if (value != null)
                    //{
                    //    tabControlPowerSystems.SelectedIndex = 1;
                    //}
                }
            }
        }

        public ScenarioTEPViewModel()
        {
            //Default load duration curve
            LoadDurationCurveByBlocks myLoadDurationCurve = new LoadDurationCurveByBlocks();
            myLoadDurationCurve.DurationBlocks.Add(new LoadBlock(6000, 0.4));
            myLoadDurationCurve.DurationBlocks.Add(new LoadBlock(2000, 0.6));
            myLoadDurationCurve.DurationBlocks.Add(new LoadBlock(760, 1));
            //Default name and discount rate
            MyScenarioTEPModel = new ScenarioTEPModel("Unnamed power system model", 0.07, myLoadDurationCurve);
            //Adds an empty scenario
            MyScenarioTEPModel.MyScenarios.Add(new PowerSystemScenario("Unnamed scenario", new PowerSystem()));
        }

        public ScenarioTEPViewModel(ScenarioTEPModel myScenarioTEPModel)
        {
            MyScenarioTEPModel = myScenarioTEPModel;
        }
    }
}
