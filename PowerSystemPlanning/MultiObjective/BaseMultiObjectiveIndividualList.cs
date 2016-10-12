using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.MultiObjective
{
    /// <summary>
    /// A collection of multiobjective individuals used to build pareto frontiers.
    /// </summary>
    /// <remarks>
    /// This class is intended to be used directly in order to build pareto frontiers.
    /// This class is not intended to be extended by classes which deal directly 
    /// with concrete implementations of <see cref="BaseMultiObjectiveIndividual"/>.
    /// </remarks>
    public class BaseMultiObjectiveIndividualList
    {
        /// <summary>
        /// A reference to the optimization problem for which this individual is a solution.
        /// </summary>
        public IMultiObjectiveOptimizationProblem MyProblem { get; protected set; }

        public List<IMultiObjectiveIndividual> Individuals { get; protected set; }

        public BaseMultiObjectiveIndividualList(IMultiObjectiveOptimizationProblem myProblem)
        {
            this.MyProblem = myProblem;
        }

        public BaseMultiObjectiveIndividualList(IMultiObjectiveOptimizationProblem myProblem,
            IList<IMultiObjectiveIndividual> individuals)
        {
            this.MyProblem = myProblem;
            Individuals = new List<IMultiObjectiveIndividual>(individuals);
        }

        /// <summary>
        /// Builds the pareto efficient frontier by comparing each individual with every other individual in this collection.
        /// </summary>
        /// <returns>A collection of pareto efficient individuals.</returns>
        public List<IMultiObjectiveIndividual> BuildParetoEfficientFrontier()
        {
            List<IMultiObjectiveIndividual> paretoFront = new List<IMultiObjectiveIndividual>();
            //check every individual in this collection
            for (int i = 0; i < Individuals.Count; i++)
            {
                var individual = Individuals[i];
                //check 'ind' against every other individual
                //we need to find one individual that dominates ind1
                //if not, ind1 is in the pareto frontier
                bool individualIsInParetoFront = true;
                for (int j = 0; j < Individuals.Count; j++)
                {
                    if (j != i)
                    {
                        if (Individuals[j].Dominates(individual))
                        {
                            individualIsInParetoFront = false;
                            break;
                        }
                    }
                }
                if (individualIsInParetoFront) paretoFront.Add(individual);
            }
            return paretoFront;
        }
    }
}
