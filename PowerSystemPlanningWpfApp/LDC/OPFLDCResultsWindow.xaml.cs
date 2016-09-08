using PowerSystemPlanning.Solvers.LDCOPF;
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

namespace PowerSystemPlanningWpfApp.LDC
{
    /// <summary>
    /// Interaction logic for OPFLDCResultsWindow.xaml
    /// </summary>
    public partial class OPFLDCResultsWindow : Window
    {

        public OPFLDCResultsWindow()
        {
            InitializeComponent();
        }
        
        public OPFLDCResultsWindow(PowerSystemPlanning.PowerSystem powerSystem) : this()
        {
            this.opfLdcResultsControl.PowerSystem = powerSystem;
        }
    }
}
