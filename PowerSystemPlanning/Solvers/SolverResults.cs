using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Encapsulates the overall results of a solver (e.g. elapsed time, solver messages, etc).
    /// </summary>
    /// <remarks>
    /// This class encapsulates overall results of a solver and provides a structure for simplifyng its use in practical solvers. 
    /// 
    /// Solvers should use the constructor providing a custom name, and then call the methods
    /// <see cref="StartSolutionProcess"/> upon starting the solution process,
    /// and then some of the provided methods to signal that the solution process has finished
    /// (e.g. <see cref="StopSuccessfulSolutionProcess(object)"/> or <see cref="StopFailedSolutionProcessWithException(Exception)"/>).
    /// That's it.
    /// 
    /// This class also provides logging at start and stop.
    /// </remarks>
    public class SolverResults
    {
        /// <summary>
        /// NLog Logger for this solver.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

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

        Stopwatch MyStopWatch { get; set; }

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
                string msg = String.Format("Solver '{0}' started on {1}", SolverName, StartTime);
                SolverMessages.Add(msg);
                logger.Info(msg);
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
                string msg = String.Format("Solver '{0}' finished with state '{1}' after {2} (on {3})", SolverName, State, ExecutionTime, StopTime);
                SolverMessages.Add(msg);
                logger.Info(msg);
            }
        }

        /// <summary>
        /// stop time of the process (upon completion, successful or not)
        /// Does not consider pauses
        /// </summary>
        public DateTime StopTime { get { return StartTime + ExecutionTime; } }

        /// <summary>
        /// The result of the solver's internal process (e.g. the optimization).
        /// </summary>
        public object Result { get; set; }

        public SolverResults()
        {
            SolverMessages = new List<string>();
            State = SolverResultState.Created;
        }

        public SolverResults(string name) : this()
        {
            this.SolverName = name;
        }

        /// <summary>
        /// Notifies a solution process has begun, and begins counting elapsed time.
        /// </summary>
        public void StartSolutionProcess()
        {
            State = SolverResultState.Processing;
            this.StartTime = DateTime.Now;
            this.MyStopWatch = new Stopwatch();
            this.MyStopWatch.Start();
        }

        /// <summary>
        /// Notifies a solution process has finished.
        /// </summary>
        /// <param name="result">The result of the solution process.</param>
        /// <param name="state">The final state of the solution process.</param>
        public void StopSolutionProcess(object result, SolverResultState state)
        {
            this.Result = result;
            this.State = state;
            this.MyStopWatch.Stop();
            this.ExecutionTime = this.MyStopWatch.Elapsed;
        }

        /// <summary>
        /// Notifies that the solution process was successfully completed.
        /// </summary>
        /// <param name="result">The result of the solution process.</param>
        public void StopSuccessfulSolutionProcess(object result)
        {
            StopSolutionProcess(result, SolverResultState.Successful);
        }

        /// <summary>
        /// Notifies that the solution process was finished with an exception.
        /// </summary>
        /// <param name="e">The exception caught during the solution process.</param>
        public void StopFailedSolutionProcessWithException(Exception e)
        {
            StopSolutionProcess(null, SolverResultState.FailedWithException);
            string msg = String.Format("Solution process '{0}' failed with exception {1} after {2}.", SolverName, e.Message, ExecutionTime);
            this.SolverMessages.Add(msg);
            logger.Error(e, msg);
        }

        public override string ToString()
        {
            return String.Format("Solver '{0}' in state '{1}'. Elapsed time: {2} (start: {3}; finish: {4}).", SolverName, State, ExecutionTime, StartTime, StopTime);
        }
    }

    /// <summary>
    /// Represents the state of the result of a solver (e.g. successful, failed).
    /// </summary>
    public enum SolverResultState
    {
        Created,
        Successful,
        SuccessfulWithWarning,
        Failed,
        FailedWithException,
        Paused,
        Processing
    }
}
