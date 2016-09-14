using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels.Planning
{
    /// <summary>
    /// Encapsulates power system data in a particular future scenario being modeled.
    /// </summary>
    public class PowerSystemScenario
    {
        /// <summary>
        /// The name of this scenario.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The power system data for this scenario
        /// </summary>
        public PowerSystem MyPowerSystem { get; set; }

        public PowerSystemScenario()
        {
            Name = "";
            MyPowerSystem = new PowerSystem();
        }

        public PowerSystemScenario(string name, PowerSystem scenarioPowerSystem)
        {
            Name = name;
            MyPowerSystem = scenarioPowerSystem;
        }
    }
}
