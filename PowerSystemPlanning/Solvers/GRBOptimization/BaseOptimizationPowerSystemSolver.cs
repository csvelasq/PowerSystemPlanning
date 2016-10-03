using PowerSystemPlanning.Models.SystemBaseData;
using System;

namespace PowerSystemPlanning.Solvers.GRBOptimization
{
    /// <summary>
    /// Base class for power system solvers which wrap a single optimization model (e.g. OPF, LDC OPF). Intended for detailed analysis of single instances. Concrete implementations should only override <see cref="MyGRBOptimizationModel"/> and <see cref="BuildOptimizationModelResults"/>.
    /// </summary>
    /// <remarks>This class adds non-essential functionality to <see cref="BaseGRBOptimizationModel"/>, particularly automated detailed result reporting such as start time, execution time, etc.
    /// This class should be implemented in order to provide the ability to analyze detailed results for an existing optimization model. Implementing classes should override <see cref="BuildOptimizationModelResults"/> in order to build detailed and specific results for the wrapped optimization model. The reason for such an implementation is to avoid disposing of the gurobi model before detailed results can be extracted (<see cref="Solve"/>). Once the implementation overwrites the two required elements, using the solver only requires a single call to <see cref="Solve"/> (obviously after creating an instance of this class).
    /// </remarks>
    public abstract class BaseOptimizationPowerSystemSolver : IPowerSystemStudy
    {
        public IPowerSystem MyPowerSystem { get; protected set; }

        /// <summary>
        /// Encapsulator of the overall results of the solution process (e.g. elapsed time).
        /// </summary>
        public SolverResults MySolverResults { get; protected set; }

        public string SolverName => MyGRBOptimizationModel.GRBOptimizationModelName;

        public string StudyName { get; set; }

        /// <summary>
        /// Gurobi optimization model wrapped by this solver.
        /// </summary>
        /// <remarks>Concrete implementations should provide a property of the class of the particular GRBOptimizationModel, and override this getter in order to point to the specific GRBOptimizationModel.</remarks>
        public abstract BaseGRBOptimizationModel MyGRBOptimizationModel { get; }

        /// <summary>
        /// Base abstract constructor for power system solvers which solve a single Gurobi optimization model.
        /// </summary>
        /// <param name="powerSystem">The power system to which this solver will be applied.</param>
        /// <remarks>GRBEnv and GRBModel are first created here, and then passed through to the specific optimization model objects.</remarks>
        protected BaseOptimizationPowerSystemSolver(IPowerSystem powerSystem)
        {
            this.MyPowerSystem = powerSystem;
        }
        
        /// <summary>
        /// Builds the specific and detailed results of the optimization model (to be overwritten by concrete implementations).
        /// </summary>
        public abstract void BuildOptimizationModelResults();

        /// <summary>
        /// Synchronously solves the Gurobi optimization model wrapped by this solver.
        /// </summary>
        public void Solve()
        {
            // Initializes result reporting
            this.MySolverResults = new SolverResults(this.MyGRBOptimizationModel.GRBOptimizationModelName);
            this.MySolverResults.StartSolutionProcess();
            // Builds the model
            MyGRBOptimizationModel.BuildGRBOptimizationModel();
            // Solves the model
            MyGRBOptimizationModel.OptimizeGRBModel();
            // Builds results of the model
            MyGRBOptimizationModel.BuildGRBOptimizationModelResults();
            this.BuildOptimizationModelResults(); //needs to be overwritten, and it is called here before Dispose, that is why i don't use MyGRBOptimizationModel.BuildSolveAndDispose()
            // Disposes of model
            MyGRBOptimizationModel.Dispose();
            // Finalizes result reporting
            this.MySolverResults.StopSuccessfulSolutionProcess(this.MyGRBOptimizationModel.MyBaseGRBOptimizationModelResult);
        }

        /// <summary>
        /// Asynchronously solves the Gurobi optimization model wrapped by this solver.
        /// </summary>
        public void SolveAsync()
        {
            throw new NotImplementedException();
        }

        public void SaveToXml(string path)
        {
            throw new NotImplementedException();
        }
    }
}
