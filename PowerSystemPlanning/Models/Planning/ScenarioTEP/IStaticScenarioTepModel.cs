using PowerSystemPlanning.Models.Planning.ExpansionState.Tep;
using PowerSystemPlanning.Models.Planning.InvestmentBranch;
using PowerSystemPlanning.Models.Planning.Scenarios;
using System.Collections.Generic;

namespace PowerSystemPlanning.Models.Planning.ScenarioTEP
{
    /// <summary>
    /// A generic model for static transmission expansion planning.
    /// </summary>
    public interface IStaticScenarioTepModel
    {
        /// <summary>
        /// The set of candidate transmission lines that can be built.
        /// </summary>
        IList<ICandidateTransmissionLine> MyCandidateTransmissionLines { get; }
        /// <summary>
        /// Set of scenarios for assessing transmission expansion plans.
        /// </summary>
        IStaticScenarioCollection MyScenarios { get; }
        /// <summary>
        /// Adimensional factor that multiplies operation costs in objective function (e.g. NPV factor).
        /// </summary>
        double OperationCostsMultiplierInObjectiveFunction { get; }
        /// <summary>
        /// Adimensional factor that multiplies investment costs in objective function.
        /// </summary>
        double InvestmentCostsMultiplierInObjectiveFunction { get; }
    }
}
