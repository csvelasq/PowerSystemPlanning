using NLog;
using Ookii.Dialogs.Wpf;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanningWpfApp.ApplicationWide;
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
                    MyOpenDocuments.Clear();
                    MyOpenDocuments.Add(OpenedPowerSystemViewModel);
                }
            }
        }

        public ObservableCollection<BaseDocumentViewModel> MyOpenDocuments { get; private set; }

        BaseDocumentViewModel _ActiveDocumentViewModel;
        public BaseDocumentViewModel ActiveDocumentViewModel
        {
            get { return _ActiveDocumentViewModel; }
            set { SetProperty<BaseDocumentViewModel>(ref _ActiveDocumentViewModel, value); }
        }
        #endregion

        #region Commands
        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand NewScenarioTepCommand { get; private set; }

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
                OpenedPowerSystemViewModel = new PowerSysViewModel(dlg.FileName);
                logger.Info($"Succesfully opened power system '{pwsName}' from folder '{folder}' (file '{dlg.FileName}')");
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
            SaveFileCommand = new DelegateCommand(SaveFile);
            ExitCommand = new DelegateCommand(Exit);
            NewScenarioTepCommand = new DelegateCommand(NewScenarioTep);

            //Avalon Dock
            MyOpenDocuments = new ObservableCollection<BaseDocumentViewModel>();

            //Create new power system
            var powerSystem = new PowerSystem() { Name = "Unnamed Power System" };
            OpenedPowerSystemViewModel = new PowerSysViewModel(powerSystem);
        }

        private void SaveFile()
        {
            ActiveDocumentViewModel.Save();
        }

        private void NewScenarioTep()
        {
            var tepVm = new ScenarioEditorViewModel(OpenedPowerSystemViewModel.MyPowerSystem);
            MyOpenDocuments.Add(tepVm);
        }
    }
}
