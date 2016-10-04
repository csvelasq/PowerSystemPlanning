using NLog;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    /// <summary>
    /// Encapsulates information on the local workspace directory which contains power systems and studies
    /// </summary>
    /// <remarks>
    /// A local directory is used as workspace for the application. In the workspace, each subdir is a power system.
    /// In the subdir of each power system, there is an XML serialization of the power system, and there are also
    /// subdirs for each saved study on the power system.
    /// For example:
    ///     *Test case 3-bus
    ///         +OPF peak load
    ///         +Stochastic TEP under scenarios
    ///         +Stochastic TEP under scenarios v2
    /// 
    /// Currently all power systems in the <see cref="WorkspaceDirectoryFullPath"/> are always opened in the application.
    /// If necessary, it may be useful to extract an interface of <see cref="PowerSystemSummary"/> 
    /// and provide two implementations: one of an unopened power system, and one of an opened power system.
    /// </remarks>
    public class PowerSystemPlanningApplication : BindableBase
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The event aggregator of this app to comunicate between viewmodels.
        /// </summary>
        protected readonly IEventAggregator _eventAggregator;

        #region Basic Properties
        /// <summary>
        /// The full absolute path to the workspace directory.
        /// </summary>
        /// <remarks>
        /// Workspace directory is a user setting, can be modified by calling <see cref="OpenWorkspaceDirectory(string)"/>
        /// which is in turn called by <see cref="OpenWorkspaceCommand"/>
        /// </remarks>
        public string WorkspaceDirectoryFullPath => Properties.Settings.Default.Workspace;

        /// <summary>
        /// True if <see cref="WorkspaceDirectoryFullPath"/> refers to an existing directory.
        /// </summary>
        public bool WorkspaceExists => Directory.Exists(WorkspaceDirectoryFullPath);

        /// <summary>
        /// A collection of the topmost directories in <see cref="WorkspaceDirectoryFullPath"/>.
        /// </summary>
        public IEnumerable<string> WorkspaceSubDirs => Directory.EnumerateDirectories(WorkspaceDirectoryFullPath);

        /// <summary>
        /// The list of power systems in the current workspace, all of which are currently opened.
        /// </summary>
        public BindingList<PowerSystemSummary> MyPowerSystemsSummary { get; protected set; }
        #endregion

        #region UI properties (e.g. selected item)
        PowerSystemSummary _SelectedPowerSystemSummary;

        /// <summary>
        /// The power system currently selected
        /// </summary>
        public PowerSystemSummary SelectedPowerSystemSummary
        {
            get { return _SelectedPowerSystemSummary; }
            set
            {
                SetProperty<PowerSystemSummary>(ref _SelectedPowerSystemSummary, value);
                OnSelectionCanExecuteChanged(); //not necessary to call it always but it doesn't hurt
            }
        }

        public bool IsTherePowerSystemSelected() => SelectedPowerSystemSummary != null;
        #endregion

        #region Commands
        public DelegateCommand OpenWorkspaceCommand { get; protected set; }
        public DelegateCommand NewPowerSystemCommand { get; protected set; }
        public DelegateCommand EditSelectedPowerSystemCommand { get; protected set; }
        public DelegateCommand DeleteSelectedPowerSystemCommand { get; protected set; }
        public DelegateCommand SaveSelectedPowerSystemCommand { get; protected set; }
        public CompositeCommand SaveAllPowerSystemsCommand { get; protected set; }
        #endregion

        public PowerSystemPlanningApplication()
        {
            _eventAggregator = ApplicationService.Instance.EventAggregator;
            //Command initialization
            OpenWorkspaceCommand =
                new DelegateCommand(OpenWorkspace);
            NewPowerSystemCommand =
                new DelegateCommand(CreateNewPowerSystem);
            EditSelectedPowerSystemCommand =
                new DelegateCommand(EditSelectedPowerSystem, IsTherePowerSystemSelected);
            DeleteSelectedPowerSystemCommand =
                new DelegateCommand(DeleteSelectedPowerSystem, IsTherePowerSystemSelected);
            SaveSelectedPowerSystemCommand =
                new DelegateCommand(SaveSelectedPowerSystem, IsTherePowerSystemSelected);
            SaveAllPowerSystemsCommand =
                new CompositeCommand();
            MyPowerSystemsSummary = new BindingList<PowerSystemSummary>();
            MyPowerSystemsSummary.AddingNew += MyPowerSystemsSummary_AddingNew;
            //Open workspace
            OpenWorkspaceDirectory(Properties.Settings.Default.Workspace);
        }

        #region Command Methods
        private void OpenWorkspace()
        {
            // Configure the message box to be displayed
            string caption = "Power system planning";
            string messageBoxText = "Unsaved changes in this workspace will be lost. Continue?";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;// Display message box
            MessageBoxResult warningResult = MessageBox.Show(messageBoxText, caption, button, icon);
            // Process message box results
            if (warningResult == MessageBoxResult.Yes)
            {
                // Create dialog
                var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
                bool? result = dialog.ShowDialog();
                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    this.OpenWorkspaceDirectory(dialog.SelectedPath);
                }
            }
        }

        private void OnSelectionCanExecuteChanged()
        {
            EditSelectedPowerSystemCommand.RaiseCanExecuteChanged();
            DeleteSelectedPowerSystemCommand.RaiseCanExecuteChanged();
            SaveSelectedPowerSystemCommand.RaiseCanExecuteChanged();
        }

        private void EditSelectedPowerSystem()
        {
            //Publish event
            _eventAggregator.GetEvent<Events.RequestPowerSystemEditionEvent>().Publish(_SelectedPowerSystemSummary);
        }

        private void SaveSelectedPowerSystem()
        {
            SelectedPowerSystemSummary.Save();
        }

        private void DeleteSelectedPowerSystem()
        {
            //TODO Request confirmation
            //Delete the directory from the local disk
            SelectedPowerSystemSummary.DeleteDirectory();
            //Delte the object from memory
            MyPowerSystemsSummary.Remove(SelectedPowerSystemSummary);
            SelectedPowerSystemSummary = null;
        }

        private void CreateNewPowerSystem()
        {
            //Create a new unnamed power system
            var newPowerSystemSummary = new PowerSystemSummary();
            newPowerSystemSummary.MyPowerSystem.Name = "Unnamed power system";
            MyPowerSystemsSummary.Add(newPowerSystemSummary);
            //Publish event
            _eventAggregator.GetEvent<Events.PowerSystemCreatedEvent>().Publish(_SelectedPowerSystemSummary);
        }
        #endregion

        /// <summary>
        /// Opens every power system within a given power system directory
        /// </summary>
        /// <param name="selectedPath"></param>
        public void OpenWorkspaceDirectory(string selectedPath)
        {
            //Only reopen workspace if a new directory was selected
            if (selectedPath != Properties.Settings.Default.Workspace)
            {
                //Persist workspace
                Properties.Settings.Default.Workspace = selectedPath;
                Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(WorkspaceDirectoryFullPath));
                //Log
                logger.Debug($"Opening workspace '{WorkspaceDirectoryFullPath}'");
                foreach (var subdir in WorkspaceSubDirs)
                {
                    logger.Debug($"Opening power system directory '{subdir}'");
                }
            }
        }

        private void MyPowerSystemsSummary_AddingNew(object sender, AddingNewEventArgs e)
        {
            var newObject = (PowerSystemSummary)e.NewObject;
            this.SaveAllPowerSystemsCommand.RegisterCommand(newObject.SaveCommand);
        }
    }
}
