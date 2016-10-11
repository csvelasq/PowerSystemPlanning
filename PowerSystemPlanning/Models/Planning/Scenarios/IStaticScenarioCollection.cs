using PowerSystemPlanning.Models.SystemBaseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Models.Planning.Scenarios
{
    public interface IStaticScenarioCollection : IHavePowerSystem
    {
        IList<IStaticScenario> MyStaticScenarios { get; }
    }
}
