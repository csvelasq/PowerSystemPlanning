using PowerSystemPlanning;
using PowerSystemPlanning.Solvers.LDCOPF;
using PowerSystemPlanning.Solvers.LDCOPF.LdcOpfResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerSystemPlanningWpfApp.Analysis.LDC
{
    /// <summary>
    /// Interaction logic for OPFLDCResultsControl.xaml (meant to be bound to a LDCOPFModelResults)
    /// </summary>
    public partial class OPFLDCResultsControl : UserControl
    {
        LdcOpfModelResults _MyLDCOPFModelResults;
        public LdcOpfModelResults MyLDCOPFModelResults
        {
            get { return _MyLDCOPFModelResults; }
            set
            {
                _MyLDCOPFModelResults = value;
                //Databinding
                DataContext = _MyLDCOPFModelResults;
            }
        }

        public OPFLDCResultsControl()
        {
            InitializeComponent();
        }
    }
}
