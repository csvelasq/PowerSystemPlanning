using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.MultiObjective
{
    /// <summary>
    /// An individual (as in genetic algorithms) with multiple objectives
    /// </summary>
    public interface IMultiObjectiveIndividual
    {
        /// <summary>
        /// A reference to the optimization problem for which this individual is a solution.
        /// </summary>
        BaseMultiObjectiveOptimizationProblem MyProblem { get; }
        /// <summary>
        /// The value of each of the objective functions for this individual
        /// </summary>
        List<double> ObjectiveValues { get; }
        /// <summary>
        /// Evaluates the multiple objectives for this individual.
        /// </summary>
        /// <returns>A list with the value for each objective function for this individual.</returns>
        List<double> EvaluateObjectives();
        /// <summary>
        /// Indicates whether this individual dominates the other.
        /// </summary>
        /// <param name="other">The other individual to which this individual will be compared for dominance.</param>
        /// <returns>True if this individual dominates 'other', false otherwise</returns>
        /// <remarks>
        /// An individual I1 dominates another individual I2 if no objective value of I1 
        /// is strictly less than the objective value of I2; and at least one objective of I1
        /// is strictly greater than the objective value of I2.
        /// </remarks>
        bool Dominates(IMultiObjectiveIndividual other);
        /// <summary>
        /// Indicates whether this individual dominates all the individuals in 'others'.
        /// </summary>
        /// <param name="others">The set of other individuals to which this individual will be compared for dominance.</param>
        /// <returns>True if this individual dominates each and every individual in the set 'others', false otherwise</returns>
        bool Dominates(List<IMultiObjectiveIndividual> others);
        /// <summary>
        /// Indicates whether this individual is dominated by the other (i.e. 'this' is inferior to 'other').
        /// </summary>
        /// <param name="other">The other individual to which this individual will be compared for dominance.</param>
        /// <returns>True if this individual is dominated by 'other', false otherwise</returns>
        bool IsDominatedBy(IMultiObjectiveIndividual other);
    }
}
