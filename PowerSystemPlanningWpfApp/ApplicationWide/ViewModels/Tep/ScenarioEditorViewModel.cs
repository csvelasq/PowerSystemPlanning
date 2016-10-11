using NLog;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios;
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

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    public class ScenarioEditorViewModel : BaseDocumentOneFolderViewModel
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public PowerSystem MyPowerSystem { get; set; }

        BindableStaticScenarioCollection _MyScenarios;
        public BindableStaticScenarioCollection MyScenarios
        {
            get { return _MyScenarios; }
            set { SetProperty<BindableStaticScenarioCollection>(ref _MyScenarios, value); }
        }

        #region Basic UI Properties
        public override string Title => "Scenarios";

        PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios.BindableStaticScenario _SelectedScenario;
        public BindableStaticScenario SelectedScenario
        {
            get { return _SelectedScenario; }
            set { base.SetProperty(ref _SelectedScenario, (PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios.BindableStaticScenario)value); }
        }

        PowerSystemState _SelectedState;
        public PowerSystemState SelectedState
        {
            get { return _SelectedState; }
            set { SetProperty<PowerSystemState>(ref _SelectedState, value); }
        }
        #endregion

        #region Commands
        public DelegateCommand EditStatesCommand { get; private set; }
        public DelegateCommand CommitStatesCommand { get; private set; }

        private void EditStates()
        {
            MyScenarios.CreateStateCollection();
        }

        private void CommitStates()
        {
            MyScenarios.CommitStateCollectionToPowerSystemState();
        }
        #endregion

        #region Open&Save
        public string TepXmlStatesDefinitionAbsolutePath => Path.Combine(FolderAbsolutePath, "tep.xml");
        public string TepCsvStatesDataAbsolutePath => Path.Combine(FolderAbsolutePath, "states.csv");

        public override void SaveToFolder()
        {
            MyScenarios.SaveToLocalFolderForEdition(TepXmlStatesDefinitionAbsolutePath, TepCsvStatesDataAbsolutePath);
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

        public ScenarioEditorViewModel(PowerSystem system)
        {
            MyPowerSystem = system;
            var defaultScenarios =
                BindableStaticScenarioCollection.CreateDefaultScenarios(MyPowerSystem);
            MyScenarios = new BindableStaticScenarioCollection(MyPowerSystem, defaultScenarios);

            //Commands
            EditStatesCommand = new DelegateCommand(EditStates);
            CommitStatesCommand = new DelegateCommand(CommitStates);
        }
    }
}
