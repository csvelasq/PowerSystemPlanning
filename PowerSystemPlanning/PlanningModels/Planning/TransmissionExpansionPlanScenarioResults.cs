using PowerSystemPlanning.Solvers.LDCOPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels.Planning
{
    /// <summary>
    /// Container of the results of evaluating the implementation of a transmission expansion plan in a set of possible future scenarios.
    /// </summary>
    /// <remarks>This is meant to be a rather static object, set after evaluating an expansion plan and never changed later. It includes each step of the calculation of the PV of expected total (op+investment) costs; and also detailed results of the evaluation under each scenario.</remarks>
    public class TransmissionExpansionPlanScenarioDetailedResults
    {
        public TransmissionExpansionPlan MyTransmissionExpansionPlan { get; protected set; }

        public List<TransmissionExpansionPlanLDCResultsForOneScenario> MyDetailedTEPResultsForEachScenario { get; protected set; }

        /// <summary>
        /// The present value of expected total costs (investment plus operation).
        /// </summary>
        public double PVExpectedTotalCosts
        {
            get
            {
                return PVExpectedOperationCosts + MyTransmissionExpansionPlan.TotalInvestmentCost;
            }
        }
        /// <summary>
        /// The present value of expected operation costs
        /// </summary>
        public double PVExpectedOperationCosts
        {
            get
            {
                return (from x in MyDetailedTEPResultsForEachScenario select x.ProbabilityWeightedPVScenarioOperationCosts).Sum();
            }
        }
        /// <summary>
        /// The future value of expected operation costs (probability-weighted sum of total operation costs across scenarios).
        /// </summary>
        public double FVExpectedOperationCosts
        {
            get
            {
                return (from x in MyDetailedTEPResultsForEachScenario select x.ProbabilityWeightedScenarioOperationCosts).Sum();
            }
        }
        /// <summary>
        /// The future value of total operation costs across all scenarios.
        /// </summary>
        public double FVTotalOperationCosts
        {
            get
            {
                return (from x in MyDetailedTEPResultsForEachScenario select x.ScenarioOperationCosts).Sum();
            }
        }

        public TransmissionExpansionPlanScenarioDetailedResults(TransmissionExpansionPlan tePlan)
        {
            MyTransmissionExpansionPlan = tePlan;
            // Builds detailed results
            MyDetailedTEPResultsForEachScenario = new List<TransmissionExpansionPlanLDCResultsForOneScenario>();
            double invCost = MyTransmissionExpansionPlan.TotalInvestmentCost;
            for (int s = 0; s < MyTransmissionExpansionPlan.MyScenarioTEPModel.MyScenarios.Count; s++)
            {
                //foreach scenario
                var t = new TransmissionExpansionPlanLDCResultsForOneScenario(
                    MyTransmissionExpansionPlan.MyScenarioTEPModel.MyScenarios[s], 
                    invCost, 
                    MyTransmissionExpansionPlan.ScenariosOperationCosts[s], 
                    MyTransmissionExpansionPlan.DiscountFactor, 
                    MyTransmissionExpansionPlan.MyLDCOPFModelEachScenario[s].MyDetailedLDCOPFModelResults
                    );
                MyDetailedTEPResultsForEachScenario.Add(t);
            }
        }
    }

    /// <summary>
    /// Container of the results of evaluating the implementation of a transmission expansion plan in a single scenario.
    /// </summary>
    /// <remarks>This is meant to be a rather static object, set after evaluating an scenario and never changed later. It includes each step of the calculation of the PV of total (op+investment) costs; and also probability weighted (by the probability of occurrence of a single scenario).</remarks>
    public class TransmissionExpansionPlanLDCResultsForOneScenario
    {
        /// <summary>
        /// The scenario under which the implementation of the transmission expansion plan was evaluated
        /// </summary>
        public PowerSystemScenario ScenarioEvaluated { get; protected set; }
        /// <summary>
        /// The total investment cost (in MMUS$) incurred by implementing this transmission expansion plan.
        /// </summary>
        /// <remarks>Equals the sum of investment costs of each transmission line in this expansion plan.</remarks>
        public double TotalInvestmentCost { get; protected set; }
        /// <summary>
        /// The total operation costs (in MMUS$) with this transmission expansion plan in each scenario.
        /// </summary>
        /// <remarks>Total operation cost under each scenario is calculated by simulating the LDC OPF in each scenario, considering that the transmission lines in this expansion plan are operational. This variable is set by the <see cref="EvaluateObjectives"/> function.</remarks>
        public double ScenarioOperationCosts { get; protected set; }
        /// <summary>
        /// The detailed LDC OPF model results for this scenario.
        /// </summary>
        public LDCOPFModelResults DetailedLDCOPFModelResults { get; protected set; }

        /// <summary>
        /// The present worth of the operation costs under the evaluated scenario.
        /// </summary>
        public double PresentValueScenarioOperationCosts { get; protected set; }
        /// <summary>
        /// The present value of total costs (investment plus operation) under the evaluated scenario.
        /// </summary>
        public double PresentValueScenarioTotalCosts { get; protected set; }

        public double ProbabilityWeightedScenarioOperationCosts { get; protected set; }
        /// <summary>
        /// The present value of operation costs under the evaluated scenario, weighted by the probability of occurrence of the scenario.
        /// </summary>
        public double ProbabilityWeightedPVScenarioOperationCosts { get; protected set; }
        /// <summary>
        /// The present value of total costs (investment plus operation) under the evaluated scenario, weighted by the probability of occurrence of the scenario.
        /// </summary>
        public double ProbabilityWeightedPVScenarioTotalCosts { get; protected set; }

        public TransmissionExpansionPlanLDCResultsForOneScenario() { }

        public TransmissionExpansionPlanLDCResultsForOneScenario(PowerSystemScenario scenarioEvaluated, double totalInvestmentCost, double scenarioOperationCosts, double discountFactor, LDCOPFModelResults opfModelResults)
        {
            ScenarioEvaluated = scenarioEvaluated;
            TotalInvestmentCost = totalInvestmentCost;
            // TODO build these detailed results
            DetailedLDCOPFModelResults = opfModelResults;
            ScenarioOperationCosts = scenarioOperationCosts;
            PresentValueScenarioOperationCosts = discountFactor * scenarioOperationCosts;
            PresentValueScenarioTotalCosts = TotalInvestmentCost + PresentValueScenarioOperationCosts;
            //probability weighted results
            ProbabilityWeightedScenarioOperationCosts = ScenarioEvaluated.Probability * ScenarioOperationCosts;
            ProbabilityWeightedPVScenarioOperationCosts = ScenarioEvaluated.Probability * PresentValueScenarioOperationCosts;
            ProbabilityWeightedPVScenarioTotalCosts = TotalInvestmentCost + ProbabilityWeightedPVScenarioOperationCosts;
        }
    }
}
