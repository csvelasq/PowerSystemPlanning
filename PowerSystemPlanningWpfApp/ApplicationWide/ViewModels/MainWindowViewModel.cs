using NLog;
using Ookii.Dialogs.Wpf;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
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
using PowerSystemPlanningWpfApp.ApplicationWide.Events;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;

namespace PowerSystemPlanningWpfApp.ApplicationWide.ViewModels
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

        #region Internal fields
        PowerSysViewModel _OpenedPowerSystemViewModel;
        BaseDocumentViewModel _ActiveDocumentViewModel;
        #endregion

        #region View Models
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
                    _eventAggregator.GetEvent<PowerSystemOpenedEvent>().Publish(_OpenedPowerSystemViewModel);
                    _eventAggregator.GetEvent<RequestDocumentOpenEvent>().Publish(_OpenedPowerSystemViewModel);
                }
            }
        }

        public ObservableCollection<BaseDocumentViewModel> MyOpenDocuments { get; private set; }

        public BaseDocumentViewModel ActiveDocumentViewModel
        {
            get { return _ActiveDocumentViewModel; }
            set { SetProperty<BaseDocumentViewModel>(ref _ActiveDocumentViewModel, value); }
        }
        #endregion

        #region Commands
        public DelegateCommand NewFileCommand { get; private set; }
        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand SaveFileAsCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public DelegateCommand NewScenarioOpfCommand { get; private set; }
        public DelegateCommand NewScenarioTepCommand { get; private set; }
        public DelegateCommand OpenScenarioTepCommand { get; private set; }

        private void NewFile()
        {
            OpenedPowerSystemViewModel = new PowerSysViewModel(new PowerSystem());
            //log opened file and persiste last opened file
            logger.Info("New power system created.");
        }

        private void OpenFile()
        {
            // Create OpenFileDialog 
            var dlg = new VistaOpenFileDialog();
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML Files (.xml)|*.xml";
            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                OpenPowerSystemFile(dlg.FileName);
            }
        }

        private bool OpenPowerSystemFile(string path)
        {
            try
            {
                var pwsName = Path.GetFileNameWithoutExtension(path);
                var folder = Path.GetDirectoryName(path);
                OpenedPowerSystemViewModel = new PowerSysViewModel(path);
                //log opened file and persiste last opened file
                logger.Info(
                    $"Succesfully opened power system '{pwsName}' from folder '{folder}' (file '{path}')");
                Properties.Settings.Default.LastOpenedPowerSystem = path;
                Properties.Settings.Default.Save();
                return true;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error opening power system in file '{path}'.");
                return false;
            }
        }

        private void SaveFile()
        {
            ActiveDocumentViewModel.Save();
        }

        private void SaveFileAs()
        {
            ActiveDocumentViewModel.SaveAs();
        }

        private void Exit()
        {
            logger.Info($"Shutting down...");
            System.Windows.Application.Current.Shutdown();
        }

        private void NewScenarioOpf()
        {
            var tepVm = new ScenarioEditorViewModel(OpenedPowerSystemViewModel.MyPowerSystem, new BindableStaticScenarioCollection(OpenedPowerSystemViewModel.MyPowerSystem));
            OpenDocument(tepVm);
        }

        private void NewScenarioTep()
        {
            var tepVm = new ScenarioTepSetupAndParetoViewModel(OpenedPowerSystemViewModel.MyPowerSystem);
            OpenDocument(tepVm);
        }

        private void OpenScenarioTep()
        {
            throw new NotImplementedException();
        }

        private void OpenDocument(BaseDocumentViewModel document)
        {
            _eventAggregator.GetEvent<RequestDocumentOpenEvent>().Publish(document);
        }
        #endregion

        public MainWindowViewModel()
        {
            _eventAggregator = ApplicationService.Instance.EventAggregator;
            _eventAggregator.GetEvent<RequestDocumentOpenEvent>().Subscribe(OnDocumentOpened);

            //Commands
            NewFileCommand = new DelegateCommand(NewFile);
            OpenFileCommand = new DelegateCommand(OpenFile);
            SaveFileCommand = new DelegateCommand(SaveFile);
            SaveFileCommand = new DelegateCommand(SaveFileAs);
            ExitCommand = new DelegateCommand(Exit);
            NewScenarioOpfCommand = new DelegateCommand(NewScenarioOpf);
            NewScenarioTepCommand = new DelegateCommand(NewScenarioTep);
            OpenScenarioTepCommand = new DelegateCommand(OpenScenarioTep);

            //Avalon Dock
            MyOpenDocuments = new ObservableCollection<BaseDocumentViewModel>();

            //Try to open last opened power system
            var lastPws = Properties.Settings.Default.LastOpenedPowerSystem;
            if (!OpenPowerSystemFile(lastPws))
            {
                //Create new power system
                var powerSystem = new PowerSystem() { Name = "Unnamed Power System" };
                OpenedPowerSystemViewModel = new PowerSysViewModel(powerSystem);
                logger.Info("New power system created since last opened power system could not be opened.");
            }
        }

        private void OnDocumentOpened(BaseDocumentViewModel document)
        {
            MyOpenDocuments.Add(document);
            ActiveDocumentViewModel = document;
        }
    }
}
