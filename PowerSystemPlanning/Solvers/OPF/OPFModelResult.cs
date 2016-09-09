using Gurobi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.OPF
{
    /// <summary>
    /// Encapsulates results of the OPF model.
    /// </summary>
    public class OPFModelResult : BaseGRBOptimizationModelResult
    {
        protected PowerSystem PowerSystem;
        /// <summary>
        /// Gets the total operation cost (generation plus load shedding, the model's objective value).
        /// </summary>
        public virtual double TotalOperationCost
        {
            get
            {
                return (TotalGenerationCost + TotalLoadSheddingCost);
            }
        }
        /// <summary>
        /// Gets the total generation (in MW) by all generators in the power system.
        /// </summary>
        public virtual double TotalGeneration
        {
            get
            {
                return (from genResults in this.GeneratingUnitOPFResults select genResults.Output).Sum();
            }
        }
        /// <summary>
        /// Gets the total generation cost (in US$).
        /// </summary>
        /// <remarks> Equals the sum over all generators of output (MW) multiplied by marginal cost (US$/MW).</remarks>
        public virtual double TotalGenerationCost
        {
            get
            {
                return (from genResults in this.GeneratingUnitOPFResults select genResults.TotalGenerationCost).Sum();
            }
        }
        /// <summary>
        /// Geths the total load shedding in the system (in MW).
        /// </summary>
        public virtual double TotalLoadShedding
        {
            get
            {
                return (from node in this.NodeOPFResults select node.LoadShedding).Sum();
            }
        }
        /// <summary>Gets the total load shedding cost (in US$).</summary>
        /// <remarks> Equals the total load-shedding (MW) multiplied by the system's load shedding cost (US$/MW).</remarks>
        public virtual double TotalLoadSheddingCost
        {
            get
            {
                return (this.PowerSystem.LoadSheddingCost * TotalLoadShedding);
            }
        }

        public virtual List<GeneratingUnitOPFResult> GeneratingUnitOPFResults { get; protected set; }

        public virtual List<NodeOPFResult> NodeOPFResults { get; protected set; }

        public virtual List<TransmissionLineOPFResult> TransmissionLineOPFResults { get; protected set; }

        /// <summary>
        /// Initializes the result container with the given Gurobi status.
        /// </summary>
        /// <param name="status">The Gurobi status of the optimization</param>
        /// <remarks>This constructor can be used to find out if the model was correctly solved by means of the <see cref="IsModelSolved"/> property.</remarks>
        public OPFModelResult(int status) : base(status) { }

        public OPFModelResult(PowerSystem powerSystem, int status, double objVal, double[] pGen_Solution, double[] pFlow_Solution, double[] lShed_Solution, double[] busAng_Solution, double[] nodalSpotPrice)
            : base(status, objVal)
        {
            this.PowerSystem = powerSystem;
            //Generating units
            this.GeneratingUnitOPFResults = new List<GeneratingUnitOPFResult>();
            foreach (GeneratingUnit gen in this.PowerSystem.GeneratingUnits)
            {
                this.GeneratingUnitOPFResults.Add(new GeneratingUnitOPFResult(gen, pGen_Solution[gen.Id]));
            }
            //Transmission lines
            this.TransmissionLineOPFResults = new List<TransmissionLineOPFResult>();
            foreach (TransmissionLine tl in this.PowerSystem.TransmissionLines)
            {
                this.TransmissionLineOPFResults.Add(new TransmissionLineOPFResult(tl, pFlow_Solution[tl.Id]));
            }
            //Nodes
            this.NodeOPFResults = new List<NodeOPFResult>();
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
                this.NodeOPFResults.Add(new NodeOPFResult(node, busAng_Solution[node.Id], pgen, pcons, lshed, nodalSpotPrice[node.Id]));
            }
        }
    }

    /// <summary>
    /// Encapsulator of the OPF result of a generating unit (output, in MW).
    /// </summary>
    public class GeneratingUnitOPFResult
    {
        public GeneratingUnit GeneratingUnit { get; protected set; }

        /// <summary>
        /// The power output (MW) of this generator in the current OPF solution.
        /// </summary>
        public double Output { get; protected set; }

        public virtual double TotalGenerationCost { get { return this.Output * this.GeneratingUnit.MarginalCost; } }

        public GeneratingUnitOPFResult(GeneratingUnit generatingUnit, double output)
        {
            this.GeneratingUnit = generatingUnit;
            this.Output = output;
        }
    }

    /// <summary>
    /// Encapsulator of the OPF result of a node (bus angle and load shedding).
    /// </summary>
    public class NodeOPFResult
    {
        public Node Node { get; protected set; }

        public int NodeId { get { return this.Node.Id; } }

        public string NodeName { get { return this.Node.Name; } }

        public double Angle { get; protected set; }

        public double TotalPowerGenerated { get; protected set; }

        public double TotalPowerConsumed { get; protected set; }

        public double LoadShedding { get; protected set; }

        public double SpotPrice { get; protected set; }

        public NodeOPFResult(Node node, double angle, double totalPowerGenerated, double totalPowerConsumed, double loadShed, double spotPrice)
        {
            this.Node = node;
            this.Angle = angle;
            this.TotalPowerGenerated = totalPowerGenerated;
            this.TotalPowerConsumed = totalPowerConsumed;
            this.LoadShedding = loadShed;
            this.SpotPrice = spotPrice;
        }
    }

    /// <summary>
    /// Encapsulator of the OPF result of a transmission line (power flow in MW).
    /// </summary>
    public class TransmissionLineOPFResult
    {
        public TransmissionLine TransmissionLine { get; protected set; }

        public int Id { get { return this.TransmissionLine.Id; } }
        public int NodeFromId { get { return this.TransmissionLine.NodeFromID; } }
        public int NodeToId { get { return this.TransmissionLine.NodeToID; } }

        public double PowerFlow { get; protected set; }

        public double Utilization { get { return Math.Abs(PowerFlow) / TransmissionLine.ThermalCapacityMW; } }

        public TransmissionLineOPFResult(TransmissionLine transmissionLine, double power_flow)
        {
            this.TransmissionLine = transmissionLine;
            this.PowerFlow = power_flow;
        }
    }
}
