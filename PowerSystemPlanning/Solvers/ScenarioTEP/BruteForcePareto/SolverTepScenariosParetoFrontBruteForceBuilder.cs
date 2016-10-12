using PowerSystemPlanning.Models.Planning.ScenarioTEP.Multiobjective;
using PowerSystemPlanning.Models.SystemBaseData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.ScenarioTEP.BruteForcePareto
{
    /// <summary>
    /// Wrapper class for <see cref="TepScenariosParetoFrontBruteForceBuilder"/>.
    /// </summary>
    /// <remarks>
    /// Call <see cref="Solve"/> and inspect results in <see cref="MySolverResults"/>
    /// as well as <see cref="EfficientTransmissionExpansionAlternatives"/>.
    /// </remarks>
    public class SolverTepScenariosParetoFrontBruteForceBuilder
        : TepScenariosParetoFrontBruteForceBuilder, IPowerSystemStudy
    {
        #region Internal fields
        public IPowerSystem MyPowerSystem =>
            MyTepSimulationModel.MyTepModelDefinition.MyScenarios.MyPowerSystem;
        public string StudyGenericName =>
            "Brute-force Pareto Front builder for MOO TEP under Scenarios";
        public string StudyInstanceName { get; set; }
        public SolverResults MySolverResults { get; protected set; }
        #endregion

        public SolverTepScenariosParetoFrontBruteForceBuilder() : base() { }

        public SolverTepScenariosParetoFrontBruteForceBuilder(IMooStaticTepSimulationModel myTepModel)
            : base(myTepModel) { }

        public void Solve()
        {
            // Initializes result reporting
            MySolverResults = new SolverResults(StudyGenericName);
            MySolverResults.StartSolutionProcess();
            // Solves
            BuildAlternativesInParetoFront(true);
            // Finalizes result reporting
            MySolverResults.StopSuccessfulSolutionProcess(EfficientTransmissionExpansionAlternatives);
        }

        public void SolveAsync()
        {
            throw new NotImplementedException();
        }

        public void SaveToXml(string fullPath)
        {
            throw new NotImplementedException();
        }
    }
}
