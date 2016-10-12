using NLog;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;
using PowerSystemPlanningWpfApp.ApplicationWide.ViewModels;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemPlanning.BindingModels.StateBinding;

namespace PowerSystemPlanningWpfApp.ApplicationWide.ViewModels
{
    public class ScenarioEditorViewModel : BaseDocumentOneFolderViewModel
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PowerSystem MyPowerSystem { get; set; }

        public BindableStaticScenarioCollection MyStaticScenarios
        {
            get { return _MyStaticScenarios; }
            set { SetProperty<BindableStaticScenarioCollection>(ref _MyStaticScenarios, value); }
        }

        #region Basic UI Properties
        public override string Title => "Scenarios";

        public BindableStaticScenario SelectedScenario
        {
            get { return _SelectedScenario; }
            set { base.SetProperty(ref _SelectedScenario, (PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios.BindableStaticScenario)value); }
        }

        public PowerSystemState SelectedState
        {
            get { return _SelectedState; }
            set { SetProperty<PowerSystemState>(ref _SelectedState, value); }
        }
        #endregion

        #region Internal fields
        BindableStaticScenarioCollection _MyStaticScenarios;
        BindableStaticScenario _SelectedScenario;
        PowerSystemState _SelectedState;
        #endregion

        #region Commands
        public DelegateCommand EditStatesCommand { get; private set; }
        public DelegateCommand CommitStatesCommand { get; private set; }

        private void EditStates()
        {
            MyStaticScenarios.CreateStateCollection();
        }

        private void CommitStates()
        {
            MyStaticScenarios.CommitStateCollectionToPowerSystemState();
        }
        #endregion

        #region Open&Save
        public string TepXmlStatesDefinitionAbsolutePath => Path.Combine(FolderAbsolutePath, "tep.xml");
        public string TepCsvStatesDataAbsolutePath => Path.Combine(FolderAbsolutePath, "states.csv");

        public override void SaveToFolder()
        {
            MyStaticScenarios.SaveToLocalFolderForEdition(TepXmlStatesDefinitionAbsolutePath, TepCsvStatesDataAbsolutePath);
            logger.Info($"Scenarios saved to '{FolderAbsolutePath}'.");
        }

        //public override void Open()
        //{
        //    /*
        //     * Open input data
        //     */
        //    MyTepStudy = BindingTepScenarios.Load(MyOwnerPowerSys.MyPowerSystem, TepXmlStatesDefinitionAbsolutePath, TepCsvStatesDataAbsolutePath);
        //    /*
        //     * Open results
        //     */
        //}
        #endregion
            
        public ScenarioEditorViewModel(PowerSystem system, BindableStaticScenarioCollection scenarios)
        {
            MyPowerSystem = system;
            MyStaticScenarios = scenarios;
            //Commands
            EditStatesCommand = new DelegateCommand(EditStates);
            CommitStatesCommand = new DelegateCommand(CommitStates);
        }
    }
}
