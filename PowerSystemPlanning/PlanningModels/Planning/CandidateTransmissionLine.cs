using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels
{
    public class CandidateTransmissionLine : TransmissionLine
    {
        /// <summary>
        /// The investment cost (US$) in this candidate transmission line
        /// </summary>
        public double InvestmentCost { get; set; }

        public CandidateTransmissionLine() : base() { }

        public CandidateTransmissionLine(PowerSystem powerSystem) : base(powerSystem) { }
    }
}
