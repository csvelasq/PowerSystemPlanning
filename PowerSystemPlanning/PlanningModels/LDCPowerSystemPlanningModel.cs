using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels
{
    //TODO decoupled open & save (a collection of serializable objects which can be modified in runtime by plugins)
    public class LDCPowerSystemPlanningModel
    {
        public PowerSystem MyPowerSystem { get; protected set; }
        public LoadDurationCurveByBlocks MyLoadDurationCurve { get; protected set; }
        public BindingList<CandidateTransmissionLine> _MyCandidateTransmissionLines { get; protected set; }

        public LDCPowerSystemPlanningModel(PowerSystem powerSystem)
        {
            MyPowerSystem = powerSystem;
            _MyCandidateTransmissionLines = new BindingList<CandidateTransmissionLine>();
            //Default load duration curve
            MyLoadDurationCurve = new LoadDurationCurveByBlocks();
            MyLoadDurationCurve.DurationBlocks.Add(new LoadBlock(6000, 0.4));
            MyLoadDurationCurve.DurationBlocks.Add(new LoadBlock(2000, 0.6));
            MyLoadDurationCurve.DurationBlocks.Add(new LoadBlock(760, 1));
        }

        public LDCPowerSystemPlanningModel() : this(new PowerSystem()) { }
    }
}
