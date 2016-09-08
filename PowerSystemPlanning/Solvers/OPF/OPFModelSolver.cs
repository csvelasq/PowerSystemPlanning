using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.OPF
{
    /// <summary>
    /// Builder and solver of the simple OPF model.
    /// </summary>
    public class OPFModelSolver : IPowerSystemSolver
    {
        PowerSystem powerSystem;

        GRBEnv grbEnv;
        GRBModel grbModel;

        OPFModel opfModel;
        
        PowerSystemSolverResults SolverResults;

        /// <summary>
        /// The detailed results of this OPF model (per node, generator, and transmission line).
        /// </summary>
        public OPFModelResult OPFResults;

        public OPFModelSolver(PowerSystem powerSystem)
        {
            this.powerSystem = powerSystem;
            this.grbEnv = new GRBEnv();
            this.grbModel = new GRBModel(grbEnv);
            this.opfModel = new OPFModel(this.powerSystem, this.grbEnv, this.grbModel);
        }

        /// <summary>
        /// Synchronously solves the Gurobi linear optimal (DC) power flow model.
        /// </summary>
        public void Solve()
        {
            // Initializes result reporting
            this.SolverResults = new PowerSystemSolverResults();
            this.SolverResults.SolverName = this.opfModel.GRBOptimizationModelName;
            this.SolverResults.StartTime = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Builds the model
            this.opfModel.BuildGRBOptimizationModel();
            // Solves the model
            this.grbModel.Optimize();
            // Finalizes result reporting
            int status = this.grbModel.Get(GRB.IntAttr.Status);
            if (status == GRB.Status.OPTIMAL)
            {
                this.OPFResults = opfModel.BuildOPFModelResults();
            }
            else
            {
                this.OPFResults = new OPFModelResult(status);
            }
            this.SolverResults.Result = this.OPFResults;
            stopwatch.Stop();
            this.SolverResults.ExecutionTime = stopwatch.Elapsed;
            this.SolverResults.StopTime = DateTime.Now;
        }

        public void SolveAsync()
        {
            throw new NotImplementedException();
        }

        public PowerSystemSolverResults getResults()
        {
            return this.SolverResults;
        }

        /// <summary>
        /// Dispose of Gurobi model and env
        /// </summary>
        public void Dispose()
        {
            this.grbModel.Dispose();
            this.grbEnv.Dispose();
        }
    }
}
