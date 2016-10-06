using NLog;
using Ookii.Dialogs.Wpf;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanningWpfApp.ApplicationWide;
using PowerSystemPlanningWpfApp.ApplicationWide.AppModels;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerSystemPlanningWpfApp
{

    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The event aggregator of this app to comunicate between viewmodels.
        /// </summary>
        protected readonly IEventAggregator _eventAggregator;

        #region View Models
        PowerSysViewModel _OpenedPowerSystemViewModel;
        public PowerSysViewModel OpenedPowerSystemViewModel
        {
            get { return _OpenedPowerSystemViewModel; }
            private set
            {
                if (_OpenedPowerSystemViewModel != value)
                {
                    _OpenedPowerSystemViewModel = value;
                    OnPropertyChanged();
                    //Publish event
                    _eventAggregator.GetEvent<ApplicationWide.Events.PowerSystemOpenedEvent>().Publish(OpenedPowerSystemViewModel);
                }
            }
        }

        public DockManagerViewModel MyDockManagerViewModel { get; private set; }
        #endregion

        #region Commands
        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }

        private void OpenFile()
        {
            // Create OpenFileDialog 
            var dlg = new VistaOpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (.xml)|*.xml";
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                var pwsName = Path.GetFileNameWithoutExtension(dlg.FileName);
                var folder = Path.GetDirectoryName(dlg.FileName);
                logger.Info($"Opening power system '{pwsName}' from folder '{folder}' (file '{dlg.FileName}')");
                OpenedPowerSystemViewModel = new PowerSysViewModel(dlg.FileName);
            }
        }

        private void Exit()
        {
            logger.Info($"Shutting down...");
            System.Windows.Application.Current.Shutdown();
        }
        #endregion

        public MainWindowViewModel()
        {
            _eventAggregator = ApplicationService.Instance.EventAggregator;

            //Commands
            OpenFileCommand = new DelegateCommand(OpenFile);
            ExitCommand = new DelegateCommand(Exit);

            //View models
            this.MyDockManagerViewModel = new DockManagerViewModel();
            //Create new power system
            var powerSystem = new PowerSystemPlanning.BindingModels.BaseDataBinding.PowerSystem() { Name = "Unnamed Power System" };
            OpenedPowerSystemViewModel = new PowerSysViewModel(powerSystem);
        }

        public void DockingManager_DocumentClosed(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            //Publish event
            _eventAggregator.GetEvent<ApplicationWide.Events.DocumentClosedEvent>().Publish((BaseDocumentViewModel)e.Document.Content);
        }
    }
}
