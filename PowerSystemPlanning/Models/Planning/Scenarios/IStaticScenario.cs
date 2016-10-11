using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemState;

namespace PowerSystemPlanning.Models.Planning.Scenarios
{
    public interface IStaticScenario : IHavePowerSystem
    {
        IPowerSystemStateCollection MyStateCollection { get; }
        string Name { get; set; }
        double Probability { get; set; }
    }
}