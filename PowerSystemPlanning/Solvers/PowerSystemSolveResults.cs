using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Encapsulates the overall results of a solver, including specific optimization results.
    /// </summary>
    public class PowerSystemSolverResults
    {
        /// <summary>
        /// The total execution time of this solver.
        /// </summary>
        public TimeSpan ExecutionTime { get; set; }

        /// <summary>
        /// The name of the solver whose results are encapsulated in this object.
        /// </summary>
        public string SolverName { get; set; }

        /// <summary>
        /// The current state of the solution (e.g. successful, failed).
        /// </summary>
        public PowerSystemSolveResultState State { get; set; }

        /// <summary>
        /// Messages of the solution process.
        /// </summary>
        public List<string> Messages { get; set; }

        /// <summary>
        /// start time of the process
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// stop time of the process (upon completion, successful or not)
        /// Does not consider pauses
        /// </summary>
        public DateTime StopTime { get; set; }

        /// <summary>
        /// The result of the solver's internal process (e.g. the optimization).
        /// </summary>
        public object Result { get; set; }

        public PowerSystemSolverResults() { }

        public PowerSystemSolverResults(object result, PowerSystemSolveResultState state, string solverName, TimeSpan executionTime, DateTime startTime, DateTime stopTime, List<string> messages)
        {
            this.Result = result;
            this.State = state;
            this.SolverName = SolverName;
            this.ExecutionTime = executionTime;
            this.StartTime = startTime;
            this.StopTime = stopTime;
            this.Messages = messages;
        }

        public override string ToString()
        {
            return String.Format("Power system solver '{0}' {1}. Elapsed time: {2} (start: {3}; finish: {4}).", SolverName, State, ExecutionTime, StartTime, StopTime);
        }
    }

    /// <summary>
    /// Represents the state of the result of a solver (e.g. successful, failed).
    /// </summary>
    public enum PowerSystemSolveResultState
    {
        Successful,
        SuccessfulWithWarning,
        Failed,
        Paused,
        Processing
    }

    public enum OptimizationResultStatus
    {
        Optimal,
        Infeasible,
        Unbounded,
        Other
    }
}
