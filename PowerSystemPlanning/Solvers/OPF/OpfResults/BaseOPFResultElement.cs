using PowerSystemPlanning.Models.SystemState;

namespace PowerSystemPlanning.Solvers.OPF.OpfResults
{
    public abstract class BaseOPFResultElement
    {
        public OpfModelResult GlobalResultsOpfModelResults { get; protected set; }

        public double OpfDuration => GlobalResultsOpfModelResults.MyPowerSystemState.Duration;

        public BaseOPFResultElement(OpfModelResult globalResults)
        {
            GlobalResultsOpfModelResults = globalResults;
        }
    }
}
