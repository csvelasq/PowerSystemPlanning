﻿using PowerSystemPlanning.Models.Planning.ExpansionState.Tep;
using PowerSystemPlanning.Models.Planning.InvestmentBranch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Models.Planning.ScenarioTEP
{
    /// <summary>
    /// A simulation model for static transmission expansion planning.
    /// </summary>
    /// <remarks>
    /// Simulates power system operation under different scenarios after implementing a set of candidate transmission lines (<see cref="EvaluateScenarioOperationCosts(IList{ICandidateTransmissionLineState})"/>).
    /// </remarks>
    public interface IStaticScenarioTepSimulationModel
    {
        IStaticScenarioTepModel MyTepModelDefinition { get; }
        IList<ICandidateTransmissionLineState> BuildNewCandidateTransmissionLinesStates(IList<ICandidateTransmissionLine> candidateLinesToBuild);
        /// <summary>
        /// Evaluates operation costs under each scenario after building a set of candidate transmission lines.
        /// </summary>
        /// <param name="candidateLinesStates">A set of objects which describes, for each candidate transmission line in <see cref="MyCandidateTransmissionLines"/>, whether the line is built or not.</param>
        /// <returns>Total operation costs (generation plus load shedding) under each scenario.</returns>
        List<double> EvaluateScenarioOperationCosts(
            IList<ICandidateTransmissionLineState> candidateLinesStates);
    }
}