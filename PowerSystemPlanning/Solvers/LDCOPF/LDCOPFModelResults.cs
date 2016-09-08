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
        PowerSystem PowerSystem;
        
        /// <summary>
        /// Summary of LDC OPF results for each node in the power system (total energy consumed, etc).
        /// </summary>
        public List<NodeLDCOPFResult> NodeLDCOPFResults { get; protected set; }

        /// <summary>
        /// The results of the OPF model in each block of the duration curve.
        /// </summary>
        public List<OPFModelResultForLDC> OpfResultsByBlock { get; protected set; }

        public LDCOPFModelResults(int status) : base(status) { }

        public LDCOPFModelResults(PowerSystem powerSystem, int status, double objVal, LoadDurationCurveByBlocks DurationCurveBlocks, List<OPFModelResultForLDC> opfResultsByBlock)
            : base(status, objVal)
        {
            this.PowerSystem = powerSystem;
            this.ObjVal = objVal;
            this.NodeLDCOPFResults = new List<NodeLDCOPFResult>();
            this.OpfResultsByBlock = opfResultsByBlock;
            List<double> BlockDurations = DurationCurveBlocks.BlockDurations;
            List<double> RelativeBlockDuration = DurationCurveBlocks.RelativeBlockDurations;
            foreach (Node node in this.PowerSystem.Nodes)
            {
                double TotalEnergyGenerated = 0;
                double TotalEnergyConsumed = 0;
                double LoadShedding = 0;
                double AverageSpotPrice = 0;
                for (int i = 0; i < opfResultsByBlock.Count; i++)
                {
                    NodeOPFResultForLDC nodeResultsInThisBlock = opfResultsByBlock[i].NodeOPFResultsForLDC[node.Id];
                    TotalEnergyGenerated += nodeResultsInThisBlock.TotalPowerGenerated * BlockDurations[i];
                    TotalEnergyConsumed += nodeResultsInThisBlock.TotalPowerConsumed * BlockDurations[i];
                    LoadShedding += nodeResultsInThisBlock.LoadShedding * BlockDurations[i];
                    AverageSpotPrice += nodeResultsInThisBlock.SpotPrice * RelativeBlockDuration[i];
                }
                NodeLDCOPFResult nodeResultsLDCOPF = new NodeLDCOPFResult(node, TotalEnergyGenerated, TotalEnergyConsumed, LoadShedding, AverageSpotPrice);
                NodeLDCOPFResults.Add(nodeResultsLDCOPF);
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

        /// <summary>
        /// Total energy generated during the whole year (in GWh, sum of energy generated under each block).
        /// </summary>
        /// <remarks>
        /// Energy generated in a particular block equals the dispatch of each generator (in MW) multiplied by the duration of the block (in hours).
        /// </remarks>
        public double TotalEnergyGenerated { get; protected set; }

        /// <summary>
        /// Total energy consumed during the whole year (in GWh, sum of energy consumed under each block).
        /// </summary>
        public double TotalEnergyConsumed { get; protected set; }

        /// <summary>
        /// Total load shedding (in GWh) in this node.
        /// </summary>
        public double LoadShedding { get; protected set; }

        /// <summary>
        /// Average spot price (in US$/MW) during the whole year.
        /// </summary>
        /// <remarks>Average of the spot price in each block, weighted by the duration (in hours/8760) of the block.</remarks>
        public double AverageSpotPrice { get; protected set; }

        public NodeLDCOPFResult(Node node, double totalEnergyGenerated, double totalEnergyConsumed, double loadShed, double averageSpotPrice)
        {
            this.Node = node;
            this.TotalEnergyGenerated = totalEnergyGenerated;
            this.TotalEnergyConsumed = totalEnergyConsumed;
            this.LoadShedding = loadShed;
            this.AverageSpotPrice = averageSpotPrice;
        }
    }
}
