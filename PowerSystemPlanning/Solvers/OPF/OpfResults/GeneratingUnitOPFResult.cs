using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Models.SystemState.Generator;

namespace PowerSystemPlanning.Solvers.OPF.OpfResults
{
    /// <summary>
    /// Encapsulator of the OPF result of a generating unit (output, in MW).
    /// </summary>
    public class GeneratingUnitOPFResult : BaseOPFResultElement
    {
        public IGeneratingUnitState MyGeneratingUnitState { get; protected set; }

        /// <summary>
        /// The power output (MW) of this generator in the current OPF solution.
        /// </summary>
        public double PowerOutput { get; protected set; }
        public double MarketShare => PowerOutput / GlobalResultsOpfModelResults.TotalPowerGenerated;

        /// <summary>
        /// The total energy output (GWh) of this generator in the current OPF solution.
        /// </summary>
        public double EnergyOutput => PowerOutput * OpfDuration;

        /// <summary>
        /// The utilization of this generator (actual output / maximum output).
        /// </summary>
        public double Utilization => PowerOutput / MyGeneratingUnitState.UnderlyingGeneratingUnit.InstalledCapacity;
        /// <summary>
        /// The hourly generation cost (US$/MWh) of this generator in the current solution.
        /// </summary>
        public double HourlyGenerationCost => PowerOutput * MyGeneratingUnitState.MarginalCost;
        /// <summary>
        /// Total generation costs (US$) of this generator.
        /// </summary>
        public double TotalGenerationCost => HourlyGenerationCost * OpfDuration;

        public GeneratingUnitOPFResult(OpfModelResult globalResult, IGeneratingUnitState genState, double output)
            : base(globalResult)
        {
            MyGeneratingUnitState = genState;
            PowerOutput = output;
        }
    }
}
