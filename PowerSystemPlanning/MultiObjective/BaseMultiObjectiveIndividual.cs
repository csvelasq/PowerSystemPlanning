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
    public abstract class BaseMultiObjectiveIndividual : IMultiObjectiveIndividual
    {
        /// <summary>
        /// A reference to the optimization problem for which this individual is a solution.
        /// </summary>
        public IMultiObjectiveOptimizationProblem MyProblem { get; }
        /// <summary>
        /// The value of each of the objective functions for this individual
        /// </summary>
        public abstract List<double> ObjectiveValues { get; }
        /// <summary>
        /// Evaluates the multiple objectives for this individual.
        /// </summary>
        /// <returns>A list with the value for each objective function for this individual.</returns>
        public abstract List<double> EvaluateObjectives();
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
        public bool Dominates(IMultiObjectiveIndividual other)
        {
            if (MyProblem != other.MyProblem)
                throw new ArgumentException("Attempted to compare individuals from different optimization problems.");

            bool atLeastOneObjectiveBetter = false;

            for (int i = 0; i < MyProblem.MyObjectiveFunctionsDefinition.Count; i++)
            {
                if (ObjectiveValues[i] > other.ObjectiveValues[i])
                {
                    return false;
                }
                if (ObjectiveValues[i] < other.ObjectiveValues[i])
                {
                    atLeastOneObjectiveBetter = true;
                }
            }
            return atLeastOneObjectiveBetter;
        }
        /// <summary>
        /// Indicates whether this individual dominates all the individuals in 'others'.
        /// </summary>
        /// <param name="others">The set of other individuals to which this individual will be compared for dominance.</param>
        /// <returns>True if this individual dominates each and every individual in the set 'others', false otherwise</returns>
        public bool Dominates(List<IMultiObjectiveIndividual> others)
        {
            foreach (var other in others)
            {
                if (!this.Dominates(other))
                {
                    //There is one individual which is not dominated by 'this'
                    return false;
                }
            }
            //'this' dominates each and every individual in 'others'
            return true;
        }
        /// <summary>
        /// Indicates whether this individual is dominated by the other (i.e. 'this' is inferior to 'other').
        /// </summary>
        /// <param name="other">The other individual to which this individual will be compared for dominance.</param>
        /// <returns>True if this individual is dominated by 'other' (i.e. 'this' is inferior to 'other'), false otherwise</returns>
        public bool IsDominatedBy(IMultiObjectiveIndividual other)
        {
            return other.Dominates(this);
        }

        protected BaseMultiObjectiveIndividual(IMultiObjectiveOptimizationProblem problem)
        {
            MyProblem = problem;
        }
    }
}
