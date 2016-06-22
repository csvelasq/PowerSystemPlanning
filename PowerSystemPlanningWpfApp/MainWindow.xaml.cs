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

        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
            this.backend = new PowerSystemViewModel();
            this.setDataContext();
        }

        /// <summary>
        /// Sets the DataContext of datagrids and other UI components to corresponding objects in the backend.
        /// This method is called when starting and also when opening a file.
        /// </summary>
        private void setDataContext()
        {
            this.DataContext = backend;
            this.dgNodes.DataContext = this.backend.nodes;
            this.dgGenerators.DataContext = this.backend.generatingUnits;
            this.dgConsumers.DataContext = this.backend.inelasticLoads;
            this.dgTransmissionLines.DataContext = this.backend.transmissionLines;
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
                this.backend.loadModel(dlg.FileName);
                this.setDataContext();
            }
        }

        private void SaveModel()
        {
            if (!backend.isSaved)
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
                    this.backend.saveModel(dlg.FileName);
                }
            }
            else this.backend.saveModel();
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.SaveModel();
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
                this.backend.saveModel(dlg.FileName);
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
                    this.SaveModel();
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
    }
}
