using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels
{
    public class CandidateTransmissionLine : TransmissionLine
    {
        // TODO should implement INPC

        /// <summary>
        /// The investment cost (MUS$) in this candidate transmission line
        /// </summary>
        public double InvestmentCost { get; set; }

        IList<Node> PowerSystemNodes;
        protected override IList<Node> Nodes
        {
            get { return PowerSystemNodes; }
        }

        public CandidateTransmissionLine() : base() { }

        public CandidateTransmissionLine(IList<Node> powerSystemNodes, int id)
        {
            PowerSystemNodes = powerSystemNodes;
            MyPowerSystem = null;
            Id = id;
        }
    }
}
