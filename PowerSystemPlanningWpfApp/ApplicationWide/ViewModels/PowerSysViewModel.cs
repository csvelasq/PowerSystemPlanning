using NLog;
using Ookii.Dialogs.Wpf;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanningWpfApp.ApplicationWide.AppModels;
using PowerSystemPlanningWpfApp.ApplicationWide.ViewModels;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    public class PowerSysViewModel : BaseDocumentViewModel
    {
        /// <summary>
        /// The event aggregator of this app to comunicate between viewmodels.
        /// </summary>
        protected readonly IEventAggregator _eventAggregator;

        PowerSystemDefinitionInLocalFolder _MyPowerSys;
        public PowerSystemDefinitionInLocalFolder MyPowerSys
        {
            get { return _MyPowerSys; }
            private set
            {
                if (_MyPowerSys != value)
                {
                    _MyPowerSys = value;
                    OnPropertyChanged();
                    //Publish event
                    _eventAggregator.GetEvent<ApplicationWide.Events.PowerSystemOpenedEvent>().Publish(this);
                }
            }
        }

        #region UI properties
        public override string Title => "Power System Editor";

        StudyInLocalFolder _SelectedStudy;
        public StudyInLocalFolder SelectedStudy
        {
            get { return _SelectedStudy; }
            set { SetProperty<StudyInLocalFolder>(ref _SelectedStudy, value); }
        }
        #endregion

        #region Commands
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand NewStudyCommand { get; private set; }
        public DelegateCommand OpenStudyCommand { get; private set; }

        public void SaveFile()
        {
            if (MyPowerSys.FolderAbsolutePath == null)
            {
                var dlg = new VistaFolderBrowserDialog();
                Nullable<bool> result = dlg.ShowDialog();
                // Process save file dialog box results
                if (result == true)
                {
                    MyPowerSys.FolderAbsolutePath = dlg.SelectedPath;
                    Save();
                }
            }
            else
            {
                Save();
            }
        }

        private void Save()
        {
            MyPowerSys.SavePowerSystemAndStudiesToXml();
        }

        public void Open(string xmlPath)
        {
            MyPowerSys = new PowerSystemDefinitionInLocalFolder(xmlPath);
        }

        private void OpenStudy()
        {
            throw new NotImplementedException();
        }

        private void NewStudy()
        {
            var study = MyPowerSys.NewStudy();
            var studyVm = new StudyViewModel() { MyStudy = study };
            //Publish event
            _eventAggregator.GetEvent<ApplicationWide.Events.RequestDocumentOpenEvent>().Publish(studyVm);
        }
        #endregion

        public PowerSysViewModel()
        {
            _eventAggregator = ApplicationService.Instance.EventAggregator;
            SaveFileCommand = new DelegateCommand(SaveFile);
            NewStudyCommand = new DelegateCommand(NewStudy);
            OpenStudyCommand = new DelegateCommand(OpenStudy);
        }

        public PowerSysViewModel(string xmlPath) : this()
        {
            Open(xmlPath);
        }

        public PowerSysViewModel(PowerSystem pws) : this()
        {
            MyPowerSys = new PowerSystemDefinitionInLocalFolder(pws);
        }
    }
}
