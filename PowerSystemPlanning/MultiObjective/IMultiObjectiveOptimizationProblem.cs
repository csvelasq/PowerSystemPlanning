using System.Collections.Generic;

namespace PowerSystemPlanning.MultiObjective
{
    /// <summary>
    /// Represents a multi-objective optimization problem.
    /// </summary>
    public interface IMultiObjectiveOptimizationProblem
    {
        /// <summary>
        /// A list defining the objective functions of this multiobjective problem.
        /// </summary>
        List<ObjectiveFunctionInOptimizationProblem> MyObjectiveFunctionsDefinition { get; }
    }
}