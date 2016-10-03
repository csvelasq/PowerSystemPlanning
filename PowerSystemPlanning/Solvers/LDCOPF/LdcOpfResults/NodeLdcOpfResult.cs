using PowerSystemPlanning.Models.SystemBaseData.Nodes;
using PowerSystemPlanning.Models.SystemState.Nodes;
using PowerSystemPlanning.Solvers.OPF.OpfResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.LDCOPF.LdcOpfResults
{
    /// <summary>
    /// Encapsulator of the LDC OPF results of a node (bus angle and load shedding).
    /// </summary>
    public class NodeLdcOpfResult
    {
        public INode MyNode { get; protected set; }

        public List<NodeOPFResult> MyNodeOpfResultsByBlock { get; protected set; }

        public double TotalOperationCost => TotalGenerationCost + TotalCurtailmentCost;

        /******************************************
         * SPOT PRICES
         ******************************************/
        public IEnumerable<double> SpotPrices => from node in MyNodeOpfResultsByBlock select node.SpotPrice;
        public double AverageSpotPrice => SpotPrices.Average();
        public double MinimumSpotPrice => SpotPrices.Min();
        public double MaximumSpotPrice => SpotPrices.Max();

        /******************************************
         * GENERATION (PHYSICAL)
         ******************************************/
        public double TotalEnergyGenerated =>
            (from node in MyNodeOpfResultsByBlock
             select node.TotalEnergyGenerated).Sum();

        /******************************************
         * GENERATION (COST)
         ******************************************/
        public double TotalGenerationCost =>
            (from node in MyNodeOpfResultsByBlock
             select node.TotalGenerationCost).Sum();

        /******************************************
         * CONSUMPTION (PHYSICAL)
         ******************************************/
        public double TotalEnergyConsumed =>
            (from node in MyNodeOpfResultsByBlock
             select node.TotalEnergyConsumed).Sum();

        /******************************************
         * CURTAILMENT (PHYSICAL)
         ******************************************/
        public double TotalEnergyCurtailed =>
            (from node in MyNodeOpfResultsByBlock
             select node.TotalEnergyCurtailed).Sum();

        /******************************************
         * CURTAILMENT (COST)
         ******************************************/
        public double TotalCurtailmentCost =>
            (from node in MyNodeOpfResultsByBlock
             select node.TotalCurtailmentCost).Sum();

        public NodeLdcOpfResult(INode node, List<NodeOPFResult> opfResultsByBlock)
        {
            this.MyNode = node;
            this.MyNodeOpfResultsByBlock = opfResultsByBlock;
        }
    }
}
