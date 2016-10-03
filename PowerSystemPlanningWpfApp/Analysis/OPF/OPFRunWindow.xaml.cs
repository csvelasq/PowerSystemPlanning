using PowerSystemPlanning;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.Solvers.LDCOPF;
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

namespace PowerSystemPlanningWpfApp.Analysis.OPF
{
    /// <summary>
    /// Interaction logic for OPFRunWindow.xaml
    /// </summary>
    public partial class OPFRunWindow : Window
    {
        public PowerSystem MyPowerSystem { set { opfRunControl.MyPowerSystem = value; } }

        public OPFRunWindow()
        {
            InitializeComponent();
        }

        public OPFRunWindow(PowerSystem powerSystem)
            : this()
        {
            MyPowerSystem = powerSystem;
        }
    }
}
