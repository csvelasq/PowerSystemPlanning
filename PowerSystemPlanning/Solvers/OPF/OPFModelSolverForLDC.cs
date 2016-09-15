using Gurobi;
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
    /// Solver wrapper for a simple OPF model for LDC. It simply provides the concrete implementation of the optimization solver with the corresponding types for the solver, the results, etc. Intended for detailed analysis of single OPF instances.
    /// </summary>
    public class OPFModelSolverForLDC : BaseOptimizationPowerSystemSolver
    {
        public LoadBlock LoadBlock { get; protected set; }

        public OPFModelForLDC MyOPFModelForLDC { get; protected set; }
        public override BaseGRBOptimizationModel MyGRBOptimizationModel { get { return MyOPFModelForLDC; } }

        public OPFModelResultForLDC MyOPFModelResultsForLDC { get; protected set; }

        public OPFModelSolverForLDC(PowerSystem powerSystem, LoadBlock loadBlock)
            : base(powerSystem)
        {
            this.LoadBlock = loadBlock;
        }

        public override void BuildOptimizationModelResults()
        {
            MyOPFModelResultsForLDC = MyOPFModelForLDC.BuildOPFModelResultsForLDC();
        }
    }
}
