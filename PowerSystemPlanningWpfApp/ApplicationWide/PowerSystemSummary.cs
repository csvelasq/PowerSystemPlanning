using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Solvers;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    /// <summary>
    /// Encapsulates a power system and the studies that are applied to it.
    /// </summary>
    /// <remarks>
    /// Each power system can have multiple studies (e.g. OPF, STEP) and instances of each study (e.g. OPF-winter, OPF-summer)
    /// The power system and each study are saved in XML files within a directory in <see cref="DirectoryFullPath"/>.
    /// The power system and each study are opened from the XML files within that directory as well.
    /// All the open/save logic is implemented manually in this class.
    /// </remarks>
    public class PowerSystemSummary : BaseDocumentViewModel
    {
        #region Basic properties
        /// <summary>
        /// The power system wrapped by this class.
        /// </summary>
        public PowerSystem MyPowerSystem { get; protected set; }

        /// <summary>
        /// The title of this document is the name of <see cref="MyPowerSystem"/>
        /// </summary>
        public override string Title => MyPowerSystem.Name;

        /// <summary>
        /// The studies defined on this power system.
        /// </summary>
        public BindingList<IPowerSystemStudy> MyStudies { get; protected set; } = new BindingList<IPowerSystemStudy>();

        string _DirectoryFullPath;
        /// <summary>
        /// The full path to the directory where this power system is saved.
        /// </summary>
        public string DirectoryFullPath
        {
            get { return _DirectoryFullPath; }
            set { SetProperty<string>(ref _DirectoryFullPath, value); }
        }

        public string SubDirectoryRelativePath => MyPowerSystem.Name;
        #endregion

        #region Commands
        public DelegateCommand SaveCommand { get; protected set; }
        #endregion

        public PowerSystemSummary() : this(new PowerSystem()) { }

        public PowerSystemSummary(PowerSystem sys)
        {
            MyPowerSystem = sys;
            MyPowerSystem.PropertyChanged += MyPowerSystem_PropertyChanged;
            SaveCommand = new DelegateCommand(Save);
        }

        private void MyPowerSystem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MyPowerSystem.Name))
            {
                OnPropertyChanged(nameof(Title));
            }
        }

        #region Methods
        public void DeleteDirectory()
        {
            Directory.Delete(DirectoryFullPath);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
