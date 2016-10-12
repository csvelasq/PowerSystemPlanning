using PowerSystemPlanning.Models.Planning.ScenarioTEP.Multiobjective;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemPlanning.Models.Planning.ExpansionState.Tep;
using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using PowerSystemPlanning.MultiObjective;
using PowerSystemPlanning.Models.SystemState.Branch;
using PowerSystemPlanning.Solvers.LDCOPF;
using PowerSystemPlanning.Models.Planning.InvestmentBranch;

namespace PowerSystemPlanning.Solvers.ScenarioTEP
{
    public class MooStaticTepSimulationModel : IMooStaticTepSimulationModel
    {
        public IStaticScenarioTepModel MyTepModelDefinition { get; protected set; }
        public List<ObjectiveFunctionInOptimizationProblem> MyObjectiveFunctionsDefinition { get; protected set; }

        public MooStaticTepSimulationModel(IStaticScenarioTepModel tepModelSetup)
        {
            MyTepModelDefinition = tepModelSetup;
            DefineObjectiveFunctions();
        }

        private void DefineObjectiveFunctions()
        {
            MyObjectiveFunctionsDefinition = new List<ObjectiveFunctionInOptimizationProblem>();
            foreach (var scenario in MyTepModelDefinition.MyScenarios.MyStaticScenarios)
            {
                var objectiveFunction = new ObjectiveFunctionInOptimizationProblem()
                {
                    MyObjectiveName = $"Total cost '{scenario.Name}'",
                    MySense = OptimizationSense.Minimize
                };
                MyObjectiveFunctionsDefinition.Add(objectiveFunction);
            }
        }

        public List<double> EvaluateScenarioOperationCosts(IList<ICandidateTransmissionLineState> candidateLinesStates)
        {
            var scenariosOperationCosts = new List<double>();
            foreach (var scenario in MyTepModelDefinition.MyScenarios.MyStaticScenarios)
            {
                var scenarioOperationCost = 0.0;
                //Build LDC OPF model for this scenario
                foreach (var state in scenario.MyStateCollection.MyPowerSystemStates)
                {
                    for (int l = 0; l < candidateLinesStates.Count; l++)
                    {
                        //In each state of each scenario, deactivates transmission lines 
                        //which haven't been built
                        state.SimpleTransmissionLineStates[l].IsAvailable =
                            candidateLinesStates[l].IsBuilt;
                    }
                }
                var ldcOpfModel = new LdcOpfModel(scenario.MyStateCollection.MyPowerSystemStates);
                //Solve LDC OPF model and save result
                ldcOpfModel.BuildSolveAndDisposeModel();
                scenarioOperationCost = ldcOpfModel.MyBaseGRBOptimizationModelResult.ObjVal;
                scenariosOperationCosts.Add(scenarioOperationCost);
            }
            return scenariosOperationCosts;
        }

        public IList<ICandidateTransmissionLineState> BuildNewCandidateTransmissionLinesStates(IList<ICandidateTransmissionLine> candidateLines)
        {
            var candidateLinesStates = new List<ICandidateTransmissionLineState>();
            foreach (var candidateLine in candidateLines)
            {
                var candidateLineState = MyTepModelDefinition.CreateCandidateLineState(candidateLine);
                candidateLinesStates.Add(candidateLineState);
            }
            return candidateLinesStates;
        }
    }
}
