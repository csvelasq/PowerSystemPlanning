using PowerSystemPlanning.Models.Planning.ExpansionState.Tep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.Planning.InvestmentBranch;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingInvestmentBranch
{
    public class CandidateTransmissionLineState : SerializableBindableBase, ICandidateTransmissionLineState
    {
        #region internal fields & interface implementation
        bool _IsBuilt;

        public ICandidateTransmissionLine UnderlyingCandidateTransmissionLine { get; protected set; }

        public IInvestmentElement UnderlyingInvestmentElement =>
            UnderlyingCandidateTransmissionLine;
        #endregion
        
        public bool IsBuilt
        {
            get { return _IsBuilt; }
            set { SetProperty<bool>(ref _IsBuilt, value); }
        }

        public CandidateTransmissionLineState(ICandidateTransmissionLine line)
        {
            UnderlyingCandidateTransmissionLine = line;
        }
    }
}
