using System.Collections.Generic;
using System.Linq;
using PowerSystemPlanning.Models.SystemState.Nodes;

namespace PowerSystemPlanning.Solvers.OPF.OpfResults
{
    /// <summary>
    /// Encapsulator of the OPF result of a node (bus angle and load shedding).
    /// </summary>
    public class NodeOPFResult : BaseOPFResultElement
    {
        public INodeState MyNodeState { get; protected set; }

        public double Angle { get; protected set; }

        public double SpotPrice { get; protected set; }

        /******************************************
         * GENERATION (PHYSICAL)
         ******************************************/
        public double TotalPowerGenerated =>
            (from gen in GlobalResultsOpfModelResults.MyGeneratingUnitOPFResults
             where gen.MyGeneratingUnitState.UnderlyingGeneratingUnit.ConnectionNode == MyNodeState.UnderlyingNode
             select gen.PowerOutput).Sum();
        public double TotalEnergyGenerated =>
            (from gen in GlobalResultsOpfModelResults.MyGeneratingUnitOPFResults
             where gen.MyGeneratingUnitState.UnderlyingGeneratingUnit.ConnectionNode == MyNodeState.UnderlyingNode
             select gen.EnergyOutput).Sum();

        /******************************************
         * GENERATION (COST)
         ******************************************/
        public double HourlyGenerationCost =>
            (from gen in GlobalResultsOpfModelResults.MyGeneratingUnitOPFResults
             where gen.MyGeneratingUnitState.UnderlyingGeneratingUnit.ConnectionNode == MyNodeState.UnderlyingNode
             select gen.HourlyGenerationCost).Sum();
        public double TotalGenerationCost =>
            (from gen in GlobalResultsOpfModelResults.MyGeneratingUnitOPFResults
             where gen.MyGeneratingUnitState.UnderlyingGeneratingUnit.ConnectionNode == MyNodeState.UnderlyingNode
             select gen.TotalGenerationCost).Sum();

        /******************************************
         * CONSUMPTION (PHYSICAL)
         ******************************************/
        public double TotalPowerConsumed =>
            (from load in GlobalResultsOpfModelResults.MyLoadOpfResults
             where load.MyLoadState.UnderlyingInelasticLoad.ConnectionNode == MyNodeState.UnderlyingNode
             select load.PowerConsumed).Sum();
        public double TotalEnergyConsumed =>
            (from load in GlobalResultsOpfModelResults.MyLoadOpfResults
             where load.MyLoadState.UnderlyingInelasticLoad.ConnectionNode == MyNodeState.UnderlyingNode
             select load.EnergyConsumed).Sum();

        /******************************************
         * CURTAILMENT (PHYSICAL)
         ******************************************/
        public double TotalPowerCurtailed =>
            (from load in GlobalResultsOpfModelResults.MyLoadOpfResults
             where load.MyLoadState.UnderlyingInelasticLoad.ConnectionNode == MyNodeState.UnderlyingNode
             select load.PowerCurtailed).Sum();
        public double TotalEnergyCurtailed =>
            (from load in GlobalResultsOpfModelResults.MyLoadOpfResults
             where load.MyLoadState.UnderlyingInelasticLoad.ConnectionNode == MyNodeState.UnderlyingNode
             select load.PowerCurtailed).Sum();

        /******************************************
         * CURTAILMENT (COST)
         ******************************************/
        public double HourlyCurtailmentCost =>
            (from load in GlobalResultsOpfModelResults.MyLoadOpfResults
             where load.MyLoadState.UnderlyingInelasticLoad.ConnectionNode == MyNodeState.UnderlyingNode
             select load.HourlyCurtailmentCost).Sum();
        public double TotalCurtailmentCost =>
            (from load in GlobalResultsOpfModelResults.MyLoadOpfResults
             where load.MyLoadState.UnderlyingInelasticLoad.ConnectionNode == MyNodeState.UnderlyingNode
             select load.TotalCurtailmentCost).Sum();

        public NodeOPFResult(OPFModelResult globalResult, INodeState node, double angle, double spotPrice)
            : base(globalResult)
        {
            MyNodeState = node;
            Angle = angle;
            SpotPrice = spotPrice;
        }
    }
}
