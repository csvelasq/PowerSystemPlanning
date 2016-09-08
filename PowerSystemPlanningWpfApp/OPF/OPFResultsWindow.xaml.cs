using PowerSystemPlanning;
using PowerSystemPlanning.Solvers;
using PowerSystemPlanning.Solvers.OPF;
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
using System.Windows.Shapes;

namespace PowerSystemPlanningWpfApp.OPF
{
    /// <summary>
    /// Interaction logic for RunOPFWindow.xaml
    /// </summary>
    public partial class OPFResultsWindow : Window
    {
        public OPFModelResultForLDC OPFResults
        {
            set
            {
                this.opfResultsControl.OPFResultsForLDC = value;
            }
        }

        public OPFResultsWindow()
        {
            InitializeComponent();
        }
    }
}
