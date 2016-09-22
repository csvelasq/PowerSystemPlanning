using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace PowerSystemPlanningWpfApp.Analysis.ScenarioTEP
{
    /// <summary>
    /// Interaction logic for ScenarioTEPEnumerateControl.xaml
    /// </summary>
    public partial class ScenarioTEPEnumerateControl : UserControl
    {
        ScenarioTepViewModel MyScenarioTepViewModel { get { return (ScenarioTepViewModel)DataContext; } }

        public ScenarioTEPEnumerateControl()
        {
            InitializeComponent();
            //DataContextChanged += UserControl_DataContextChanged;
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (MyScenarioTepViewModel != null)
            {
                MyScenarioTepViewModel.OnAllTepAlternativesEvaluated += AllTepAlternativesEvaluated;
            }
        }

        void AllTepAlternativesEvaluated()
        {
            var scenarios = MyScenarioTepViewModel.MyScenarioTEPModel.MyScenarios;
            for (int i = 0; i < scenarios.Count; i++)
            {
                string myheader = "Operation Costs in '" + scenarios[i].Name + "'";
                if (dgTEPEnum.Columns.Where(c => ((string)c.Header) == myheader).Count() == 0)
                {
                    var mybinding = new Binding(String.Format("ObjectiveValues[{0}]", i));
                    mybinding.StringFormat = "C";
                    var col = new DataGridTextColumn
                    {
                        Header = myheader,
                        Binding = mybinding,
                        Width = 110
                    };
                    col.ElementStyle = (Style)Application.Current.FindResource("CellRightAlign");
                    dgTEPEnum.Columns.Add(col);
                }
            }
        }
    }
}
