using System;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Branch;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Branch;
using PowerSystemPlanning.BindingModels;

namespace PowerSystemPlanning.Models.Planning.InvestmentBranch
{
    public class CandidateTransmissionLine : SerializableBindableBase, ICandidateTransmissionLine
    {
        public SimpleTransmissionLine BindableTransmissionLine { get; protected set; }
        public ISimpleTransmissionLine UnderlyingSimpleTransmissionLine => BindableTransmissionLine;
        public IPowerSystemElement UnderlyingElement => UnderlyingSimpleTransmissionLine;

        protected double _InvestmentCost;
        public double InvestmentCost
        {
            get { return _InvestmentCost; }
            set { SetProperty<double>(ref _InvestmentCost, value); }
        }
        
        public CandidateTransmissionLine(SimpleTransmissionLine transmissionLine)
        {
            BindableTransmissionLine = transmissionLine;
        }
    }
}
