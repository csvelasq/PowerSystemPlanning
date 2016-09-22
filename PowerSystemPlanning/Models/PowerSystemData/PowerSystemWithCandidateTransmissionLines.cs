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

        public static PowerSystem ClonePWSAndAddCandidateLines(PowerSystem pws, IList<CandidateTransmissionLine> candidates)
        {
            PowerSystem r = new PowerSystem();
            r._GeneratingUnits = pws._GeneratingUnits;
            r._InelasticLoads = pws._InelasticLoads;
            r.LoadSheddingCost = pws.LoadSheddingCost;
            //Copy nodes
            r._Nodes = new System.ComponentModel.BindingList<Node>();
            foreach (var originalNode in pws._Nodes)
            {
                var newNode = new Node(r, originalNode);
                r._Nodes.Add(newNode);
            }
            //Copy transmission lines (existing and candidate)
            r._TransmissionLines = new System.ComponentModel.BindingList<TransmissionLine>();
            foreach (var tl in pws._TransmissionLines)
            {
                r._TransmissionLines.Add(tl);
            }
            foreach (var alt in candidates)
            {
                r._TransmissionLines.Add(alt);
            }
            return r;
        }
    }
}
