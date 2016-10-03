using PowerSystemPlanning.MultiObjective;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Models.Planning.ScenarioTEP
{
    public class MOOScenarioTEP : BaseMultiObjectiveOptimizationProblem
    {
        public override string MyMOOName => "TEP under MOO Scenario uncertainty (one objective for each scenario)";

        public ScenarioTEPModel MyScenarioTEPModel { get; protected set; }

        public MOOScenarioTEP(ScenarioTEPModel myScenarioTEPModel)
        {
            MyScenarioTEPModel = myScenarioTEPModel;
            MyObjectiveFunctionsDefinition = new List<ObjectiveFunctionInOptimizationProblem>();
            foreach (var scenario in MyScenarioTEPModel.MyScenarios)
            {
                MyObjectiveFunctionsDefinition.Add(new ObjectiveFunctionInOptimizationProblem
                {
                    MySense = OptimizationSense.Minimize,
                    MyObjectiveName = String.Format("Total cost in scenario '{0}'", scenario.Name)
                });
            }
        }
    }
}
