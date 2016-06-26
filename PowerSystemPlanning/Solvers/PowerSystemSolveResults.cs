using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Encapsulates the results of a solver (e.g. Optimal Power Flow, Transmission Expansion Planning).
    /// </summary>
    public class PowerSystemSolverResults
    {
        private object _Result;
        private PowerSystemSolveResultState _State;
        private string _SolverName;
        private TimeSpan _ExecutionTime;
        private DateTime _StartTime;
        private DateTime _StopTime;
        private List<string> _Messages;

        /// <summary>
        /// The total execution time of this solver.
        /// </summary>
        public TimeSpan ExecutionTime
        {
            get
            {
                return _ExecutionTime;
            }

            set
            {
                _ExecutionTime = value;
            }
        }

        /// <summary>
        /// The name of the solver whose results are encapsulated in this object.
        /// </summary>
        public string SolverName
        {
            get
            {
                return _SolverName;
            }

            set
            {
                _SolverName = value;
            }
        }

        /// <summary>
        /// The current state of the solution (e.g. successful, failed).
        /// </summary>
        public PowerSystemSolveResultState State
        {
            get
            {
                return _State;
            }

            set
            {
                _State = value;
            }
        }

        /// <summary>
        /// Messages of the solution process.
        /// </summary>
        public List<string> Messages
        {
            get
            {
                return _Messages;
            }

            set
            {
                _Messages = value;
            }
        }

        /// <summary>
        /// start time of the process
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _StartTime;
            }

            set
            {
                _StartTime = value;
            }
        }

        /// <summary>
        /// stop time of the process (upon completion, successful or not)
        /// Does not consider pauses
        /// </summary>
        public DateTime StopTime
        {
            get
            {
                return _StopTime;
            }

            set
            {
                _StopTime = value;
            }
        }

        /// <summary>
        /// The result of the solver, can be of various types.
        /// </summary>
        public object Result
        {
            get
            {
                return _Result;
            }

            set
            {
                _Result = value;
            }
        }

        public PowerSystemSolverResults() { }

        public PowerSystemSolverResults(object result, PowerSystemSolveResultState state, string solverName, TimeSpan executionTime, DateTime startTime, DateTime stopTime, List<string> messages)
        {
            this._Result = result;
            this._State = state;
            this._SolverName = SolverName;
            this._ExecutionTime = executionTime;
            this._StartTime = startTime;
            this._StopTime = stopTime;
            this._Messages = messages;
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
}
