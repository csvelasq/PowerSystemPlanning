using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Branch;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingInvestmentBranch;
using PowerSystemPlanning.Models.Planning.InvestmentBranch;
using Prism.Commands;
using Prism.Mvvm;

namespace PowerSystemPlanningWpfApp.ApplicationWide.ViewModels
{
    public class CandidateTransmissionLinesEditorViewModel : BindableBase
    {
        #region internal fields
        private BindingList<SelectableTransmissionLine> _transmissionLines;
        private BindingList<CandidateTransmissionLine> _candidateLines;
        #endregion

        public BindingList<SelectableTransmissionLine> TransmissionLines
        {
            get { return _transmissionLines; }
            set { SetProperty<BindingList<SelectableTransmissionLine>>(ref _transmissionLines, value); }
        }
        
        public BindingList<CandidateTransmissionLine> CandidateLines
        {
            get { return _candidateLines; }
            set { SetProperty<BindingList<CandidateTransmissionLine>>(ref _candidateLines, value); }
        }

        #region Commands
        public DelegateCommand EditCandidateLinesCommand { get; protected set; }

        private void EditCandidateLines()
        {
            var candidates = new BindingList<CandidateTransmissionLine>();
            foreach (var line in TransmissionLines)
            {
                if (line.IsSelected)
                    candidates.Add(new CandidateTransmissionLine(line.MyLine));
            }
            CandidateLines = candidates;
        }
        #endregion

        public CandidateTransmissionLinesEditorViewModel(BindingList<SimpleTransmissionLine> transmissionLines)
        {
            TransmissionLines = new BindingList<SelectableTransmissionLine>();
            foreach (var line in transmissionLines)
            {
                var selectableLine = new SelectableTransmissionLine(line);
                TransmissionLines.Add(selectableLine);
            }
            EditCandidateLinesCommand = new DelegateCommand(EditCandidateLines);
        }
    }
}
