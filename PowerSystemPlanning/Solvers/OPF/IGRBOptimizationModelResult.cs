namespace PowerSystemPlanning.Solvers.OPF
{
    /// <summary>
    /// Encapsulates the results of a "solved" optimization model.
    /// </summary>
    /// <remarks>
    /// This class is instantiated once the optimization model is solved.
    /// </remarks>
    public interface IGRBOptimizationModelResult
    {
        int GRBStatus { get; set; }
        bool IsModelInfeasible { get; }
        bool IsModelSolved { get; }
        bool IsModelUnbounded { get; }
        /// <summary>
        /// The value of the objective function.
        /// </summary>
        double ObjVal { get; }
    }
}