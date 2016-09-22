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
    public class BaseMultiObjectiveIndividualList : List<IMultiObjectiveIndividual>
    {
        /// <summary>
        /// A reference to the optimization problem for which this individual is a solution.
        /// </summary>
        public BaseMultiObjectiveOptimizationProblem MyProblem
        {
            get
            {
                return _MyProblem;
            }

            protected set
            {
                _MyProblem = value;
            }
        }
        BaseMultiObjectiveOptimizationProblem _MyProblem;

        public BaseMultiObjectiveIndividualList(BaseMultiObjectiveOptimizationProblem myProblem)
        {
            this.MyProblem = myProblem;
        }

        /// <summary>
        /// Builds the pareto efficient frontier by comparing each individual with every other individual in this collection.
        /// </summary>
        /// <returns>A collection of pareto efficient </returns>
        public BaseMultiObjectiveIndividualList BuildParetoEfficientFrontier()
        {
            BaseMultiObjectiveIndividualList paretoFront = new BaseMultiObjectiveIndividualList(this.MyProblem);
            //check every individual in this collection
            for (int i = 0; i < this.Count; i++)
            {
                var individual = this[i];
                //check 'ind' against every other individual
                //we need to find one individual that dominates ind1
                //if not, ind1 is in the pareto frontier
                bool individualIsInParetoFront = true;
                for (int j = 0; j < this.Count; j++)
                {
                    if (j != i)
                    {
                        if (this[j].Dominates(individual))
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
