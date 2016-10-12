using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Models.Planning.ScenarioTEP
{
    public interface IBuildTepUnderScenariosParetoFrontBruteForce
        : IEnumerateTransmissionExpansionAlternatives, IBuildTepUnderScenariosParetoFront
    {
    }
}
