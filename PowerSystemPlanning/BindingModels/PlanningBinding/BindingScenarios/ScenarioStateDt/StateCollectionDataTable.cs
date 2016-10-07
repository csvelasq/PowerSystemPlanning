using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios
{
    public class StateCollectionDataTable : SerializableBindableBase
    {
        public const string NodeNameColumn = "Node Name"; //string
        public const string ConsumptionColumn = "Consumption '{0}.{1}'"; //double [MW]
        public const string GeneratingCapacityColumn = "Generating Capacity '{0}.{1}'"; //double [MW]
        public const string MarginalCostColumn = "Marginal Cost '{0}.{1}'"; //double [US$/MWh]

        public PowerSystem MyPowerSystem { get; protected set; }

        public BindingList<BindingScenario> MyScenariosAndStates { get; protected set; }

        public DataTable DtNodesStates { get; protected set; }

        public StateCollectionDataTable() { }

        public StateCollectionDataTable(PowerSystem powerSystem, BindingList<BindingScenario> scenariosAndStates) : this()
        {
            MyPowerSystem = powerSystem;
            MyScenariosAndStates = scenariosAndStates;
            InitializeStateCollectionDataTable();
        }

        #region Initialize DataTable
        /// <summary>
        /// Initializes <see cref="DtNodesStates"/> with the structure of <see cref="MyScenariosAndStates"/>.
        /// </summary>
        /// <remarks>
        /// Recall this method any time there is a change in <see cref="MyScenariosAndStates"/> in:
        ///     *number of scenarios / states
        ///     *names of scenarios / states
        /// </remarks>
        public void InitializeStateCollectionDataTable()
        {
            DtNodesStates = CreateDefaultRowsDt();
            PopulateDefaultNodesDt();
            OnPropertyChanged(nameof(DtNodesStates));
        }

        private DataTable CreateDefaultRowsDt()
        {
            var dt = new DataTable();
            //Add First column: name of the node
            var column = new DataColumn(NodeNameColumn, typeof(string));
            dt.Columns.Add(column);
            //Add other columns: load, Generating Capacity, Marginal Cost; under each scenario; under each state
            foreach (var scenario in MyScenariosAndStates)
            {
                foreach (var state in scenario.MyPowerSystemStates)
                {
                    //load
                    column = new DataColumn(String.Format(ConsumptionColumn, scenario, state), typeof(double));
                    dt.Columns.Add(column);
                }
                foreach (var state in scenario.MyPowerSystemStates)
                {
                    //generating capacity
                    column = new DataColumn(String.Format(GeneratingCapacityColumn, scenario, state), typeof(double));
                    dt.Columns.Add(column);
                    //marginal cost
                    column = new DataColumn(String.Format(MarginalCostColumn, scenario, state), typeof(double));
                    dt.Columns.Add(column);
                }
            }
            return dt;
        }

        private DataColumn GetDataColumnInNodesDt(string colGenericName, int state)
        {
            return DtNodesStates.Columns[String.Format(colGenericName, state)];
        }

        private void PopulateDefaultNodesDt()
        {
            foreach (var node in MyPowerSystem.Nodes)
            {
                var row = DtNodesStates.NewRow();
                row[NodeNameColumn] = node.Name;
                DtNodesStates.Rows.Add(row);
            }
        }
        #endregion

        /// <summary>
        /// Updates <see cref="MyScenariosAndStates"/> with the data provided by <see cref="DtNodesStates"/>
        /// </summary>
        public void CommitStateCollectionToPowerSystemState()
        {
            throw new NotImplementedException();
        }

        public void SaveDtToCsv(string csvAbsolutePath)
        {
            DtNodesStates.SaveToCsv(csvAbsolutePath);
        }

        public void LoadDtFromCsv(string csvAbsolutePath)
        {
            var csvParser = new GenericParsing.GenericParserAdapter(csvAbsolutePath);
            csvParser.ColumnDelimiter = ',';
            csvParser.FirstRowHasHeader = true;
            //csvParser.TextQualifier = '\"';
            var dt = csvParser.GetDataTable();
        }
    }
}
