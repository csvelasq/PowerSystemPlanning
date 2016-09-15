using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels.Planning
{
    /// <summary>
    /// An individual (as in genetic algorithms) with multiple objectives
    /// </summary>
    public interface IMultiObjectiveIndividual
    {
        /// <summary>
        /// The value of each of the objective functions for this individual
        /// </summary>
        List<double> ObjectiveValues { get; }
        /// <summary>
        /// Evaluates the multiple objectives for this individual.
        /// </summary>
        /// <returns>A list with the value for each objective function for this individual.</returns>
        List<double> EvaluateObjectives();
    }
}
