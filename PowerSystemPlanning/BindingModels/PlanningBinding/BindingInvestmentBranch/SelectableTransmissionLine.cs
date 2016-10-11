using PowerSystemPlanning.BindingModels.BaseDataBinding.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingInvestmentBranch
{
    public class SelectableTransmissionLine : BindableBase
    {
        public SimpleTransmissionLine MyLine { get; protected set; }

        protected bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty<bool>(ref _isSelected, value); }
        }

        public SelectableTransmissionLine(SimpleTransmissionLine transmissionLine)
        {
            MyLine = transmissionLine;
        }
    }
}
