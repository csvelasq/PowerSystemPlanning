using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Encapsulates configuration parameters for power system solvers.
    /// </summary>
    public class PowerSystemSolverConfiguration
    {
        private bool _SaveModelTxt;
        private bool _SaveSolutionTxt;
        private SolverVerbosity _VerbosityLevel;

        /// <summary>
        /// Indicates whether the model will be saved to a txt file
        /// </summary>
        public bool SaveModelTxt
        {
            get
            {
                return _SaveModelTxt;
            }

            set
            {
                _SaveModelTxt = value;
            }
        }

        /// <summary>
        /// Indicates whether the solution of the model will be saved to a txt file
        /// </summary>
        public bool SaveSolutionTxt
        {
            get
            {
                return _SaveSolutionTxt;
            }

            set
            {
                _SaveSolutionTxt = value;
            }
        }

        /// <summary>
        /// Indicates the verbosity level of the solution process
        /// </summary>
        public SolverVerbosity VerbosityLevel
        {
            get
            {
                return _VerbosityLevel;
            }

            set
            {
                _VerbosityLevel = value;
            }
        }
    }

    /// <summary>
    /// Defines the verbosity level of the solver
    /// </summary>
    /// <remarks>Verbosity levels are drawn from NLog's logging levels.</remarks>
    public enum SolverVerbosity
    {
        Trace,
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }
}
