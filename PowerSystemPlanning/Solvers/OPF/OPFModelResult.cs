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
        protected IPowerSystemState MyPowerSystemState;
        /// <summary>
        /// Gets the total operation cost (MMUS$, generation plus load shedding, the model's objective value).
        /// </summary>
        public double TotalOperationCost
        {
            get { return (TotalGenerationCost + TotalLoadSheddingCost); }
        }
        /// <summary>
        /// Gets the total generation (in MW) by all generators in the power system.
        /// </summary>
        public double HourlyGenerationPower
        {
            get
            {
                return (from genResults in MyGeneratingUnitOPFResults
                        select genResults.PowerOutput).Sum();
            }
        }
        /// <summary>
        /// Gets the total energy generated (in GWh).
        /// </summary>
        public double TotalEnergyGenerated
        {
            get
            {
                return HourlyGenerationPower * MyPowerSystemState.DurationHours / 1e3;
            }
        }
        /// <summary>
        /// Gets the total generation cost (in US$).
        /// </summary>
        /// <remarks> Equals the sum over all generators of output (MW) multiplied by marginal cost (US$/MW).</remarks>
        public double TotalGenerationCost
        {
            get
            {
                return (from genResults in this.MyGeneratingUnitOPFResults
                        select genResults.TotalGenerationCost).Sum();
            }
        }
        /// <summary>
        /// Gets the total load shedding in the system (in GWh).
        /// </summary>
        public double TotalLoadShedding
        {
            get
            {
                return HourlyLoadShedding * MyPowerSystemState.DurationHours / 1e3;
            }
        }
        /// <summary>
        /// Gets the hourly load shedding in the system (MW).
        /// </summary>
        public double HourlyLoadShedding
        {
            get
            {
                return (from node in this.MyNodeOPFResults
                        select node.LoadShedding).Sum();
            }
        }
        /// <summary>Gets the total load shedding cost (in US$).</summary>
        /// <remarks> Equals the total load-shedding (MW) multiplied by the system's load shedding cost (US$/MW).</remarks>
        public double TotalLoadSheddingCost
        {
            get
            {
                return (this.MyPowerSystemState.LoadSheddingCost * TotalLoadShedding);
            }
        }

        public List<GeneratingUnitOPFResult> MyGeneratingUnitOPFResults { get; protected set; }

        public List<NodeOPFResult> MyNodeOPFResults { get; protected set; }

        public List<TransmissionLineOPFResult> MyTransmissionLineOPFResults { get; protected set; }

        /// <summary>
        /// Initializes the result container with the given Gurobi status.
        /// </summary>
        /// <param name="status">The Gurobi status of the optimization</param>
        /// <remarks>This constructor can be used to find out if the model was correctly solved by means of the <see cref="IsModelSolved"/> property.</remarks>
        public OPFModelResult(int status) : base(status) { }

        public OPFModelResult(IPowerSystemState powerSystem,
            int status, double objVal,
            double[] pGen_Solution,
            double[] pFlow_Solution,
            double[] lShed_Solution,
            double[] busAng_Solution,
            double[] nodalSpotPrice)
            : base(status, objVal)
        {
            this.MyPowerSystemState = powerSystem;
            //Generating units
            this.MyGeneratingUnitOPFResults = new List<GeneratingUnitOPFResult>();
            foreach (GeneratingUnit gen in MyPowerSystemState.GeneratingUnits)
            {
                MyGeneratingUnitOPFResults.Add(
                    new GeneratingUnitOPFResult(MyPowerSystemState,
                    gen,
                    pGen_Solution[MyPowerSystemState.GeneratingUnits.IndexOf(gen)])
                    );
            }
            //Transmission lines
            this.MyTransmissionLineOPFResults = new List<TransmissionLineOPFResult>();
            foreach (TransmissionLine tl in this.MyPowerSystemState.TransmissionLines)
            {
                MyTransmissionLineOPFResults.Add(
                    new TransmissionLineOPFResult(tl,
                    pFlow_Solution[MyPowerSystemState.TransmissionLines.IndexOf(tl)])
                    );
            }
            //Nodes
            this.MyNodeOPFResults = new List<NodeOPFResult>();
            foreach (Node node in this.MyPowerSystemState.Nodes)
            {
                double pgen = 0;
                pgen = (from g in MyGeneratingUnitOPFResults
                        select g.PowerOutput).Sum();
                int b = MyPowerSystemState.Nodes.IndexOf(node);
                double pcons = 0;
                double lshed = 0;
                foreach (InelasticLoad load in node.InelasticLoads)
                {
                    int i = MyPowerSystemState.InelasticLoads.IndexOf(load);
                    pcons += load.ConsumptionMW - lShed_Solution[i];
                    lshed += lShed_Solution[i];
                }
                this.MyNodeOPFResults.Add(
                    new NodeOPFResult(node,
                    busAng_Solution[b],
                    pgen,
                    pcons,
                    lshed,
                    nodalSpotPrice[b]));
            }
        }
    }

    /// <summary>
    /// Encapsulator of the OPF result of a generating unit (output, in MW).
    /// </summary>
    public class GeneratingUnitOPFResult
    {
        protected IPowerSystemState MyPowerSystemState;

        public GeneratingUnit MyGeneratingUnit { get; protected set; }

        /// <summary>
        /// The power output (MW) of this generator in the current OPF solution.
        /// </summary>
        public double PowerOutput { get; protected set; }

        /// <summary>
        /// The total energy output (GWh) of this generator in the current OPF solution.
        /// </summary>
        public double EnergyOutput
        {
            get { return PowerOutput * MyPowerSystemState.DurationHours; }
        }

        /// <summary>
        /// The utilization of this generator (actual output / maximum output).
        /// </summary>
        public double Utilization
        {
            get { return PowerOutput / MyGeneratingUnit.InstalledCapacityMW; }
        }
        /// <summary>
        /// The hourly generation cost (US$/MWh) of this generator in the current solution.
        /// </summary>
        public double HourlyGenerationCost { get { return PowerOutput * MyGeneratingUnit.MarginalCost; } }

        /// <summary>
        /// Total generation costs (US$) of this generator.
        /// </summary>
        public double TotalGenerationCost
        {
            get { return HourlyGenerationCost * MyPowerSystemState.DurationHours; }
        }

        public GeneratingUnitOPFResult(IPowerSystemState powerSystem, GeneratingUnit generatingUnit, double output)
        {
            MyPowerSystemState = powerSystem;
            MyGeneratingUnit = generatingUnit;
            PowerOutput = output;
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

        /// <summary>
        /// Load shedding (MW)
        /// </summary>
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
        public TransmissionLine MyTransmissionLine { get; protected set; }

        /// <summary>
        /// Power flow (MW) through this branch (positive if going from source to sink node).
        /// </summary>
        public double PowerFlow { get; protected set; }

        /// <summary>
        /// The utilization of this transmission line (actual flow / maximum flow).
        /// </summary>
        public double Utilization
        {
            get { return Math.Abs(PowerFlow) / MyTransmissionLine.ThermalCapacityMW; }
        }

        public TransmissionLineOPFResult(TransmissionLine transmissionLine, double power_flow)
        {
            this.MyTransmissionLine = transmissionLine;
            this.PowerFlow = power_flow;
        }
    }
}
