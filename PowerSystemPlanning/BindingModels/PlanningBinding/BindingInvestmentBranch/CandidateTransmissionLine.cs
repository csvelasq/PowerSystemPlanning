using System;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Branch;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Branch;

namespace PowerSystemPlanning.Models.Planning.InvestmentBranch
{
    public class CandidateTransmissionLine : PowerSystemElement, ICandidateTransmissionLine
    {
        public SimpleTransmissionLine BindableTransmissionLine { get; protected set; }
        public ISimpleTransmissionLine UnderlyingSimpleTransmissionLine => BindableTransmissionLine;

        protected double _InvestmentCost;
        public double InvestmentCost
        {
            get { return _InvestmentCost; }
            set { SetProperty<double>(ref _InvestmentCost, value); }
        }

        public CandidateTransmissionLine(SimpleTransmissionLine transmissionLine)
            : base(transmissionLine.MyBindingPowerSystem, transmissionLine.Id)
        {
            BindableTransmissionLine = transmissionLine;
        }
    }
}
