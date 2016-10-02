using PowerSystemPlanning.Models.SystemState;

namespace PowerSystemPlanning.Solvers.OPF.OpfResults
{
    public abstract class BaseOPFResultElement
    {
        public OPFModelResult GlobalResultsOpfModelResults { get; protected set; }

        public double OpfDuration => GlobalResultsOpfModelResults.MyPowerSystemState.Duration;

        public BaseOPFResultElement(OPFModelResult globalResults)
        {
            GlobalResultsOpfModelResults = globalResults;
        }
    }
}
