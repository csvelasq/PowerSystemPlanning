using Gurobi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Required methods and properties for any Gurobi optimization model.
    /// </summary>
    public interface IGRBOptimizationModel
    {
        /// <summary>
        /// The name of the implemented Gurobi optimization model.
        /// </summary>
        string GRBOptimizationModelName { get; }
        GRBEnv grbEnv { get; }
        GRBModel grbModel { get; }
        /// <summary>
        /// Builds the Gurobi optimization model (objective, variables and constraints).
        /// </summary>
        void BuildGRBOptimizationModel();
        /// <summary>
        /// Builds the results of the optimization process.
        /// </summary>
        /// <returns>An encapsulator of the results of the optimization process.</returns>
        BaseGRBOptimizationModelResult BuildGRBOptimizationModelResults();
    }
}
