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
        PowerSystem PowerSystem;
        List<GeneratingUnitOPFResult> _GeneratingUnitOPFResults;
        List<NodeOPFResult> _NodeOPFResults;
        List<TransmissionLineOPFResult> _TransmissionLineOPFResults;

        /// <summary>
        /// Gets the total generation cost (the model's objective value) in the current solution.
        /// </summary>
        public double TotalOperationCost
        {
            get
            {
                return this._ObjVal;
            }
        }

        public List<GeneratingUnitOPFResult> GeneratingUnitOPFResults
        {
            get
            {
                return _GeneratingUnitOPFResults;
            }
        }

        public List<NodeOPFResult> NodeOPFResults
        {
            get
            {
                return _NodeOPFResults;
            }
            private set
            {
                _NodeOPFResults = value;
            }
        }

        public List<TransmissionLineOPFResult> TransmissionLineOPFResults
        {
            get
            {
                return _TransmissionLineOPFResults;
            }
            private set
            {
                _TransmissionLineOPFResults = value;
            }
        }
        
        /// <summary>
        /// Initializes the result container with the given Gurobi status.
        /// </summary>
        /// <param name="status">The Gurobi status of the optimization</param>
        /// <remarks>This constructor can be used to find out if the model was correctly solved by means of the <see cref="IsModelSolved"/> property.</remarks>
        public OPFModelResult(int status) : base(status) { }

        public OPFModelResult(PowerSystem powerSystem, int status, double totalOperationCost, double[] pGen_Solution, double[] pFlow_Solution, double[] lShed_Solution, double[] busAng_Solution, double[] nodalSpotPrice)
            : base(status, totalOperationCost)
        {
            this.PowerSystem = powerSystem;
            //Generating units
            this._GeneratingUnitOPFResults = new List<GeneratingUnitOPFResult>();
            foreach (GeneratingUnit gen in this.PowerSystem.GeneratingUnits)
            {
                this._GeneratingUnitOPFResults.Add(new GeneratingUnitOPFResult(gen, pGen_Solution[gen.Id]));
            }
            //Transmission lines
            this._TransmissionLineOPFResults = new List<TransmissionLineOPFResult>();
            foreach (TransmissionLine tl in this.PowerSystem.TransmissionLines)
            {
                this._TransmissionLineOPFResults.Add(new TransmissionLineOPFResult(tl, pFlow_Solution[tl.Id]));
            }
            //Nodes
            this._NodeOPFResults = new List<NodeOPFResult>();
            foreach (Node node in this.PowerSystem.Nodes)
            {
                double pgen = 0;
                foreach (GeneratingUnit gen in node.GeneratingUnits)
                {
                    pgen += this._GeneratingUnitOPFResults[gen.Id].Output;
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
        GeneratingUnit _GeneratingUnit;
        double _Output;

        public GeneratingUnit GeneratingUnit
        {
            get
            {
                return _GeneratingUnit;
            }
        }

        public double Output
        {
            get
            {
                return _Output;
            }
        }

        public double TotalGenerationCost
        {
            get
            {
                return this.Output * this.GeneratingUnit.MarginalCost;
            }
        }

        public GeneratingUnitOPFResult(GeneratingUnit generatingUnit, double output)
        {
            this._GeneratingUnit = generatingUnit;
            this._Output = output;
        }
    }

    /// <summary>
    /// Encapsulator of the OPF result of a node (bus angle and load shedding).
    /// </summary>
    public class NodeOPFResult
    {
        Node _Node;
        double _Angle;
        double _TotalPowerGenerated;
        double _TotalPowerConsumed;
        double _LoadShedding;
        double _SpotPrice;

        public Node Node
        {
            get
            {
                return _Node;
            }
            private set
            {
                _Node = value;
            }
        }

        public int NodeId { get { return this.Node.Id; } }

        public string NodeName { get { return this.Node.Name; } }

        public double Angle
        {
            get
            {
                return _Angle;
            }
            private set
            {
                _Angle = value;
            }
        }

        public double TotalPowerGenerated
        {
            get
            {
                return _TotalPowerGenerated;
            }

            private set
            {
                _TotalPowerGenerated = value;
            }
        }

        public double TotalPowerConsumed
        {
            get
            {
                return _TotalPowerConsumed;
            }

            private set
            {
                _TotalPowerConsumed = value;
            }
        }

        public double LoadShedding
        {
            get
            {
                return _LoadShedding;
            }
            private set
            {
                _LoadShedding = value;
            }
        }

        public double SpotPrice
        {
            get
            {
                return this._SpotPrice;
            }

            private set
            {
                this._SpotPrice = value;
            }
        }

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
        TransmissionLine _TransmissionLine;
        double _PowerFlow;

        public TransmissionLine TransmissionLine
        {
            get
            {
                return _TransmissionLine;
            }
            private set
            {
                _TransmissionLine = value;
            }
        }

        public int Id { get { return this._TransmissionLine.Id; } }
        public int NodeFromId { get { return this._TransmissionLine.NodeFromID; } }
        public int NodeToId { get { return this._TransmissionLine.NodeToID; } }

        public double PowerFlow
        {
            get
            {
                return _PowerFlow;
            }
            private set
            {
                _PowerFlow = value;
            }
        }

        public TransmissionLineOPFResult(TransmissionLine transmissionLine, double power_flow)
        {
            this.TransmissionLine = transmissionLine;
            this.PowerFlow = power_flow;
        }
    }
}
