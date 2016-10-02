using System;
using System.Collections.Generic;
using System.Linq;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Generator;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Load;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Nodes;
using PowerSystemPlanning.BindingModels.StateBinding.Generator;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Models.SystemState.Branch;
using PowerSystemPlanning.Models.SystemState.Generator;
using PowerSystemPlanning.Models.SystemState.Nodes;

namespace PowerSystemPlanning.Solvers.OPF.OpfResults
{
    /// <summary>
    /// Encapsulates results of the OPF model.
    /// </summary>
    public class OPFModelResult : BaseGRBOptimizationModelResult
    {
        public IPowerSystemState MyPowerSystemState { get; protected set; }

        /******************************************
         * DETAILED RESULTS
         ******************************************/
        public List<NodeOPFResult> MyNodeOPFResults { get; protected set; }
        public List<LoadOPFResult> MyLoadOpfResults { get; protected set; }
        public List<GeneratingUnitOPFResult> MyGeneratingUnitOPFResults { get; protected set; }
        public List<TransmissionLineOPFResult> MyTransmissionLineOPFResults { get; protected set; }

        /******************************************
         * AGREGGATE RESULTS
         ******************************************/
        /// <summary>
        /// Gets the total operation cost (MMUS$, generation plus load shedding, the model's objective value).
        /// </summary>
        public double TotalOperationCost => TotalGenerationCost + TotalCurtailmentCost;

        public double HourlyOperationCost => HourlyGenerationCost + HourlyCurtailmentCost;

        public IEnumerable<double> NodalSpotPrices =>
            from load in MyNodeOPFResults
            select load.SpotPrice;

        public double AvgSpotPrice => NodalSpotPrices.Average();

        public double MaxSpotPrice => NodalSpotPrices.Max();

        public double MinSpotPrice => NodalSpotPrices.Min();

        public double SpotPriceRange => MaxSpotPrice - MinSpotPrice;

        /******************************************
         * GENERATION (PHYSICAL)
         ******************************************/
        public double TotalPowerGenerated =>
            (from gen in MyGeneratingUnitOPFResults
             select gen.PowerOutput).Sum();
        public double TotalEnergyGenerated =>
            (from gen in MyGeneratingUnitOPFResults
             select gen.EnergyOutput).Sum();

        /******************************************
         * GENERATION (COST)
         ******************************************/
        public double HourlyGenerationCost =>
            (from gen in MyGeneratingUnitOPFResults
             select gen.HourlyGenerationCost).Sum();
        public double TotalGenerationCost =>
            (from gen in MyGeneratingUnitOPFResults
             select gen.TotalGenerationCost).Sum();

        /******************************************
         * CONSUMPTION (PHYSICAL)
         ******************************************/
        public double TotalPowerConsumed =>
            (from load in MyLoadOpfResults
             select load.PowerConsumed).Sum();
        public double TotalEnergyConsumed =>
            (from load in MyLoadOpfResults
             select load.EnergyConsumed).Sum();

        /******************************************
         * CURTAILMENT (PHYSICAL)
         ******************************************/
        public double PowerCurtailed =>
            (from load in MyLoadOpfResults
             select load.PowerCurtailed).Sum();
        public double EnergyCurtailed =>
            (from load in MyLoadOpfResults
             select load.PowerCurtailed).Sum();

        /******************************************
         * CURTAILMENT (COST)
         ******************************************/
        public double HourlyCurtailmentCost =>
            (from load in MyLoadOpfResults
             select load.HourlyCurtailmentCost).Sum();
        public double TotalCurtailmentCost =>
            (from load in MyLoadOpfResults
             select load.TotalCurtailmentCost).Sum();

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
            //Loads
            this.MyLoadOpfResults = new List<LoadOPFResult>();
            foreach (var load in MyPowerSystemState.InelasticLoadStates)
            {
                MyLoadOpfResults.Add(new LoadOPFResult(this, load, lShed_Solution[MyPowerSystemState.InelasticLoadStates.IndexOf(load)]));
            }
            //Generating units
            this.MyGeneratingUnitOPFResults = new List<GeneratingUnitOPFResult>();
            foreach (var gen in MyPowerSystemState.GeneratingUnitStates)
            {
                double output = 0;
                if (gen.IsAvailable)
                {
                    output = pGen_Solution[MyPowerSystemState.ActiveGeneratingUnitStates.IndexOf(gen)];
                }
                MyGeneratingUnitOPFResults.Add(
                    new GeneratingUnitOPFResult(this,
                        gen,
                        output));
            }
            //Transmission lines
            this.MyTransmissionLineOPFResults = new List<TransmissionLineOPFResult>();
            foreach (var tl in this.MyPowerSystemState.SimpleTransmissionLineStates)
            {
                double flow = 0;
                if (tl.IsAvailable)
                {
                    flow = pFlow_Solution[MyPowerSystemState.ActiveSimpleTransmissionLineStates.IndexOf(tl)];
                }
                MyTransmissionLineOPFResults.Add(
                        new TransmissionLineOPFResult(this, tl, flow));
            }
            //Nodes
            this.MyNodeOPFResults = new List<NodeOPFResult>();
            foreach (var node in this.MyPowerSystemState.NodeStates)
            {
                int b = MyPowerSystemState.NodeStates.IndexOf(node);
                this.MyNodeOPFResults.Add(
                    new NodeOPFResult(this, node, busAng_Solution[b], nodalSpotPrice[b])
                    );
            }
        }
    }
}
