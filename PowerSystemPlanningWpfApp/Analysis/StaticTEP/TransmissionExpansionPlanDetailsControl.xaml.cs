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

namespace PowerSystemPlanningWpfApp.Analysis.StaticTEP
{
    /// <summary>
    /// Interaction logic for TransmissionExpansionPlanDetailsControl.xaml
    /// </summary>
    public partial class TransmissionExpansionPlanDetailsControl : UserControl
    {
        // TODO control whether "Is Built?" column of the datagrid is readonly by means of a property of the control (which can be set in XAML)
        /*
        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(Boolean), typeof(TransmissionExpansionPlanDetailsControl));

        /// <summary>
        /// Indicates whether the control allows the underlying transmission expansion plan to be edited.
        /// </summary>
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }
        */

        // TODO filtering http://www.codeproject.com/Articles/32919/Auto-filter-for-Microsoft-WPF-DataGrid

        public TransmissionExpansionPlanDetailsControl()
        {
            InitializeComponent();
        }
    }
}
