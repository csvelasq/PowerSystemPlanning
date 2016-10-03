using PowerSystemPlanning;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
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

namespace PowerSystemPlanningWpfApp.Model
{
    /// <summary>
    /// Interaction logic for PowerSystemEditorControl.xaml
    /// </summary>
    public partial class PowerSystemEditorControl : UserControl
    {
        PowerSystem _MyPowerSystem;
        public PowerSystem MyPowerSystem
        {
            get { return _MyPowerSystem; }
            set
            {
                _MyPowerSystem = value;
                this.DataContext = _MyPowerSystem;
            }
        }

        public PowerSystemEditorControl()
        {
            InitializeComponent();
        }
    }
}
