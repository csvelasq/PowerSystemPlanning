using PowerSystemPlanning;
using PowerSystemPlanning.Solvers;
using PowerSystemPlanning.Solvers.OPF;
using PowerSystemPlanning.Solvers.OPF.OpfResults;
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

namespace PowerSystemPlanningWpfApp.Analysis.OPF
{
    /// <summary>
    /// Interaction logic for OPFResultsControl.xaml
    /// </summary>
    public partial class OPFResultsControl : UserControl
    {
        OPFModelResult _OPFResultsForLDC;

        public OPFModelResult OPFResultsForLDC
        {
            set
            {
                this._OPFResultsForLDC = value;
                this.DataContext = this._OPFResultsForLDC;
            }
        }
        
        public OPFResultsControl()
        {
            InitializeComponent();
        }
    }
}
