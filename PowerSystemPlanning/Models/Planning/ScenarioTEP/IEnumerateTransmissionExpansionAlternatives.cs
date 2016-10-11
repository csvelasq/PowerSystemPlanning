using System.Collections.Generic;

namespace PowerSystemPlanning.Models.Planning.ScenarioTEP
{
    public interface IEnumerateTransmissionExpansionAlternatives
    {
        IStaticScenarioTepSimulationModel MyTepSimulationModel { get; }
        /// <summary>
        /// Enumerates all possible transmission expansion alternatives for the model <see cref="MyTepSimulationModel"/>.
        /// </summary>
        /// <returns>A list of all possible transmission expansion alternatives, 
        /// with costs evaluated under each possible scenario.</returns>
        /// <remarks>
        /// Expansion alternatives created and evaluated are also saved to <see cref="AllExpansionAlternatives"/>.
        /// </remarks>
        IList<IStaticTransmissionExpansionPlan> EnumerateAndEvaluateExpansionAlternatives();
    }
}
