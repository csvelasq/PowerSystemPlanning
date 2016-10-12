using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;
using PowerSystemPlanning.Models.Planning.Scenarios;
using PowerSystemPlanning.Models.SystemState;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios
{
    public class ScenarioAndStateDataTableEditor : SerializableBindableBase
    {
        public const string NodeNameColumn = "Node Name"; //string
        public const string ConsumptionColumn = "Consumption '{0}'-'{1}'"; //double [MW]
        public const string GeneratingCapacityColumn = "Generating Capacity '{0}'-'{1}'"; //double [MW]
        public const string MarginalCostColumn = "Marginal Cost '{0}'-'{1}'"; //double [US$/MWh]

        public PowerSystem MyPowerSystem { get; protected set; }

        public BindingList<BindableStaticScenario> MyScenariosAndStates { get; protected set; }

        public DataTable DtNodesStates { get; protected set; }

        public ScenarioAndStateDataTableEditor() { }

        public ScenarioAndStateDataTableEditor(PowerSystem powerSystem, BindingList<BindingScenarios.BindableStaticScenario> scenariosAndStates) : this()
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
                foreach (var state in scenario.MyStateCollection.MyPowerSystemStates)
                {
                    //load
                    column = new DataColumn(String.Format(ConsumptionColumn, scenario, state), typeof(double));
                    dt.Columns.Add(column);
                }
                foreach (var state in scenario.MyStateCollection.MyPowerSystemStates)
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

        private DataColumn GetDataColumnInNodesDt(string colGenericName, IStaticScenario scenario, IPowerSystemState state)
        {
            return DtNodesStates.Columns[String.Format(colGenericName, scenario.Name, state.Name)];
        }

        private void PopulateDefaultNodesDt()
        {
            foreach (var node in MyPowerSystem.Nodes)
            {
                var row = DtNodesStates.NewRow();
                row[NodeNameColumn] = node.Name;
                //Add default consumption, generation capacity and marginal cost
                foreach (var scenario in MyScenariosAndStates)
                {
                    foreach (var state in scenario.MyStateCollection.MyPowerSystemStates)
                    {
                        var nodeState = state.NodeStates.First(x => x.UnderlyingNode == node);
                        //Consumption
                        if (nodeState.InelasticLoadsStates.Count == 1)
                            row[String.Format(ConsumptionColumn, scenario, state)] =
                                nodeState.InelasticLoadsStates[0].Consumption;
                        else
                            row[String.Format(ConsumptionColumn, scenario, state)] = 0;
                        //Generation capacity and marginal cost
                        if (nodeState.GeneratingUnitsStates.Count == 1)
                        {
                            row[String.Format(GeneratingCapacityColumn, scenario, state)] = 
                                nodeState.GeneratingUnitsStates[0].AvailableCapacity;
                            row[String.Format(MarginalCostColumn, scenario, state)] =
                                nodeState.GeneratingUnitsStates[0].MarginalCost;
                        }
                        else
                        {
                            row[String.Format(GeneratingCapacityColumn, scenario, state)] = 0;
                            row[String.Format(MarginalCostColumn, scenario, state)] = 0;
                        }
                    }
                }
                //Add the new row
                DtNodesStates.Rows.Add(row);
            }
        }
        #endregion

        /// <summary>
        /// Updates <see cref="MyScenariosAndStates"/> with the data provided by <see cref="DtNodesStates"/>
        /// </summary>
        public void CommitStateCollectionToPowerSystemState()
        {
            foreach (DataRow row in DtNodesStates.Rows)
            {
                foreach (var scenario in MyScenariosAndStates)
                {
                    foreach (var state in scenario.MyStateCollection.MyPowerSystemStates)
                    {
                        var node = state.NodeStates.First(x => x.UnderlyingNode.Name == (string)row[NodeNameColumn]);
                        //load
                        if (node.InelasticLoadsStates.Count > 0)
                        {
                            var load = node.InelasticLoadsStates.First();
                            load.Consumption = (double)row[String.Format(ConsumptionColumn, scenario, state)];
                        }
                        //generating capacity
                        if (node.GeneratingUnitsStates.Count > 0)
                        {
                            var gen = node.GeneratingUnitsStates.First();
                            gen.IsAvailable = true;
                            gen.AvailableCapacity = (double)row[String.Format(GeneratingCapacityColumn, scenario, state)];
                            gen.MarginalCost = (double)row[String.Format(MarginalCostColumn, scenario, state)];
                        }
                    }
                }
            }
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
