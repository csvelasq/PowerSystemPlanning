using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels
{
    public class PowerSystemWithCandidateTransmissionLines : IPowerSystem
    {
        PowerSystem MyPowerSystem;
        List<TransmissionLine> AllTransmissionLines;

        public IList<GeneratingUnit> GeneratingUnits
        {
            get
            {
                return MyPowerSystem.GeneratingUnits;
            }
        }

        public IList<InelasticLoad> InelasticLoads
        {
            get
            {
                return MyPowerSystem.InelasticLoads;
            }
        }

        public double LoadSheddingCost
        {
            get
            {
                return MyPowerSystem.LoadSheddingCost;
            }
        }

        public IList<Node> Nodes
        {
            get
            {
                return MyPowerSystem.Nodes;
            }
        }

        public IList<TransmissionLine> TransmissionLines
        {
            get
            {
                return MyPowerSystem.TransmissionLines;
            }
        }

        public PowerSystemWithCandidateTransmissionLines(PowerSystem myPowerSystem, IList<CandidateTransmissionLine> builtTransmissionLines)
        {
            MyPowerSystem = myPowerSystem;
            AllTransmissionLines = new List<TransmissionLine>();
            AllTransmissionLines.AddRange(myPowerSystem.TransmissionLines);
            AllTransmissionLines.AddRange(builtTransmissionLines);
        }
    }
}
