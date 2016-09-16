using Gurobi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Base implementation of a Gurobi optimization model. Concrete implementations should only override <see cref="GRBOptimizationModelName"/> and <see cref="BuildGRBOptimizationModel"/>.
    /// </summary>
    /// <remarks>
    /// Provides basic and common functionality for solving a gurobi optimization model. This class should be overwritten by any particular gurobi optimization model. Once <see cref="GRBOptimizationModelName"/> and <see cref="BuildGRBOptimizationModel"/> are overwritten, there are two options for using the resulting concrete implementation:
    ///     1. If only the objective value is required, call <see cref="BuildSolveAndDisposeModel"/> immediately after initializing the concrete object.
    ///     2. If more detailed results are required, proceed as following:
    ///         +Call <see cref="BuildGRBOptimizationModel"/>
    ///         +Call <see cref="OptimizeGRBModel"/>
    ///         +Call <see cref="BuildGRBOptimizationModelResults"/>
    ///         +Build detailed optimization results (custom process)
    ///         +Call <see cref="Dispose"/>
    /// 
    /// To easily get detailed results you can also implement a wrapper by extending <see cref="BaseOptimizationPowerSystemSolver"/>, and use that wrapper to solve the wrapped optimization model.
    /// </remarks>
    public abstract class BaseGRBOptimizationModel
    {
        /// <summary>
        /// The name of the implemented Gurobi optimization model.
        /// </summary>
        public abstract string GRBOptimizationModelName { get; }

        public GRBEnv MyGrbEnv { get; protected set; }
        public GRBModel MyGrbModel { get; protected set; }

        /// <summary>
        /// The current status of this Gurobi optimization model.
        /// </summary>
        protected int GRBModelStatus { get { return MyGrbModel.Get(GRB.IntAttr.Status); } }
        /// <summary>
        /// The value of the objective function in this Gurobi optimization model.
        /// </summary>
        protected double ObjVal { get { return MyGrbModel.Get(GRB.DoubleAttr.ObjVal); } }

        /// <summary>
        /// The basic results of this gurobi optimization model.
        /// </summary>
        public BaseGRBOptimizationModelResult MyBaseGRBOptimizationModelResult { get; protected set; }

        /// <summary>
        /// Constructor, creates new Gurobi environment and variables.
        /// </summary>
        public BaseGRBOptimizationModel()
        {
            MyGrbEnv = new GRBEnv();
            MyGrbModel = new GRBModel(MyGrbEnv);
        }

        /// <summary>
        /// Constructor, uses the provided Gurobi environment and variables.
        /// </summary>
        /// <param name="grbEnv"></param>
        /// <param name="grbModel"></param>
        public BaseGRBOptimizationModel(GRBEnv grbEnv, GRBModel grbModel)
        {
            MyGrbEnv = grbEnv;
            MyGrbModel = grbModel;
        }

        /// <summary>
        /// Builds the Gurobi optimization model (objective, variables and constraints).
        /// </summary>
        public abstract void BuildGRBOptimizationModel();

        /// <summary>
        /// Optimizes a previously built Gurobi optimization model.
        /// </summary>
        public void OptimizeGRBModel()
        {
            MyGrbModel.Optimize();
        }

        /// <summary>
        /// Gets the basic state and result (objective value) on this model. Do not override.
        /// </summary>
        /// <returns>An encapsulator of the basic state and result of this model, which is also saved to <seealso cref="MyBaseGRBOptimizationModelResult"/>.</returns>
        public BaseGRBOptimizationModelResult BuildGRBOptimizationModelResults()
        {
            MyBaseGRBOptimizationModelResult = new BaseGRBOptimizationModelResult(GRBModelStatus, ObjVal);
            return MyBaseGRBOptimizationModelResult;
        }

        /// <summary>
        /// Dispose of Gurobi model and env. Must be called after solving the model and retrieving relevant results.
        /// </summary>
        public void Dispose()
        {
            MyGrbModel.Dispose();
            MyGrbEnv.Dispose();
        }

        /// <summary>
        /// Builds and optimizes this Gurobi optimization model.
        /// </summary>
        /// <returns>An encapsulator of the results of the optimization process.</returns>
        public BaseGRBOptimizationModelResult BuildSolveAndDisposeModel()
        {
            BuildGRBOptimizationModel();
            OptimizeGRBModel();
            BuildGRBOptimizationModelResults();
            Dispose();
            return MyBaseGRBOptimizationModelResult;
        }
    }
}
