using System.Collections.Generic;

namespace PowerSystemPlanning.MultiObjective
{
    /// <summary>
    /// Represents a multi-objective optimization problem.
    /// </summary>
    public interface IMultiObjectiveOptimizationProblem
    {
        /// <summary>
        /// The name of this multiobjective optimization problem.
        /// </summary>
        string MyMOOName { get; }
        /// <summary>
        /// A list defining the objective functions of this multiobjective problem.
        /// </summary>
        List<ObjectiveFunctionInOptimizationProblem> MyObjectiveFunctionsDefinition { get; set; }
        /// <summary>
        /// Gets the number of objectives in this optimization problem.
        /// </summary>
        int NumberOfObjectives { get; }
    }
}