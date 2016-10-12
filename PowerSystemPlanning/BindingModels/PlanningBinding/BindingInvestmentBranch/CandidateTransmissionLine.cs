using System;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Branch;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Branch;
using PowerSystemPlanning.BindingModels;
using System.Runtime.Serialization;

namespace PowerSystemPlanning.Models.Planning.InvestmentBranch
{
    [DataContract()]
    public class CandidateTransmissionLine : SerializableBindableBase, ICandidateTransmissionLine
    {
        [DataMember()]
        public SimpleTransmissionLine BindableTransmissionLine { get; protected set; }
        public ISimpleTransmissionLine UnderlyingSimpleTransmissionLine => BindableTransmissionLine;
        public IPowerSystemElement UnderlyingElement => UnderlyingSimpleTransmissionLine;

        protected double _InvestmentCost;
        [DataMember()]
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
