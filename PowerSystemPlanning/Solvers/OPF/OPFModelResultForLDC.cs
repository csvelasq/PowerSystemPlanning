using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.Solvers.LDCOPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.OPF
{
    /// <summary>
    /// Encapsulates results of the OPF model for LDC representation of demand.
    /// </summary>
    public class OPFModelResultForLDC : OPFModelResult
    {
        public LoadBlock LoadBlock { get; protected set; }

        //TODO: avoiding inheritance is beginning to smell...
        public List<NodeOPFResultForLDC> NodeOPFResultsForLDC { get; protected set; }
        public List<GeneratingUnitOPFResultForLDC> GeneratingUnitOPFResultsForLDC { get; protected set; }

        /// <summary>
        /// Gets the total energy generated in the system (GWh).
        /// </summary>
        /// <remarks>Equals the sum over all generators of output (MW), multiplied by the block duration (hours).</remarks>
        public double TotalGenerationEnergy
        {
            get
            {
                return (LoadBlock.Duration * (from genResults in this.GeneratingUnitOPFResults select genResults.Output).Sum()) / 1000.0;
            }
        }
        /// <summary>
        /// Gets the total generation cost (in US$) for suplying demand during the whole block's duration.
        /// </summary>
        /// <remarks> Equals the sum over all generators of output (MW) multiplied by marginal cost (US$/MW), multiplied by the block duration (hours).</remarks>
        public override double TotalGenerationCost
        {
            get
            {
                return (LoadBlock.Duration * (from genResults in this.GeneratingUnitOPFResults select genResults.TotalGenerationCost).Sum());
            }
        }
        /// <summary>
        /// Gets the total hourly generation cost (in US$/h) for supplying demand.
        /// </summary>
        public double TotalHourlyGenerationCost
        {
            get
            {
                return (from genResults in GeneratingUnitOPFResultsForLDC select genResults.HourlyGenerationCost).Sum();
            }
        }
        /// <summary>
        /// Gets the total energy shed in the system (GWh).
        /// </summary>
        /// <remarks>Equals the total load-shedding (MW), multiplied by the block duration (hours).</remarks>
        public double TotalLoadSheddingEnergy
        {
            get
            {
                return (LoadBlock.Duration * (from node in this.NodeOPFResults select node.LoadShedding).Sum()) / 1000.0;
            }
        }
        /// <summary>
        /// Gets the total load shedding cost (in US$).
        /// </summary>
        /// <remarks>Equals the total load-shedding (MW) multiplied by the system's load shedding cost (US$/MW), multiplied by the block duration (hours).</remarks>
        public override double TotalLoadSheddingCost
        {
            get
            {
                return (LoadBlock.Duration * this.PowerSystem.LoadSheddingCost * TotalLoadShedding);
            }
        }

        /// <summary>
        /// The average spot price across all nodes of the system.
        /// </summary>
        public double AverageNodalSpotPrice
        {
            get
            {
                //TODO more efficient summary of spot prices
                return (from nodeResults in NodeOPFResultsForLDC select nodeResults.SpotPrice).Average();
            }
        }

        /// <summary>
        /// The maximum spot price across all nodes of the system.
        /// </summary>
        public double MaximumNodalSpotPrice
        {
            get
            {
                return (from nodeResults in NodeOPFResultsForLDC select nodeResults.SpotPrice).Max();
            }
        }

        /// <summary>
        /// The minimum spot price across all nodes of the system.
        /// </summary>
        public double MinimumNodalSpotPrice
        {
            get
            {
                return (from nodeResults in NodeOPFResultsForLDC select nodeResults.SpotPrice).Min();
            }
        }

        /// <summary>
        /// Initializes the result container with the given Gurobi status.
        /// </summary>
        /// <param name="status">The Gurobi status of the optimization</param>
        /// <remarks>This constructor can be used to find out if the model was correctly solved by means of the <see cref="IsModelSolved"/> property.</remarks>
        public OPFModelResultForLDC(int status) : base(status) { }

        public OPFModelResultForLDC(PowerSystem powerSystem, int status, double objVal, double[] pGen_Solution, double[] pFlow_Solution, double[] lShed_Solution, double[] busAng_Solution, double[] nodalSpotPrice, LoadBlock loadBlock)
            : base(powerSystem, status, objVal, pGen_Solution, pFlow_Solution, lShed_Solution, busAng_Solution, nodalSpotPrice)
        {
            this.LoadBlock = loadBlock;
            //Generating units results
            this.GeneratingUnitOPFResultsForLDC = new List<GeneratingUnitOPFResultForLDC>();
            foreach (GeneratingUnit gen in this.PowerSystem.GeneratingUnits)
            {
                this.GeneratingUnitOPFResultsForLDC.Add(new GeneratingUnitOPFResultForLDC(gen, pGen_Solution[gen.Id], this.LoadBlock));
            }
            //Node results
            this.NodeOPFResultsForLDC = new List<NodeOPFResultForLDC>();
            foreach (Node node in this.PowerSystem.Nodes)
            {
                double pgen = 0;
                foreach (GeneratingUnit gen in node.GeneratingUnits)
                {
                    pgen += this.GeneratingUnitOPFResults[gen.Id].Output;
                }
                double pcons = 0;
                double lshed = 0;
                foreach (InelasticLoad load in node.InelasticLoads)
                {
                    pcons += load.ConsumptionMW - lShed_Solution[load.Id];
                    lshed += lShed_Solution[load.Id];
                }
                this.NodeOPFResultsForLDC.Add(new NodeOPFResultForLDC(node, busAng_Solution[node.Id], pgen, pcons, lshed, nodalSpotPrice[node.Id] / loadBlock.Duration, loadBlock));
                //nodal spot price is the dual variable of power balance constraint, divided by the duration of the block
            }
        }
    }

    public class NodeOPFResultForLDC : NodeOPFResult
    {
        public LoadBlock MyLoadBlock { get; protected set; }

        public double TotalEnergyGenerated { get { return TotalPowerGenerated * MyLoadBlock.Duration / 1000.0; } }

        public double TotalEnergyConsumed { get { return TotalPowerConsumed * MyLoadBlock.Duration * MyLoadBlock.LoadMultiplier / 1000.0; } }

        public double EnergyLoadShedding { get { return LoadShedding * MyLoadBlock.Duration * MyLoadBlock.LoadMultiplier / 1000.0; } }

        public NodeOPFResultForLDC(Node node, double angle, double totalPowerGenerated, double totalPowerConsumed, double loadShed, double spotPrice,
            LoadBlock loadBlock)
            : base(node, angle, totalPowerGenerated, totalPowerConsumed, loadShed, spotPrice)
        {
            this.MyLoadBlock = loadBlock;
        }
    }

    public class GeneratingUnitOPFResultForLDC : GeneratingUnitOPFResult
    {
        public LoadBlock LoadBlock { get; protected set; }
        public double HourlyGenerationCost { get { return this.TotalGenerationCost / LoadBlock.Duration; } }
        public double EnergyGenerated { get { return this.Output * LoadBlock.Duration / 1000.0; } }
        public override double TotalGenerationCost { get { return Output * GeneratingUnit.MarginalCost * LoadBlock.Duration; } }

        public GeneratingUnitOPFResultForLDC(GeneratingUnit generatingUnit, double output, LoadBlock loadBlock)
            : base(generatingUnit, output)
        {
            this.LoadBlock = loadBlock;
        }
    }
}