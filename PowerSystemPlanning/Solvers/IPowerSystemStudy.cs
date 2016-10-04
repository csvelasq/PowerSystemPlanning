using PowerSystemPlanning.Models.SystemBaseData;

namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// An instance of a generic study applied to a particular power system
    /// </summary>
    /// <remarks>
    /// A study is a particular instance of some analysis undertaken in a given power system (e.g. an instance of an OPF). 
    /// 
    /// Implementations of this class are meant to encapsulate:
    ///     1. The definition of the generic study (its generic name, e.g. "OPF")
    ///     2. The definition of this instance of the study, particularly the input data (based on <see cref="MyPowerSystem"/>)
    ///     2. The results of this instance of the study, encapsulated by <see cref="MySolverResults"/> and other particular objects as defined by implementing studies.
    /// </remarks>
    public interface IPowerSystemStudy
    {
        /// <summary>
        /// The power system for which this solver is applied.
        /// </summary>
        IPowerSystem MyPowerSystem { get; }
        /// <summary>
        /// The (generic) name of the underlying solver for this study (e.g. "OPF").
        /// </summary>
        string StudyGenericName { get; }
        /// <summary>
        /// The name of this study (e.g. "OPF peak demand 2020").
        /// </summary>
        string StudyInstanceName { get; set; }
        /// <summary>
        /// Solves the underlying problem involved in this study (synchronous)
        /// </summary>
        void Solve();
        /// <summary>
        /// Solves the underlying problem involved in this study (asynchronously).
        /// </summary>
        void SolveAsync();
        /// <summary>
        /// Gets the results of the solution process.
        /// </summary>
        /// <returns>An object encapsulating results of the solution process (e.g. execution time, result).</returns>
        SolverResults MySolverResults { get; }

        /// <summary>
        /// Saves this study's input data and output results to an XML file
        /// </summary>
        /// <param name="fullPath">The full file-path to which this study will be saved.</param>
        void SaveToXml(string fullPath);
    }
}
