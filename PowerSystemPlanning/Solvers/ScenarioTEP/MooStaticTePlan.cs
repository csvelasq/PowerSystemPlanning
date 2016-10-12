using PowerSystemPlanning.Models.Planning.ScenarioTEP.Multiobjective;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemPlanning.Models.Planning.ExpansionState.Tep;
using PowerSystemPlanning.Models.Planning.InvestmentBranch;
using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using PowerSystemPlanning.MultiObjective;

namespace PowerSystemPlanning.Solvers.ScenarioTEP
{
    public class MooStaticTePlan : BaseMultiObjectiveIndividual, IMooStaticTePlan
    {
        public IMooStaticTepSimulationModel MyMooSimulationModel { get; protected set; }

        #region internals and interface implementation
        public IStaticScenarioTepSimulationModel MyTepModel => MyMooSimulationModel;

        public IList<ICandidateTransmissionLineState> CandidateTransmissionLineStates { get; protected set; }
        public IList<ICandidateTransmissionLine> BuiltTransmissionLines =>
            (from line in CandidateTransmissionLineStates
             where line.IsBuilt
             select line.UnderlyingCandidateTransmissionLine)
            .ToList();

        public string BuiltTransmissionLinesNames
        {
            get
            {
                string names = "";
                if (BuiltTransmissionLines.Count > 0)
                {
                    foreach (var builtLine in BuiltTransmissionLines)
                        names += builtLine.UnderlyingSimpleTransmissionLine.Name + ",";
                    names = names.Substring(0, names.Length - 1); //removes last comma
                }
                return $"[{names}]";
            }
        }

        public ulong PlanId
        {
            get
            {
                ulong id = 0;
                for (int l = 0; l < CandidateTransmissionLineStates.Count; l++)
                {
                    var lineState = CandidateTransmissionLineStates[l];
                    id += (ulong)(lineState.IsBuilt ? 1 : 0) << l;
                }
                return id;
            }
        }

        public double TotalInvestmentCost =>
            (from line in BuiltTransmissionLines
             select line.InvestmentCost).Sum();

        public List<double> ScenariosOperationCosts { get; protected set; }

        public List<double> ScenariosTotalCosts =>
            (from cost in ScenariosOperationCosts
             select cost + TotalInvestmentCost).ToList();

        public double ExpectedTotalCosts
        {
            get
            {
                double expectedCost = TotalInvestmentCost;
                var scenarioCosts = ScenariosOperationCosts;
                for (int s = 0; s < MyTepModel.MyTepModelDefinition.MyScenarios.MyStaticScenarios.Count; s++)
                {
                    expectedCost += scenarioCosts[s] *
                        MyTepModel.MyTepModelDefinition.MyScenarios.MyStaticScenarios[s].Probability;
                }
                return expectedCost;
            }
        }

        public override List<double> ObjectiveValues => ScenariosTotalCosts;
        #endregion

        public MooStaticTePlan(IMooStaticTepSimulationModel problem) : base(problem)
        {
            MyMooSimulationModel = problem;
        }

        public MooStaticTePlan(IMooStaticTepSimulationModel problem, IList<ICandidateTransmissionLineState> candidateLinesStates)
            : this(problem)
        {
            CandidateTransmissionLineStates = candidateLinesStates;
        }

        public List<double> EvaluateScenariosTotalCosts()
        {
            ScenariosOperationCosts = MyTepModel.EvaluateScenarioOperationCosts(CandidateTransmissionLineStates);
            return ScenariosTotalCosts;
        }

        public override List<double> EvaluateObjectives() => EvaluateScenariosTotalCosts();
    }
}
