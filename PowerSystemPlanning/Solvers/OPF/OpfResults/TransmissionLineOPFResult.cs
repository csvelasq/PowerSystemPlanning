using System;
using PowerSystemPlanning.Models.SystemState.Branch;

namespace PowerSystemPlanning.Solvers.OPF.OpfResults
{
    /// <summary>
    /// Encapsulator of the OPF result of a transmission line (power flow in MW).
    /// </summary>
    public class TransmissionLineOPFResult : BaseOPFResultElement
    {
        public ISimpleTransmissionLineState MyTransmissionLineState { get; protected set; }

        /// <summary>
        /// Power flow (MW) through this branch (positive if going from source to sink node).
        /// </summary>
        public double PowerFlow { get; protected set; }

        /// <summary>
        /// The utilization of this transmission line (actual flow / maximum flow).
        /// </summary>
        public double Utilization => Math.Abs(PowerFlow) / MyTransmissionLineState.AvailableThermalCapacity;

        public TransmissionLineOPFResult(OpfModelResult globalResults, ISimpleTransmissionLineState tlState, double powerFlow)
            : base(globalResults)
        {
            MyTransmissionLineState = tlState;
            PowerFlow = powerFlow;
        }
    }
}
