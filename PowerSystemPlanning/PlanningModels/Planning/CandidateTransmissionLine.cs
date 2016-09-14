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

        IList<Node> PowerSystemNodes;
        protected override IList<Node> Nodes
        {
            get { return PowerSystemNodes; }
        }

        IList<CandidateTransmissionLine> CandidateTransmissionLines;

        public CandidateTransmissionLine() : base() { }

        public CandidateTransmissionLine(IList<Node> powerSystemNodes, IList<CandidateTransmissionLine> candidateTransmissionLines)
        {
            PowerSystemNodes = powerSystemNodes;
            CandidateTransmissionLines = candidateTransmissionLines;
            MyPowerSystem = null;
            Id = CandidateTransmissionLines.Count;
        }
    }
}
