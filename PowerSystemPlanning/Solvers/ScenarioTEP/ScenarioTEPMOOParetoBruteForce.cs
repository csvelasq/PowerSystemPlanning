using NLog;
using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using PowerSystemPlanning.MultiObjective;
using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.PlanningModels.Planning;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.ScenarioTEP
{
    /// <summary>
    /// Builds, by brute force, the pareto frontier of a transmission expansion planning problem under scenarios.
    /// </summary>
    /// <remarks>
    /// All possible alternative expansion plans are enumerated and evaluated under each possible future scenario.
    /// Then, the pareto frontier is built by comparing the whole set of possible expansion plans across the costs under every scenario.
    /// Hence, this method is extremely inefficient.
    /// 
    /// To use this class, construct providing a scenario TEP model and then call <see cref="Solve"/>.
    /// That method will enumerate, evaluate all alternatives, and then construct the pareto frontier.
    /// </remarks>
    public class ScenarioTEPMOOParetoBruteForce : IPowerSystemSolver
    {
        /// <summary>
        /// NLog Logger for this solver.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ScenarioTEPModel MyScenarioTEPModel { get; protected set; }

        public MOOScenarioTEP MyTEPMOOProblem { get; set; }

        public BaseMultiObjectiveIndividualList MOOAllPossibleTEPAlternatives { get; protected set; }

        public BaseMultiObjectiveIndividualList ParetoFront { get; protected set; }

        /// <summary>
        /// Encapsulator of the overall results of the solution process (e.g. elapsed time).
        /// </summary>
        public SolverResults MySolverResults { get; protected set; }

        public ScenarioTEPMOOParetoBruteForce(ScenarioTEPModel myScenarioTEPModel)
        {
            MyScenarioTEPModel = myScenarioTEPModel;
            MyTEPMOOProblem = new MOOScenarioTEP(myScenarioTEPModel);
        }

        public void Solve()
        {
            // Initializes result reporting
            this.MySolverResults = new SolverResults(this.MyTEPMOOProblem.MyMOOName);
            this.MySolverResults.StartSolutionProcess();
            // Builds the model
            //try
            //{
                List<TransmissionExpansionPlan> AllPossibleTEPAlternatives = MyScenarioTEPModel.EnumerateAlternativeTransmissionExpansionPlans();
                foreach (var alternative in AllPossibleTEPAlternatives)
                {
                    alternative.EvaluateObjectives();
                }
                MOOAllPossibleTEPAlternatives = new BaseMultiObjectiveIndividualList(MyTEPMOOProblem);
                MOOAllPossibleTEPAlternatives.AddRange(AllPossibleTEPAlternatives);
                AllPossibleTEPAlternatives = null;
                // Solves the model
                ParetoFront = MOOAllPossibleTEPAlternatives.BuildParetoEfficientFrontier();
            //}
            //catch (Exception e)
            //{
            //    this.MySolverResults.StopFailedSolutionProcessWithException(e);
            //    return;
            //}
            // Finalizes result reporting
            this.MySolverResults.StopSuccessfulSolutionProcess(ParetoFront);
        }

        public void SolveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
