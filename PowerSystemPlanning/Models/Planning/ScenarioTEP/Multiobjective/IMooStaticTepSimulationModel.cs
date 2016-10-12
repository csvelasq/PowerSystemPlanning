using PowerSystemPlanning.MultiObjective;

namespace PowerSystemPlanning.Models.Planning.ScenarioTEP.Multiobjective
{
    public interface IMooStaticTepSimulationModel 
        : IStaticScenarioTepSimulationModel, IMultiObjectiveOptimizationProblem
    {
    }
}
