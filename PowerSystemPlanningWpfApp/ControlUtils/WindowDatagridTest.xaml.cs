using Prism.Mvvm;
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
using System.Windows.Shapes;

namespace PowerSystemPlanningWpfApp.ControlUtils
{
    /// <summary>
    /// Interaction logic for WindowDatagridTest.xaml
    /// </summary>
    public partial class WindowDatagridTest : Window
    {
        public SimpleBackendModel backend;
        public WindowDatagridTest()
        {
            backend = new SimpleBackendModel();
            InitializeComponent();
            this.dgTest.ItemsSource = backend.backendlist;
        }
    }

    public class SimpleBackendModel
    {
        public BindingList<SimpleBackendObject> backendlist;
        public SimpleBackendModel()
        {
            this.backendlist = new BindingList<SimpleBackendObject>();
            this.backendlist.Add(new SimpleBackendObject("constantin",24));
        }
    }

    public class SimpleBackendObject : BindableBase
    {
        string _Name;
        int _Age;

        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
            }
        }

        public int Age
        {
            get
            {
                return _Age;
            }

            set
            {
                _Age = value;
            }
        }

        public SimpleBackendObject() { }

        public SimpleBackendObject(string name, int age)
        {
            this.Name = name;
            this.Age = age;
        }
    }
}
