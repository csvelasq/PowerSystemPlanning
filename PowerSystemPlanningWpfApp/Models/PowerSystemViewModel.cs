using PowerSystemPlanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.Models
{
    public class PowerSystemViewModel
    {
        public List<Node> nodes;
        public List<GeneratingUnit> generatingUnits;
        public List<InelasticLoad> inelasticLoads;
        public List<TransmissionLine> transmissionLines;

        public PowerSystemViewModel()
        {
            nodes = new List<Node>();
            generatingUnits = new List<GeneratingUnit>();
            inelasticLoads = new List<InelasticLoad>();
            transmissionLines = new List<TransmissionLine>();
        }

        public PowerSystemViewModel(PowerSystem pws)
        {
            nodes = pws.nodes;
            generatingUnits = pws.generatingUnits.ToList();
            inelasticLoads = pws.inelasticLoads.ToList();
            transmissionLines = pws.transmissionLines.ToList();
        }

        public PowerSystem toPowerSystem()
        {
            PowerSystem pws = new PowerSystem("name", generatingUnits, inelasticLoads, transmissionLines);
            return pws;
        }
    }
}
