using PowerSystemPlanning.Models.SystemState.Load;

namespace PowerSystemPlanning.Solvers.OPF.OpfResults
{
    public class LoadOPFResult : BaseOPFResultElement
    {
        public IInelasticLoadState MyLoadState { get; protected set; }

        public double PowerConsumed => MyLoadState.Consumption - PowerCurtailed;
        public double EnergyConsumed => PowerConsumed * OpfDuration;

        public double PowerCurtailed { get; protected set; }
        public double EnergyCurtailed => PowerCurtailed * OpfDuration;

        public double HourlyCurtailmentCost => PowerCurtailed * MyLoadState.LoadSheddingCost;
        public double TotalCurtailmentCost => EnergyCurtailed * MyLoadState.LoadSheddingCost;

        public LoadOPFResult(OPFModelResult globalResults, IInelasticLoadState state, double pCurtailed)
            : base(globalResults)
        {
            MyLoadState = state;
            PowerCurtailed = pCurtailed;
        }
    }
}
