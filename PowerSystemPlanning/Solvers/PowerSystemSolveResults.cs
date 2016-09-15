using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Encapsulates the overall results of a solver (e.g. elapsed time, solver messages, etc).
    /// </summary>
    public class SolverResults
    {
        /// <summary>
        /// The name of the solver whose results are encapsulated in this object.
        /// </summary>
        public string SolverName { get; set; }

        /// <summary>
        /// The current state of the solution (e.g. successful, failed).
        /// </summary>
        public SolverResultState State { get; set; }

        /// <summary>
        /// Messages of the solution process.
        /// </summary>
        public List<string> SolverMessages { get; set; }

        DateTime _StartTime;
        /// <summary>
        /// start time of the process
        /// </summary>
        public DateTime StartTime
        {
            get { return _StartTime; }
            set
            {
                _StartTime = value;
                SolverMessages.Add(String.Format("Solver {0} started on {1}", SolverName, _StartTime));
            }
        }

        TimeSpan _ExecutionTime;
        /// <summary>
        /// The total execution time of this solver.
        /// </summary>
        public TimeSpan ExecutionTime
        {
            get { return _ExecutionTime; }
            set
            {
                _ExecutionTime = value;
                SolverMessages.Add(String.Format("Finished on {0}", StopTime));
                SolverMessages.Add(String.Format("Elapsed time: {0}", ExecutionTime));
            }
        }

        /// <summary>
        /// stop time of the process (upon completion, successful or not)
        /// Does not consider pauses
        /// </summary>
        public DateTime StopTime { get { return _StartTime + _ExecutionTime; } }

        /// <summary>
        /// The result of the solver's internal process (e.g. the optimization).
        /// </summary>
        public object Result { get; set; }

        public SolverResults()
        {
            SolverMessages = new List<string>();
        }

        public override string ToString()
        {
            return String.Format("Solver '{0}' {1}. Elapsed time: {2} (start: {3}; finish: {4}).", SolverName, State, ExecutionTime, StartTime, StopTime);
        }
    }

    /// <summary>
    /// Represents the state of the result of a solver (e.g. successful, failed).
    /// </summary>
    public enum SolverResultState
    {
        Successful,
        SuccessfulWithWarning,
        Failed,
        Paused,
        Processing
    }
}
