using PowerSystemPlanning.BindingModels.BaseDataBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.BindingModels.StateCollectionDataTable
{
    public class StateCollectionDataTable : SerializableBindableBase
    {
        public const string NodeNameColumn = "Node Name";
        public const string ConsumptionColumn = "Consumption {0}";
        public const string GeneratingCapacityColumn = "Generating Capacity {0}";
        public const string MarginalCostColumn = "Marginal Cost {0}";

        public PowerSystem MyPowerSystem { get; protected set; }

        public BindingList<StateDefinition> MyStatesDefinition { get; protected set; }

        public DataTable DtNodesStates { get; protected set; }

        public StateCollectionDataTable() { }

        public StateCollectionDataTable(PowerSystem powerSystem, BindingList<StateDefinition> statesDefinition) : this()
        {
            MyPowerSystem = powerSystem;
            MyStatesDefinition = statesDefinition;
            DtNodesStates = CreateDefaultNodesDt();
            PopulateDefaultNodesDt();
            OnPropertyChanged(nameof(DtNodesStates));
        }

        #region Internal methods
        private DataTable CreateDefaultNodesDt()
        {
            var dt = new DataTable();
            //Add First column: name of the node
            var column = new DataColumn(NodeNameColumn, typeof(string));
            dt.Columns.Add(column);
            //Add other columns: load, Generating Capacity, Marginal Cost; under each state
            foreach (var state in MyStatesDefinition)
            {
                //load
                column = new DataColumn(String.Format(ConsumptionColumn, state), typeof(double));
                dt.Columns.Add(column);
            }
            foreach (var state in MyStatesDefinition)
            {
                //generating capacity
                column = new DataColumn(String.Format(GeneratingCapacityColumn, state), typeof(double));
                dt.Columns.Add(column);
                //marginal cost
                column = new DataColumn(String.Format(MarginalCostColumn, state), typeof(double));
                dt.Columns.Add(column);
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
            }
        }
        #endregion
    }
}
