using PowerSystemPlanning.Models.SystemBaseData.Nodes;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Models.SystemState.Nodes;
using PowerSystemPlanning.Solvers.GRBOptimization;
using PowerSystemPlanning.Solvers.OPF.OpfResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.LDCOPF.LdcOpfResults
{
    public class LdcOpfModelResults : BaseGRBOptimizationModelResult
    {
        /// <summary>
        /// The results of the OPF model in each block of the duration curve.
        /// </summary>
        public List<OpfModelResult> MyOpfResultsByBlock { get; protected set; }

        public List<NodeLdcOpfResult> MyDetailedNodeLdcOpfResults { get; protected set; }

        public double TotalOperationCost =>
            (from opfBlockResult in MyOpfResultsByBlock
             select opfBlockResult.TotalOperationCost).Sum();
        public double TotalGenerationCost =>
            (from opfBlockResult in MyOpfResultsByBlock
             select opfBlockResult.TotalGenerationCost).Sum();
        public double TotalCurtailmentCost =>
            (from opfBlockResult in MyOpfResultsByBlock
             select opfBlockResult.TotalCurtailmentCost).Sum();

        public double TotalEnergyGenerated =>
            (from opfBlockResult in MyOpfResultsByBlock
             select opfBlockResult.TotalEnergyGenerated).Sum();

        public double TotalEnergyConsumed =>
            (from opfBlockResult in MyOpfResultsByBlock
             select opfBlockResult.TotalEnergyConsumed).Sum();
        public double TotalEnergyCurtailed =>
            (from opfBlockResult in MyOpfResultsByBlock
             select opfBlockResult.TotalEnergyCurtailed).Sum();

        public double AverageNodalSpotPrice =>
            (from opfBlockResult in MyOpfResultsByBlock
             select opfBlockResult.AvgSpotPrice * opfBlockResult.MyPowerSystemState.Duration)
            .Sum() / 8760.0;
        public double PeakNodalSpotPrice =>
            (from opfBlockResult in MyOpfResultsByBlock
             select opfBlockResult.MaxSpotPrice).Max();

        public LdcOpfModelResults(int status) : base(status) { }

        public LdcOpfModelResults(int status, double objVal, List<OpfModelResult> opfResultsByBlock, IList<INode> nodes)
            : base(status, objVal)
        {
            this.MyOpfResultsByBlock = opfResultsByBlock;
            //Build results by node
            this.MyDetailedNodeLdcOpfResults = new List<NodeLdcOpfResult>();
            foreach (var node in nodes)
            {
                var nodeResultsOpf = new List<NodeOPFResult>();
                //adds the NodeOPFResult for nodestate "node", foreach Opf Block
                foreach (var opfResult in MyOpfResultsByBlock)
                {
                    nodeResultsOpf.Add(opfResult.MyNodeOPFResults.Find(x => x.MyNodeState.UnderlyingNode == node));
                }
                //creates the detailed node LDC OPF results encapsulator
                var nodeResultsLDCOPF = new NodeLdcOpfResult(node, new List<NodeOPFResult>(nodeResultsOpf));
                MyDetailedNodeLdcOpfResults.Add(nodeResultsLDCOPF);
            }
        }
    }
}
