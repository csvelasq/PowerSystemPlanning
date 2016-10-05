using System;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Branch;
using PowerSystemPlanning.BindingModels.BaseDataBinding;

namespace PowerSystemPlanning.Models.Planning.InvestmentBranch
{
    public class CandidateTransmissionLine : PowerSystemElement, ICandidateTransmissionLine
    {
        protected double _InvestmentCost;
        public double InvestmentCost
        {
            get { return _InvestmentCost; }
            set { SetProperty<double>(ref _InvestmentCost, value); }
        }

        public ISimpleTransmissionLine UnderlyingSimpleTransmissionLine
        { get; protected set; }
        
        public CandidateTransmissionLine(PowerSystem pws)
            : base(pws, pws.GeneratingUnits.Count)
        { }
    }
}
