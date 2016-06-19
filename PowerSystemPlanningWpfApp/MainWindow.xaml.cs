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
using PowerSystemPlanning;
using System.Collections.ObjectModel;
using PowerSystemPlanningWpfApp.Models;
using System.IO;

namespace PowerSystemPlanningWpfApp
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PowerSystemViewModel backend;
        List<Node> nodes { get { return backend.nodes; } set { backend.nodes = value; } }
        List<GeneratingUnit> generatingUnits { get { return backend.generatingUnits; } set { backend.generatingUnits = value; } }
        List<InelasticLoad> inelasticLoads { get { return backend.inelasticLoads; } set { backend.inelasticLoads = value; } }
        List<TransmissionLine> transmissionLines { get { return backend.transmissionLines; } set { backend.transmissionLines = value; } }

        private string fileName;
        private string filePath;
        private bool hasFilePath;

        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
            this.backend = new PowerSystemViewModel();
            nodes = backend.nodes;
            generatingUnits = backend.generatingUnits;
            inelasticLoads = backend.inelasticLoads;
            transmissionLines = backend.transmissionLines;
            dgNodes.ItemsSource = nodes;
            dgGenerators.ItemsSource = generatingUnits;
            dgConsumers.ItemsSource = inelasticLoads;
            dgTransmissionLines.ItemsSource = transmissionLines;
        }

        private void saveModel(Stream myStream)
        {
            // Save document
            this.backend.toPowerSystem().saveToXMLFile(myStream);
        }

        private void btnRunOPF_Click(object sender, RoutedEventArgs e)
        {
            PowerSystem powerSystem = new PowerSystem(tbPowerSystemName.Text, generatingUnits, inelasticLoads, transmissionLines);
            OPF.RunOPFWindow runOPFWindow = new OPF.RunOPFWindow(powerSystem);
            runOPFWindow.Show();
        }

        private void btnRunED_Click(object sender, RoutedEventArgs e)
        {
            PowerSystem powerSystem = new PowerSystem(tbPowerSystemName.Text, generatingUnits, inelasticLoads, transmissionLines);
            OPF.RunEconomicDispatch runEDWindow = new OPF.RunEconomicDispatch(powerSystem);
            runEDWindow.Show();
        }

        private void numNodes_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            int n = (int)numNodes.Value;
            if (n < nodes.Count)
            {
                int n2 = (nodes.Count - n);
                MessageBoxResult res = MessageBox.Show("You are about to delete " + n2 + " nodes, are you sure you want to continue?", "Power system planning - delete nodes", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                if (res == MessageBoxResult.Yes)
                {
                    for (int i = nodes.Count; i > n; i--)
                    {
                        nodes.RemoveAt(i - 1);
                    }
                }
                else
                {
                    numNodes.Value = nodes.Count;
                }
            }
            else if (n > nodes.Count)
            {
                for (int i = nodes.Count + 1; i <= n; i++)
                {
                    nodes.Add(new Node(i));
                }
            }
            dgNodes.Items.Refresh();
        }

        private void btnLDCModel_Click(object sender, RoutedEventArgs e)
        {
            //tbTerminal.Inlines.Add("LDC Clicked\n");
        }

        private void btnTEPLDCModel_Click(object sender, RoutedEventArgs e)
        {
            //tbTerminal.Inlines.Add("TEP LDC Clicked\n");
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (.xml)|*.xml";
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                Stream myStream;
                if ((myStream = dlg.OpenFile()) != null)
                {
                    using (myStream)
                    {
                        this.backend = new PowerSystemViewModel(PowerSystem.readFromXMLFile(myStream));
                    }
                }
            }
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML file (.xml)|*.xml"; // Filter files by extension
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                Stream myStream;
                if ((myStream = dlg.OpenFile()) != null)
                {
                    using (myStream)
                    {
                        this.saveModel(myStream);
                    }
                }
            }
        }

        private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML file (.xml)|*.xml"; // Filter files by extension
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                Stream myStream;
                if ((myStream = dlg.OpenFile()) != null)
                {
                    using (myStream)
                    {
                        this.saveModel(myStream);
                    }
                }
            }
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
