namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Required methods of a power system solver tool (e.g. optimal power flow, transmission expansion planning)
    /// </summary>
    public interface IPowerSystemSolver
    {
        /// <summary>
        /// Solves the problem (synchronous). Results can be retrieved with getResults()
        /// </summary>
        void Solve();
        /// <summary>
        /// Solves the problem (asynchronously).
        /// </summary>
        void SolveAsync();
        /// <summary>
        /// Gets the results of the solution process.
        /// </summary>
        /// <returns>An object encapsulating results of the solution process (e.g. execution time, result).</returns>
        SolverResults MySolverResults { get; }
    }
}
