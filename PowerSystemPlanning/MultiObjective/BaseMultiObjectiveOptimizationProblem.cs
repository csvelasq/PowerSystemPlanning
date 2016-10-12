using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.MultiObjective
{
    /// <summary>
    /// Base class for defining a multi-objective optimization problem
    /// </summary>
    /// <remarks>
    /// Concrete implementations must define, at the outset, the list of objective functions <see cref="MyObjectiveFunctionsDefinition"/>.
    /// </remarks>
    public abstract class BaseMultiObjectiveOptimizationProblem : IMultiObjectiveOptimizationProblem
    {
        /// <summary>
        /// A list defining the objective functions of this multiobjective problem.
        /// </summary>
        public List<ObjectiveFunctionInOptimizationProblem> MyObjectiveFunctionsDefinition
        { get; protected set; }
    }

    /// <summary>
    /// Defines a single objective function of an optimization problem (e.g. minimize costs; or maximize benefits)
    /// </summary>
    public class ObjectiveFunctionInOptimizationProblem
    {
        /// <summary>
        /// The sense of this optimization objective (minimize or maximize).
        /// </summary>
        public OptimizationSense MySense { get; set; }

        /// <summary>
        /// The name of this objective function.
        /// </summary>
        public string MyObjectiveName { get; set; }
    }

    /// <summary>
    /// Represents the sense of an optimization function (maxi/minimize)
    /// </summary>
    public enum OptimizationSense
    {
        Minimize,
        Maximize
    }
}
