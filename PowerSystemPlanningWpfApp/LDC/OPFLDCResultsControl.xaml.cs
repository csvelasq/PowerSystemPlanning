using PowerSystemPlanning;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerSystemPlanningWpfApp.LDC
{
    /// <summary>
    /// Interaction logic for OPFLDCResultsControl.xaml
    /// </summary>
    public partial class OPFLDCResultsControl : UserControl
    {
        LDCOPFModel _LDCOPFModel;

        public LDCOPFModel LDCOPFModel
        {
            get
            {
                return this._LDCOPFModel;
            }
            set
            {
                this._LDCOPFModel = value;
            }
        }

        public OPFLDCResultsControl()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            int selected_index = this.dgLDC.SelectedIndex;
            OPF.OPFResultsWindow opfResultsWindow = new OPF.OPFResultsWindow();
            opfResultsWindow.OPFResults = this.LDCOPFModel.OpfByBlock[selected_index].BuildOPFModelResults();
            opfResultsWindow.Show();
        }
    }
}
