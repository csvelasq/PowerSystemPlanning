using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;
using PowerSystemPlanning.BindingModels.StateCollectionDataTable;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    public class ScenarioEditorViewModel : StudyViewModel
    {
        public override string Title => "Scenarios";

        #region Basic UI Properties
        BindingList<BindingScenario> _MyScenarios;
        public BindingList<BindingScenario> MyScenarios
        {
            get { return _MyScenarios; }
            set { SetProperty<BindingList<BindingScenario>>(ref _MyScenarios, value); }
        }

        BindingScenario _SelectedScenario;
        public BindingScenario SelectedScenario
        {
            get { return _SelectedScenario; }
            set { SetProperty<BindingScenario>(ref _SelectedScenario, value); }
        }

        StateCollectionDataTable _dtStateData;
        public StateCollectionDataTable dtStateData
        {
            get { return _dtStateData; }
            set { SetProperty<StateCollectionDataTable>(ref _dtStateData, value); }
        }
        #endregion

        #region Commands
        public DelegateCommand EditStatesCommand { get; private set; }

        private void EditStates()
        {
            dtStateData = new StateCollectionDataTable(MyPowerSystem, MyScenarios);
        }
        #endregion

        public ScenarioEditorViewModel()
        {
            MyScenarios = new BindingList<BindingScenario>();
            EditStatesCommand = new DelegateCommand(EditStates);
        }
    }
}
