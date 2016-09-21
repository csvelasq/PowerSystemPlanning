using PowerSystemPlanning.Solvers.LDCOPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels.Planning
{
    /// <summary>
    /// A transmission expansion plan, composed by a subset of the candidate transmission lines.
    /// </summary>
    /// <remarks>
    /// This class is intended to be constructed with a predefined set of transmission lines to be built (selected among all candidate transmission lines), in order to evaluate the pareto-frontier of future planning under scenario uncertainty. The performance of this transmission expansion plan under various future scenarios can be evaluated by calling <see cref="EvaluateObjectives"/>, which returns, for each scenario evaluated, to total investment costs plus the present value of total operation costs under each scenario (discounted at the rate given by <see cref="MyScenarioTEPModel"/>).
    /// Detailed results are avilable in <see cref="MyDetailedTEPScenariosResults"/> after calling <see cref="BuildDetailedTEPScenariosResults+"/>.
    /// </remarks>
    public class TransmissionExpansionPlan : IMultiObjectiveIndividual, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Example implementation from: https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ScenarioTEPModel MyScenarioTEPModel { get; protected set; }

        /// <summary>
        /// The list of candidate transmission lines which are built in this transmission expansion plan.
        /// </summary>
        public IList<CandidateTransmissionLine> BuiltTransmissionLines { get; set; }
        /// <summary>
        /// Gets a list of the names of built transmission lines.
        /// </summary>
        public string BuiltTransmissionLinesNames
        {
            get
            {
                string s = "[";
                for (int i = 0; i < BuiltTransmissionLines.Count; i++)
                {
                    var t = BuiltTransmissionLines[i];
                    s += t.Name;
                    if (i < BuiltTransmissionLines.Count - 1)
                        s += ",";

                }
                return s + "]";
            }
        }
        /// <summary>
        /// The total investment cost (in MMUS$) incurred by implementing this transmission expansion plan.
        /// </summary>
        /// <remarks>Equals the sum of investment costs of each transmission line in this expansion plan.</remarks>
        public double TotalInvestmentCost
        {
            get { return (from tl in BuiltTransmissionLines select tl.InvestmentCost).Sum() / 1e3; }
        }

        /// <summary>
        /// The LDC OPF model for simulating the implementation of this expansion plan under each scenario.
        /// </summary>
        public List<LDCOPFModel> MyLDCOPFModelEachScenario { get; protected set; }

        /// <summary>
        /// The total operation costs (in MMUS$) with this transmission expansion plan in each scenario.
        /// </summary>
        /// <remarks>Total operation cost under each scenario is calculated by simulating the LDC OPF in each scenario, considering that the transmission lines in this expansion plan are operational. This variable is set by the <see cref="EvaluateObjectives"/> function.</remarks>
        public List<double> ScenariosOperationCosts { get; protected set; }
        /// <summary>
        /// The present value of total costs (investment plus operation) under each scenario.
        /// </summary>
        public List<double> PresentValueScenariosTotalCosts { get; protected set; }
        /// <summary>
        /// Each objective function is the PV of total costs (investment plus operation) under each scenario.
        /// </summary>
        public List<double> ObjectiveValues { get { return PresentValueScenariosTotalCosts; } }
        /// <summary>
        /// Expected total costs (investment plus operation), present value and weighted by scenario probabilities.
        /// </summary>
        public double ExpectedTotalCosts { get; protected set; }

        /// <summary>
        /// The detailed results of implementing this transmission expansion plan under each scenario simulated.
        /// </summary>
        public TransmissionExpansionPlanScenarioDetailedResults MyDetailedTEPScenariosResults { get; protected set; }

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
            EvaluateScenarios(false);
            return ObjectiveValues;
        }

        /// <summary>
        /// Evaluates the operation costs under several scenarios after implementing this transmission expansion plan.
        /// </summary>
        /// <param name="buildDetailedResults">Indicates whether detailed results are built and reported.</param>
        /// <returns>An object that encapsulates detailed results for the TEP under several scenarios if buildDetailedResults=true; null otherwise.</returns>
        public TransmissionExpansionPlanScenarioDetailedResults EvaluateScenarios(bool buildDetailedResults)
        {
            //Initialization
            ScenariosOperationCosts = new List<double>();
            PresentValueScenariosTotalCosts = new List<double>();
            ExpectedTotalCosts = 0;
            double pvFactor = MyScenarioTEPModel.OperationPresentValueFactor;
            double totalInvestmentCost = TotalInvestmentCost;
            //Build and solve the LDC OPF model under each scenario
            MyLDCOPFModelEachScenario = new List<LDCOPFModel>();
            foreach (PowerSystemScenario scenarioToEval in MyScenarioTEPModel.MyScenarios)
            {
                //evaluate scenario operation costs
                double opC = EvaluateScenarioOperationCosts(scenarioToEval, buildDetailedResults) / 1e6; //must convert from US$ to MMUS$
                ScenariosOperationCosts.Add(opC);
                //scenario total costs
                double totC = pvFactor * opC + totalInvestmentCost;
                PresentValueScenariosTotalCosts.Add(totC);
                //expected total costs
                ExpectedTotalCosts += totC * scenarioToEval.Probability;
            }
            //Notify Property Changes
            // TODO implement in each property
            NotifyPropertyChanged("ScenariosOperationCosts");
            NotifyPropertyChanged("PresentValueScenariosTotalCosts");
            NotifyPropertyChanged("ObjectiveValues");
            NotifyPropertyChanged("ExpectedTotalCosts");
            //Results
            if (buildDetailedResults)
            {
                return BuildDetailedTEPScenariosResults();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Evaluates the total operation costs for this transmission expansion plan under a single scenario.
        /// </summary>
        /// <param name="scenarioToEval">The scenario under which operation will be evaluated.</param>
        /// <returns>The total operation costs (by LDC OPF) in this scenario and implementing this expansion plan.</returns>
        public double EvaluateScenarioOperationCosts(PowerSystemScenario scenarioToEval, bool buildDetailedResults)
        {
            //builds the model
            PowerSystemWithCandidateTransmissionLines MyPowerSystemWithCandidateTransmissionLines = new PowerSystemWithCandidateTransmissionLines(scenarioToEval.MyPowerSystem, BuiltTransmissionLines);
            LDCOPFModel model = new LDCOPFModel(MyPowerSystemWithCandidateTransmissionLines, MyScenarioTEPModel.MyLoadDurationCurve);
            MyLDCOPFModelEachScenario.Add(model);
            // TODO implement an OPF optimization model where single parameters can be modified (instead of rebuilding the whole model on each call) via GRB.ChgCoeff(constr,var,newvalue)
            if (buildDetailedResults)
            {
                model.BuildGRBOptimizationModel();
                model.OptimizeGRBModel();
                model.BuildGRBOptimizationModelResults();
                model.BuildLDCOPFModelResults();
                model.Dispose();
            }
            else
            {
                model.BuildSolveAndDisposeModel();
            }
            return model.MyBaseGRBOptimizationModelResult.ObjVal;
        }

        /// <summary>
        /// Constructs detailed results for this transmission expansion plan evaluated under several scenarios.
        /// </summary>
        /// <returns></returns>
        public TransmissionExpansionPlanScenarioDetailedResults BuildDetailedTEPScenariosResults()
        {
            MyDetailedTEPScenariosResults = new TransmissionExpansionPlanScenarioDetailedResults(this);
            return MyDetailedTEPScenariosResults;
        }
    }
}
