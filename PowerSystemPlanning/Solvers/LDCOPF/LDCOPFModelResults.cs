using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.Solvers.OPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.LDCOPF
{
    /// <summary>
    /// Encapsulator of the results of the LDC OPF.
    /// </summary>
    public class LDCOPFModelResults : BaseGRBOptimizationModelResult
    {
        protected PowerSystem MyPowerSystem;

        /// <summary>
        /// Summary of LDC OPF results for each node in the power system (total energy consumed, etc).
        /// </summary>
        public List<NodeLDCOPFResult> MyNodeLDCOPFResults { get; protected set; }

        /// <summary>
        /// The results of the OPF model in each block of the duration curve.
        /// </summary>
        public List<OPFModelResultForLDC> MyOpfResultsByBlock { get; protected set; }

        public double TotalOperationCost
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.TotalOperationCost).Sum();
            }
        }
        public double TotalGenerationCost
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.TotalGenerationCost).Sum();
            }
        }
        public double TotalEnergyGenerated
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.TotalGenerationEnergy).Sum();
            }
        }
        public double TotalLoadSheddingCost
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.TotalLoadSheddingCost).Sum();
            }
        }
        public double TotalEnergyLoadShed
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.TotalLoadSheddingEnergy).Sum();
            }
        }
        public double AverageNodalSpotPrice
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.AverageNodalSpotPrice * opfBlockResult.MyLoadBlock.Duration
                        ).Sum() / 8760.0;
            }
        }
        public double PeakNodalSpotPrice
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.MaximumNodalSpotPrice).Max();
            }
        }
        
        public LDCOPFModelResults(int status) : base(status) { }

        public LDCOPFModelResults(PowerSystem powerSystem, int status, double objVal, LoadDurationCurveByBlocks durationCurveBlocks, List<OPFModelResultForLDC> opfResultsByBlock)
            : base(status, objVal)
        {
            this.MyPowerSystem = powerSystem;
            this.ObjVal = objVal;
            this.MyOpfResultsByBlock = opfResultsByBlock;
            //Build results by node
            this.MyNodeLDCOPFResults = new List<NodeLDCOPFResult>();
            foreach (Node node in this.MyPowerSystem.Nodes)
            {
                NodeLDCOPFResult nodeResultsLDCOPF = new NodeLDCOPFResult(node, opfResultsByBlock);
                MyNodeLDCOPFResults.Add(nodeResultsLDCOPF);
            }
        }
    }

    /// <summary>
    /// Encapsulator of the LDC OPF results of a node (bus angle and load shedding).
    /// </summary>
    public class NodeLDCOPFResult
    {
        public Node Node { get; protected set; }

        public int NodeId { get { return this.Node.Id; } }

        public string NodeName { get { return this.Node.Name; } }

        private List<OPFModelResultForLDC> MyOpfResultsByBlock { get; set; }

        /// <summary>
        /// Total energy generated in this node during the whole year (GWh, sum of energy generated in each block).
        /// </summary>
        /// <remarks>
        /// Energy generated in a particular block equals the dispatch of each generator (in MW) multiplied by the duration of the block (in hours).
        /// </remarks>
        public double TotalEnergyGenerated
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.NodeOPFResultsForLDC[NodeId].TotalEnergyGenerated).Sum();
            }
        }
        /// <summary>
        /// Total energy consumed in this node during the whole year (GWh, sum of energy consumed in each block).
        /// </summary>
        public double TotalEnergyConsumed
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.NodeOPFResultsForLDC[NodeId].TotalEnergyConsumed).Sum();
            }
        }
        /// <summary>
        /// Total load shedding (GWh) in this node.
        /// </summary>
        public double EnergyLoadShedding
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.NodeOPFResultsForLDC[NodeId].EnergyLoadShedding).Sum();
            }
        }

        /// <summary>
        /// Average spot price in this node (US$/MWh) during the whole year (across all load blocks).
        /// </summary>
        /// <remarks>Average of the spot price in each block, weighted by the duration (in hours/8760) of the block.</remarks>
        public double AverageNodalSpotPrice
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.NodeOPFResultsForLDC[NodeId].SpotPrice * opfBlockResult.MyLoadBlock.Duration).Sum() / 8760.0;
            }
        }

        /// <summary>
        /// Peak spot price in this node (US$/MWh) during the whole year (across all load blocks).
        /// </summary>
        public double PeakNodalSpotPrice
        {
            get
            {
                return (from opfBlockResult in MyOpfResultsByBlock
                        select opfBlockResult.NodeOPFResultsForLDC[NodeId].SpotPrice).Max();
            }
        }

        public NodeLDCOPFResult(Node node, List<OPFModelResultForLDC> opfResultsByBlock)
        {
            this.Node = node;
            this.MyOpfResultsByBlock = opfResultsByBlock;
        }
    }
}
