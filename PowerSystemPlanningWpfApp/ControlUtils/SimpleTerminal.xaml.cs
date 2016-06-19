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

namespace PowerSystemPlanningWpfApp.ControlUtils
{
    /// <summary>
    /// Interaction logic for SimpleTerminal.xaml
    /// </summary>
    public partial class SimpleTerminal : UserControl
    {
        public double ScrollHeight
        {
            get { return this.scrollTerminal.Height; }
            set { this.scrollTerminal.Height = value; }
        }

        public SimpleTerminal()
        {
            InitializeComponent();
        }

        public void AddMessageToTerminal(string message)
        {
            this.tbTerminal.Inlines.Add(message);
            this.scrollTerminal.ScrollToEnd();
        }
    }
}
