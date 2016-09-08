namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Encapsulates the specific results of a "solved" optimization model.
    /// </summary>
    /// <remarks>
    /// This class is instantiated once the optimization model is solved.
    /// </remarks>
    public interface IGRBOptimizationModelResult
    {
        /// <summary>
        /// Status of the Gurobi optimization (defined in GRB.Status)
        /// </summary>
        int GRBStatus { get; set; }
        /// <summary>
        /// True if the model was proved to be infeasible, false otherwise.
        /// </summary>
        bool IsModelInfeasible { get; }
        /// <summary>
        /// True if the model was solved to optimality, false otherwise.
        /// </summary>
        bool IsModelSolved { get; }
        /// <summary>
        /// True if the model was proved to be unbounded, false otherwise.
        /// </summary>
        bool IsModelUnbounded { get; }
        /// <summary>
        /// The value of the objective function.
        /// </summary>
        double ObjVal { get; }
        /// <summary>
        /// A message indicating the current state of the model (e.g. solved, infeasible).
        /// </summary>
        string CurrentStateMessage { get; }
    }
}