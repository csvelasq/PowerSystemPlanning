using PowerSystemPlanning.BindingModels.PlanningBinding.BindingInvestmentBranch;
using PowerSystemPlanning.Models.Planning.ExpansionState.Tep;
using PowerSystemPlanning.Models.Planning.InvestmentBranch;
using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using PowerSystemPlanning.Models.Planning.ScenarioTEP.Multiobjective;
using PowerSystemPlanning.MultiObjective;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemPlanning.Models.SystemBaseData;

namespace PowerSystemPlanning.Solvers.ScenarioTEP.BruteForcePareto
{
    /// <summary>
    /// Builds the Pareto front for the MOO TEP under scenarios.
    /// </summary>
    /// <remarks>
    /// To use quickly, construct providing an <see cref="IMooStaticTepSimulationModel"/>,
    /// and then call <see cref="BuildAlternativesInParetoFront"/> with argument 'true'.
    /// </remarks>
    public class TepScenariosParetoFrontBruteForceBuilder
        : IBuildTepUnderScenariosParetoFrontBruteForce
    {
        #region Internal fields
        public IMooStaticTepSimulationModel MyMooTepSimulationModel { get; protected set; }
        public IStaticScenarioTepSimulationModel MyTepSimulationModel => MyMooTepSimulationModel;
        public List<MooStaticTePlan> AllTransmissionExpansionAlternatives { get; protected set; }
        public BaseMultiObjectiveIndividualList AllAlternativesIndividuals { get; protected set; }
        public List<MooStaticTePlan> EfficientTransmissionExpansionAlternatives { get; protected set; }
        #endregion

        public ulong TransmissionExpansionPlansCount =>
            (ulong)(1 << MyTepSimulationModel.MyTepModelDefinition.MyCandidateTransmissionLines.Count);

        public TepScenariosParetoFrontBruteForceBuilder()
        {

        }

        public TepScenariosParetoFrontBruteForceBuilder(IMooStaticTepSimulationModel myTepModel)
        {
            MyMooTepSimulationModel = myTepModel;
        }

        public IList<IStaticTransmissionExpansionPlan> EnumerateAndEvaluateExpansionAlternatives()
        {
            AllTransmissionExpansionAlternatives = new List<MooStaticTePlan>();
            //Build the power set of candidate transmission lines
            var powerSetExpansionPlans = MixedUtils.GetPowerSet<ICandidateTransmissionLine>(MyTepSimulationModel.MyTepModelDefinition.MyCandidateTransmissionLines);
            foreach (var linesToBuild in powerSetExpansionPlans)
            {
                //Translates the set of lines to build, to a set of candidate lines states
                var candidateLinesStates = MyTepSimulationModel.BuildNewCandidateTransmissionLinesStates(linesToBuild.ToList());
                //Create and append new expansion alternative
                var expansionAlternative = new MooStaticTePlan(MyMooTepSimulationModel, candidateLinesStates);
                AllTransmissionExpansionAlternatives.Add(expansionAlternative);
                //Evaluate the newly create expansion alternative
                expansionAlternative.EvaluateScenariosTotalCosts();
            }
            return AllTransmissionExpansionAlternatives.Cast<IStaticTransmissionExpansionPlan>().ToList();
        }

        public IList<IStaticTransmissionExpansionPlan> BuildAlternativesInParetoFront()
        {
            //Build pareto front with BaseMultiObjectiveIndividualList
            AllAlternativesIndividuals = new BaseMultiObjectiveIndividualList(MyMooTepSimulationModel, AllTransmissionExpansionAlternatives.Cast<IMultiObjectiveIndividual>().ToList());
            var efficientAlternatives = AllAlternativesIndividuals.BuildParetoEfficientFrontier();
            //Transform the set of efficient alternatives to a strongly typed list
            EfficientTransmissionExpansionAlternatives =
                (from alternative in efficientAlternatives
                 select (alternative as MooStaticTePlan)).ToList();
            /* Note that 'efficientAlternatives' is a subset of 'AllTransmissionExpansionAlternatives'
             * which is List<MooStaticTePlan>
             * Then 'alternative' is effectively of type MooStaticTePlan
             */
            // TODO Generic implementation of the method for building pareto front, so as to avoid casting with the 'as' operator
            return EfficientTransmissionExpansionAlternatives.Cast<IStaticTransmissionExpansionPlan>().ToList();
        }

        public IList<IStaticTransmissionExpansionPlan> BuildAlternativesInParetoFront(bool enumerate)
        {
            if (enumerate)
            {
                EnumerateAndEvaluateExpansionAlternatives();
            }
            return BuildAlternativesInParetoFront();
        }
    }
}
