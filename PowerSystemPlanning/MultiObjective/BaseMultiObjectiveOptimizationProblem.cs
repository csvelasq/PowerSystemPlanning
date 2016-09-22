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
    /// Concrete implementations must override <see cref="MyMOOName"/> with an arbitrary problem name;
    /// and they must define, at the outset, the list of objective functions <see cref="MyObjectiveFunctionsDefinition"/>.
    /// </remarks>
    public abstract class BaseMultiObjectiveOptimizationProblem : IMultiObjectiveOptimizationProblem
    {
        /// <summary>
        /// The name of this multiobjective optimization problem.
        /// </summary>
        public abstract string MyMOOName { get; }

        /// <summary>
        /// A list defining the objective functions of this multiobjective problem.
        /// </summary>
        public List<ObjectiveFunctionInOptimizationProblem> MyObjectiveFunctionsDefinition
        {
            get
            {
                return _MyObjectiveFunctionsDefinition;
            }

            set
            {
                _MyObjectiveFunctionsDefinition = value;
            }
        }
        List<ObjectiveFunctionInOptimizationProblem> _MyObjectiveFunctionsDefinition;

        /// <summary>
        /// Gets the number of objectives in this optimization problem.
        /// </summary>
        public int NumberOfObjectives
        {
            get { return MyObjectiveFunctionsDefinition.Count; }
        }
    }

    /// <summary>
    /// Defines a single objective function of an optimization problem (e.g. minimize costs; or maximize benefits)
    /// </summary>
    public class ObjectiveFunctionInOptimizationProblem
    {
        /// <summary>
        /// The sense of this optimization objective (minimize or maximize).
        /// </summary>
        public OptimizationSense MySense
        {
            get
            {
                return _MySense;
            }

            set
            {
                _MySense = value;
            }
        }
        OptimizationSense _MySense;

        /// <summary>
        /// The name of this objective function.
        /// </summary>
        public string MyObjectiveName
        {
            get
            {
                return _MyObjectiveName;
            }

            set
            {
                _MyObjectiveName = value;
            }
        }
        string _MyObjectiveName;
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
