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
using System.IO;
using NLog;
using PowerSystemPlanning.Solvers.OPF;
using PowerSystemPlanning.Solvers;
using PowerSystemPlanning.Solvers.LDCOPF;

namespace PowerSystemPlanningWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // TODO Avalondock for logging and other windows
        // TODO fix expander http://stackoverflow.com/questions/19516904/wpf-expander-with-gridsplitter

        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Backend of this application.
        /// </summary>
        PowerSystem _PowerSystem;

        PowerSystem PowerSystem
        {
            get { return this._PowerSystem; }
            set
            {
                this._PowerSystem = value;
                this.DataContext = this._PowerSystem;
                this.dgNodes.DataContext = this._PowerSystem._Nodes;
                this.dgConsumers.DataContext = this._PowerSystem._InelasticLoads;
                this.dgGenerators.DataContext = this._PowerSystem._GeneratingUnits;
                this.dgTransmissionLines.DataContext = this._PowerSystem._TransmissionLines;
            }
        }

        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
            this.PowerSystem = new PowerSystem();
            this.RecentFileList.MenuClick += (s, e) => OpenModelFile(e.Filepath);
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //New file
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
                this.OpenModelFile(dlg.FileName);
            }
        }

        private void OpenModelFile(string filename)
        {
            try
            {
                this.PowerSystem = PowerSystem.readFromXMLFile(filename);
                this.RecentFileList.InsertFile(filename);
            }
            catch (Exception e)
            {
                // TODO Move this to backend, find out how to show a messagebox from the backend
                string msgBoxTitle = "Error opening file";
                string msgBoxMsg = String.Format("An error occurred while opening file '{0}'.\nException: {1}", filename, e.Message);
                MainWindow.logger.Error(msgBoxMsg);
                MessageBox.Show(this, msgBoxMsg, msgBoxTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!PowerSystem.IsSaved)
            {
                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = PowerSystem.Name; // Default file name
                dlg.DefaultExt = ".xml"; // Default file extension
                dlg.Filter = "XML file (.xml)|*.xml"; // Filter files by extension
                                                      // Show save file dialog box
                Nullable<bool> result = dlg.ShowDialog();
                // Process save file dialog box results
                if (result == true)
                {
                    this.PowerSystem.saveToXMLFile(dlg.FileName);
                }
            }
            else this.PowerSystem.saveToXMLFile();
        }

        private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = PowerSystem.Name; // Default file name
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML file (.xml)|*.xml"; // Filter files by extension
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                this.PowerSystem.saveToXMLFile(dlg.FileName);
            }
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            // Configure the message box to be displayed
            string caption = "Power system planning";
            string messageBoxText = "Do you want to save changes to this document before the application closes? Click Yes to save and close, No to close without saving, or Cancel to not close.";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;// Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);

            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:
                    // User pressed Yes button: save and then close
                    Application.Current.Shutdown();
                    PowerSystem.saveToXMLFile();
                    break;
                case MessageBoxResult.No:
                    // User pressed No button: close immediately
                    Application.Current.Shutdown();
                    break;
                case MessageBoxResult.Cancel:
                    // User pressed Cancel button: do not close nor save
                    break;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Configures the richtextbox target for showing log
            Dispatcher.Invoke(() =>
            {
                var target = new Helper.WpfRichTextBoxTarget
                {
                    Name = "RichText",
                    Layout =
                        "[${longdate:useUTC=false}] :: [${level:uppercase=true}] :: ${logger}:${callsite} :: ${message} ${exception:innerFormat=tostring:maxInnerExceptionLevel=10:separator=,:format=tostring}",
                    ControlName = LogRichTextBox.Name,
                    FormName = GetType().Name,
                    AutoScroll = true,
                    MaxLines = 100000,
                    UseDefaultRowColoringRules = true,
                };
                var asyncWrapper = new NLog.Targets.Wrappers.AsyncTargetWrapper { Name = "RichTextAsync", WrappedTarget = target };

                LogManager.Configuration.AddTarget(asyncWrapper.Name, asyncWrapper);
                LogManager.Configuration.LoggingRules.Insert(0, new NLog.Config.LoggingRule("*", NLog.LogLevel.Info, asyncWrapper));
                LogManager.ReconfigExistingLoggers();

            });
        }

        private void opfMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Build and solve the opf model
            OPFModelSolver opf = new OPFModelSolver(this.PowerSystem);
            opf.Solve();
            OPFModelResult OPFResults = opf.OPFResults;
            //Show results window
            OPF.OPFResultsWindow opfResultsWindow = new OPF.OPFResultsWindow();
            opfResultsWindow.OPFResults = OPFResults;
            opfResultsWindow.Show();
        }

        private void ldcOpfMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Show results window
            LDC.OPFLDCResultsWindow optOPFLDC = new LDC.OPFLDCResultsWindow(this.PowerSystem);
            optOPFLDC.Show();
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Help.About about = new Help.About();
            about.Show();
        }

        private void staticTepMenuItem_Click(object sender, RoutedEventArgs e)
        {
            StaticTEP.StaticTEPWindow staticTEPWindow = new StaticTEP.StaticTEPWindow();
            staticTEPWindow.Show();
        }

        private void scenarioTepMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ScenarioTEP.ScenarioTEPWindow scenarioTEPWindow = new ScenarioTEP.ScenarioTEPWindow();
            scenarioTEPWindow.Show();
        }
    }
}
