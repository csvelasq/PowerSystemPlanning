using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Branch;

namespace PowerSystemPlanning.Models.Planning.InvestmentBranch
{
    public interface ICandidateTransmissionLine : IInvestmentElement
    {
        ISimpleTransmissionLine UnderlyingSimpleTransmissionLine { get; }
    }
}
