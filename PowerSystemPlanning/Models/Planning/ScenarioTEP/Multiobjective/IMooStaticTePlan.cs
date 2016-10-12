using PowerSystemPlanning.MultiObjective;

namespace PowerSystemPlanning.Models.Planning.ScenarioTEP.Multiobjective
{
    public interface IMooStaticTePlan
        : IStaticTransmissionExpansionPlan, IMultiObjectiveIndividual
    {
        IMooStaticTepSimulationModel MyMooSimulationModel { get; }
    }
}
