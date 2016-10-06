using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PowerSystemPlanningWpfApp.Analysis.ScenarioTEP.Enumerate
{
    /// <summary>
    /// View-model for a datagrid displaying multiple tep alternatives
    /// </summary>
    /// <remarks>
    /// To use, set the property <see cref="TepAlternatives"/> and the viewmodel/view will automatically
    /// generate and bind columns for the operation costs under each scenario.
    /// </remarks>
    public class ScenarioTepAlternativesEnumControlViewModel : BindableBase
    {
        public ScenarioTEPModel _MyScenarioTEPModel;
        public ScenarioTEPModel MyScenarioTEPModel
        {
            get { return _MyScenarioTEPModel; }
            set
            {
                SetProperty<ScenarioTEPModel>(ref _MyScenarioTEPModel, value);
            }
        }

        IEnumerable<TransmissionExpansionPlan> _TepAlternatives;
        public IEnumerable<TransmissionExpansionPlan> TepAlternatives
        {
            get { return _TepAlternatives; }
            set
            {
                SetProperty<IEnumerable<TransmissionExpansionPlan>>(ref _TepAlternatives, value);
                SetDgColumns();
            }
        }

        private ObservableCollection<DataGridColumn> _columnCollection = new ObservableCollection<DataGridColumn>();
        public ObservableCollection<DataGridColumn> MyColumnCollection
        {
            get { return _columnCollection; }
            set
            {
                SetProperty<ObservableCollection<DataGridColumn>>(ref _columnCollection, value);
            }
        }

        public ICommand DgTepEnum_DoubleClick { get; private set; }

        public ScenarioTepAlternativesEnumControlViewModel()
        {
            DgTepEnum_DoubleClick = new DelegateCommand(InspectTepAlternativeDetails);
            AddCommonDataColumns(MyColumnCollection);
        }

        private void InspectTepAlternativeDetails()
        {
            //Display window with detailed information of selected transmission expansion plan
            string caption = "Power system planning - MOO TEP under Scenarios";
            string messageBoxText = "Detailed inspection of a single Transmission Expansion Plan must currently be conducted manually (selecting transmission lines and solving the scenario LDC in the main window).";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;// Display message box
            MessageBox.Show(messageBoxText, caption, button, icon);
        }

        private void AddCommonDataColumns(ObservableCollection<DataGridColumn> columns)
        {
            columns.Add(ControlUtils.DataGridColumnsBehavior.CreateNewColumn_WithMyStyle("New Transmission Lines Count", "BuiltTransmissionLines.Count", "", 110));
            columns.Add(ControlUtils.DataGridColumnsBehavior.CreateNewColumn_WithMyStyle("Investment Cost (MUS$)", "TotalInvestmentCost", "C1", 90));
            columns.Add(ControlUtils.DataGridColumnsBehavior.CreateNewColumn_WithMyStyle("Transmission lines", "BuiltTransmissionLinesNames", "", 90));
            columns.Add(ControlUtils.DataGridColumnsBehavior.CreateNewColumn_WithMyStyle("Expected costs (MMUS$)", "ExpectedTotalCosts", "C", 90));
        }

        private void SetDgColumns()
        {
            if (TepAlternatives != null && TepAlternatives.Count() > 0)
            {
                ObservableCollection<DataGridColumn> colList = new ObservableCollection<DataGridColumn>();
                //Add common columns
                /*
                <DataGridTextColumn Header="New Transmission Lines Count" Binding="{Binding BuiltTransmissionLines.Count}" Width="110" />
                <DataGridTextColumn Header="Investment Cost (MUS$)" Binding="{Binding TotalInvestmentCost}" Width="90" />
                <DataGridTextColumn Header="Transmission lines" Binding="{Binding BuiltTransmissionLinesNames}" Width="90" />
                <DataGridTextColumn Header="Expected costs (MMUS$)" Binding="{Binding ExpectedTotalCosts, StringFormat=C}" Width="90" ElementStyle="{StaticResource ResourceKey=CellRightAlign}" />
                */
                AddCommonDataColumns(colList);
                //Add one column for each scenario
                var objectiveFunctions = TepAlternatives.First().MyProblem.MyObjectiveFunctionsDefinition;
                for (int i = 0; i < objectiveFunctions.Count; i++)
                {
                    colList.Add(ControlUtils.DataGridColumnsBehavior.CreateNewColumn_WithMyStyle
                        (objectiveFunctions[i].MyObjectiveName, String.Format("ObjectiveValues[{0}]", i), "C", 110));
                }
                //Set column collection in order to update the view
                MyColumnCollection = colList;
            }
        }
    }
}
