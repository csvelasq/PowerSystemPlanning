using PowerSystemPlanning.PlanningModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.Analysis.TEP
{
    public class TransmissionExpansionPlanDetailViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Example implementation from: https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public BindingList<CandidateTransmissionLineForBinding> MyCandidateTransmissionLines { get; set; }
        /// <summary>
        /// Can be used to create a new transmission expansion plan.
        /// </summary>
        public IEnumerable<CandidateTransmissionLine> MySelectedCandidateTransmissionLines
        {
            get
            {
                return from x in MyCandidateTransmissionLines where x.IsBuilt select x.MyCandidateTransmissionLine;
            }
        }
        public int NumberOfBuiltTransmissionLines
        {
            get
            {
                return MySelectedCandidateTransmissionLines.Count();
            }
        }
        public double TotalInvestmentCost
        {
            get
            {
                return (from x in MyCandidateTransmissionLines where x.IsBuilt select x.InvestmentCost).Sum();
            }
        }
        public bool IsEditable { get; set; }
        
        public TransmissionExpansionPlanDetailViewModel(BindingList<CandidateTransmissionLine> candidates, bool isEditable)
        {
            IsEditable = isEditable;
            MyCandidateTransmissionLines = new BindingList<CandidateTransmissionLineForBinding>();
            foreach (CandidateTransmissionLine c in candidates)
            {
                var c2 = new CandidateTransmissionLineForBinding(c);
                MyCandidateTransmissionLines.Add(c2);
                // detect changes in summary properties
                c2.PropertyChanged += CandidateLine_PropertyChanged;
            }
        }

        private void CandidateLine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged("NumberOfBuiltTransmissionLines");
            NotifyPropertyChanged("TotalInvestmentCost");
        }
    }

    /// <summary>
    /// A wrapper of <see cref="CandidateTransmissionLine"/> to be used by WPF GUI in order to view and edit a candidate transmission expansion plan.
    /// </summary>
    public class CandidateTransmissionLineForBinding : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Example implementation from: https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public CandidateTransmissionLine MyCandidateTransmissionLine;

        bool _IsBuilt;
        /// <summary>
        /// Indicates whether this transmission line is built in the containing transmission expansion plan.
        /// </summary>
        public bool IsBuilt
        {
            get
            {
                return _IsBuilt;
            }
            set
            {
                if (_IsBuilt!=value)
                {
                    _IsBuilt = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The name of the candidate transmission line
        /// </summary>
        public string Name { get { return MyCandidateTransmissionLine.Name; } }
        /// <summary>
        /// The investment cost in the candidate transmission line
        /// </summary>
        public double InvestmentCost { get { return MyCandidateTransmissionLine.InvestmentCost; } }

        public CandidateTransmissionLineForBinding(CandidateTransmissionLine candidateTransmissionLine) : base()
        {
            MyCandidateTransmissionLine = candidateTransmissionLine;
        }

    }
}
