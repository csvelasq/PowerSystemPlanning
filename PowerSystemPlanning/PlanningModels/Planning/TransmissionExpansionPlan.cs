using PowerSystemPlanning.Solvers.LDCOPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels.Planning
{
    /// <summary>
    /// A transmission expansion plan, composed by a subset of the candidate transmission lines.
    /// </summary>
    public class TransmissionExpansionPlan : IMultiObjectiveIndividual
    {
        protected ScenarioTEPModel MyScenarioTEPModel;

        /// <summary>
        /// The list of candidate transmission lines which are built in this transmission expansion plan.
        /// </summary>
        public IList<CandidateTransmissionLine> BuiltTransmissionLines { get; set; }
        /// <summary>
        /// The total investment cost (in MMUS$) incurred by implementing this transmission expansion plan.
        /// </summary>
        /// <remarks>Equals the sum of investment costs of each transmission line in this expansion plan.</remarks>
        public double TotalInvestmentCost
        {
            get { return (from tl in BuiltTransmissionLines select tl.InvestmentCost).Sum(); }
        }
        /// <summary>
        /// The total operation costs (in MMUS$) with this transmission expansion plan in each scenario.
        /// </summary>
        /// <remarks>Total operation cost under each scenario is calculated by simulating the LDC OPF in each scenario, considering that the transmission lines in this expansion plan are operational. This variable is set by the <see cref="EvaluateObjectives"/> function.</remarks>
        public List<double> ScenariosOperationCosts { get; protected set; }
        /// <summary>
        /// The factor by which operation costs must be multiplied to obtain present-worth.
        /// </summary>
        /// <remarks>Equals \f$ \frac{1}{(1+r)^N} \f$ where r is the yearly discount rate, and N is the target planning year (with the investment year being N=0).</remarks>
        public double DiscountFactor
        {
            get
            {
                return 1 / Math.Pow(1 + MyScenarioTEPModel.YearlyDiscountRate, MyScenarioTEPModel.TargetPlanningYear);
            }
        }
        /// <summary>
        /// The present worth of the operation costs under each scenario.
        /// </summary>
        public List<double> PresentWorthScenariosOperationCosts
        {
            get
            {
                return (from c in ScenariosOperationCosts select c * DiscountFactor).ToList<double>();
            }
        }
        /// <summary>
        /// The present worth of total costs (investment plus operation) under each scenario.
        /// </summary>
        public List<double> PresentWorthScenariosTotalCosts
        {
            get
            {
                double totalInvestmentCost = TotalInvestmentCost;
                return (from c in ScenariosOperationCosts select (c * DiscountFactor + totalInvestmentCost)).ToList<double>();
            }
        }
        public List<double> ObjectiveValues { get { return PresentWorthScenariosTotalCosts; } }

        /// <summary>
        /// Constructor of transmission expansion plan.
        /// </summary>
        /// <param name="tlsInPlan">Transmission lines to be built in this expansion plan.</param>
        /// <param name="yearlyDiscountRate">The yearly discount rate.</param>
        /// <param name="targetPlanningYear">The target planning year (with the investment year being N=0)</param>
        public TransmissionExpansionPlan(IList<CandidateTransmissionLine> tlsInPlan, ScenarioTEPModel myScenarioTEPModel)
        {
            BuiltTransmissionLines = tlsInPlan;
            MyScenarioTEPModel = myScenarioTEPModel;
        }

        public List<double> EvaluateObjectives()
        {
            ScenariosOperationCosts = new List<double>();
            foreach (PowerSystemScenario scenarioToEval in MyScenarioTEPModel.MyScenarios)
            {
                ScenariosOperationCosts.Add(EvaluateScenarioOperationCosts(scenarioToEval));
            }
            return ObjectiveValues;
        }

        /// <summary>
        /// Evaluates the total operation costs for this transmission expansion plan under a single scenario.
        /// </summary>
        /// <param name="scenarioToEval">The scenario under which operation will be evaluated.</param>
        /// <returns>The total operation costs (by LDC OPF) in this scenario and implementing this expansion plan.</returns>
        public double EvaluateScenarioOperationCosts(PowerSystemScenario scenarioToEval)
        {
            //builds the model
            LDCOPFModel MyLDCOPFModel = new LDCOPFModel(scenarioToEval.MyPowerSystem, MyScenarioTEPModel.MyLoadDurationCurve);
            // TODO implement an OPF optimization model where single parameters can be modified (instead of rebuilding the whole model on each call) via GRB.ChgCoeff(constr,var,newvalue)
            MyLDCOPFModel.BuildSolveAndDisposeModel();
            return MyLDCOPFModel.MyBaseGRBOptimizationModelResult.ObjVal;
        }
    }
}
