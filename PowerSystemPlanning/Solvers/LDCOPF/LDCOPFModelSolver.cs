using Gurobi;
using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.Solvers.OPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.LDCOPF
{
    /// <summary>
    /// Solver wrapper for a simple OPF model for LDC. It simply provides the concrete implementation of the optimization solver with the corresponding types for the solver, the results, etc. Intended for detailed analysis of single LDC OPF instances.
    /// </summary>
    public class LDCOPFModelSolver : BaseOptimizationPowerSystemSolver
    {
        public LoadDurationCurveByBlocks DurationCurveBlocks { get; protected set; }
        
        public LDCOPFModel MyLDCOPFModel { get; protected set; }
        public override BaseGRBOptimizationModel MyGRBOptimizationModel { get { return MyLDCOPFModel; } }

        /// <summary>
        /// The detailed results of this LDC OPF model (per node, generator, and transmission line).
        /// </summary>
        public LDCOPFModelResults MyLDCOPFResults { get; protected set; }

        public LDCOPFModelSolver(PowerSystem powerSystem, LoadDurationCurveByBlocks durationCurveBlocks)
            : base(powerSystem)
        {
            this.DurationCurveBlocks = durationCurveBlocks;
        }

        public override void BuildOptimizationModelResults()
        {
            MyLDCOPFResults = MyLDCOPFModel.BuildLDCOPFModelResults();
        }
    }
}
