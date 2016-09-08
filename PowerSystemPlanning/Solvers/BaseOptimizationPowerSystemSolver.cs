using Gurobi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Base class for power system solvers which wrap a single optimization model (e.g. OPF, LDC OPF).
    /// </summary>
    public abstract class BaseOptimizationPowerSystemSolver : IPowerSystemSolver
    {
        protected PowerSystem powerSystem;

        protected PowerSystemSolverResults _SolverResults;
        /// <summary>
        /// Encapsulator of the overall results of the solution process (e.g. elapsed time).
        /// </summary>
        public PowerSystemSolverResults SolverResults { get { return this._SolverResults; } protected set { _SolverResults = value; } }

        protected GRBEnv _grbEnv;
        protected GRBModel _grbModel;
        protected int _GRBModelStatus;
        public int GRBModelStatus { get { return _GRBModelStatus; } protected set { _GRBModelStatus = value; } }

        /// <summary>
        /// Gurobi optimization model wrapped by this solver.
        /// </summary>
        /// <remarks>Concrete implementations should provide a property of the class of the particular GRBOptimizationModel, and override this getter in order to point to the specific GRBOptimizationModel.</remarks>
        public abstract IGRBOptimizationModel GRBOptimizationModel { get; }

        /// <summary>
        /// Encapsulator of the specific results of the optimization problem solved by this solver (e.g. variables).
        /// </summary>
        /// <remarks>Concrete implementations should provide a property of the class of the particular GRBOptimizationModelResult, and override this getter in order to point to the specific GRBOptimizationModelResult.</remarks>
        public abstract BaseGRBOptimizationModelResult GRBOptimizationModelResults { get; }

        /// <summary>
        /// Base abstract constructor for power system solvers which solve a single Gurobi optimization model.
        /// </summary>
        /// <param name="powerSystem">The power system to which this solver will be applied.</param>
        /// <remarks>GRBEnv and GRBModel are first created here, and then passed through to the specific optimization model objects.</remarks>
        public BaseOptimizationPowerSystemSolver(PowerSystem powerSystem)
        {
            this.powerSystem = powerSystem;
            this._grbEnv = new GRBEnv();
            this._grbModel = new GRBModel(this._grbEnv);
        }

        /// <summary>
        /// Builds the Gurobi optimization model (creates variables, constraints, etc)
        /// </summary>
        /// <remarks>This method must be called before Solve.</remarks>
        public abstract void Build();

        /// <summary>
        /// Synchronously solves the Gurobi optimization model wrapped by this solver.
        /// </summary>
        public void Solve()
        {
            // Initializes result reporting
            this.SolverResults = new PowerSystemSolverResults();
            this.SolverResults.SolverName = this.GRBOptimizationModel.GRBOptimizationModelName;
            this.SolverResults.StartTime = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Solves the model
            this._grbModel.Optimize();
            // Finalizes result reporting
            GRBModelStatus = this._grbModel.Get(GRB.IntAttr.Status);
            this.BuildOptimizationModelResults();
            this.SolverResults.Result = this.GRBOptimizationModelResults;
            stopwatch.Stop();
            this.SolverResults.ExecutionTime = stopwatch.Elapsed;
            this.SolverResults.StopTime = DateTime.Now;
        }

        /// <summary>
        /// Builds the results of the optimization model (to be overwritten by concrete implementations).
        /// </summary>
        public abstract void BuildOptimizationModelResults();

        /// <summary>
        /// Asynchronously solves the Gurobi optimization model wrapped by this solver.
        /// </summary>
        public void SolveAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the results of the solution process.
        /// </summary>
        /// <returns>An encapsulator of the general results of this process, including the specific results of the gurobi optimization model.</returns>
        public PowerSystemSolverResults getResults()
        {
            return this._SolverResults;
        }

        /// <summary>
        /// Dispose of Gurobi model and env. Must be called after solving the model.
        /// </summary>
        public void Dispose()
        {
            this._grbModel.Dispose();
            this._grbEnv.Dispose();
        }
    }
}
