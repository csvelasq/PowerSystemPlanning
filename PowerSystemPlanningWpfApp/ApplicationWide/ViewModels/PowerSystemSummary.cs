using NLog;
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
using System.Runtime.Serialization;
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
    /// All the open/save logic is implemented in this class.
    /// 
    /// To save, using classes must set <see cref="MasterFolderAbsolutePath"/> and <see cref="SubFolderRelativePath"/>.
    /// Calling <see cref="Save"/> (either directly or by means or <see cref="SaveCommand"/>) will create <see cref="MyFolderAbsolutePath"/>
    /// if it doesn't exist, and then write an XML file named as the <see cref="Title"/> property, within the subfolder.
    /// </remarks>
    public class PowerSystemSummary : BaseDocumentViewModel
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        #region Basic properties
        /// <summary>
        /// The power system application who owns this power system wrapper
        /// </summary>
        public PowerSystemPlanningApplication MyOwnerApp { get; set; }

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
        #endregion

        #region Folder & paths properties
        /// <summary>
        /// The absolute path of the local folder where this power system is saved (within <see cref="SubFolderRelativePath"/>).
        /// </summary>
        public string MasterFolderAbsolutePath => MyOwnerApp.WorkspaceFolderAbsolutePath;
        /// <summary>
        /// The relative path to the subdirectory of <see cref="MasterFolderAbsolutePath"/> where this power system is saved.
        /// </summary>
        public string SubFolderRelativePath => MyPowerSystem.Name;

        /// <summary>
        /// The absolute path to the local subdirectory where this power system is saved.
        /// </summary>
        public string MyFolderAbsolutePath => Path.Combine(MasterFolderAbsolutePath, SubFolderRelativePath);

        /// <summary>
        /// The absolute path to the local XML file where this power system is saved.
        /// </summary>
        public string MyXmlAbsolutePath => Path.Combine(MyFolderAbsolutePath, MyPowerSystem.Name + ".xml");
        #endregion

        #region Commands
        public DelegateCommand SaveCommand { get; protected set; }
        #endregion

        public PowerSystemSummary() { }

        public PowerSystemSummary(PowerSystemPlanningApplication ownerApp, PowerSystem sys)
        {
            MyOwnerApp = ownerApp;
            MyPowerSystem = sys;

            MyPowerSystem.PropertyChanged += MyPowerSystem_PropertyChanged;
            SaveCommand = new DelegateCommand(Save);
        }

        #region Public Methods
        /// <summary>
        /// Recursively deletes the folder where this power system is saved.
        /// </summary>
        public void DeleteFolderIfExists()
        {
            if (Directory.Exists(MyFolderAbsolutePath))
                Directory.Delete(MyFolderAbsolutePath, true);
        }

        public void Save()
        {
            // TODO handle changes in power system's name
            //Create folder if it does not exist
            if (!Directory.Exists(MyFolderAbsolutePath))
            {
                Directory.CreateDirectory(MyFolderAbsolutePath);
            }
            //Save power system (overwrites if file existed)
            MyPowerSystem.SaveToXml(MyXmlAbsolutePath);
        }
        #endregion

        #region Private Methods
        private void MyPowerSystem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MyPowerSystem.Name))
            {
                OnPropertyChanged(nameof(Title));
            }
        }
        #endregion

        public static PowerSystemSummary OpenFromXml(PowerSystemPlanningApplication ownerApp, string xmlPath)
        {
            var deserializedPowerSystem = PowerSystem.OpenFromXml(xmlPath);
            var newSummary = new PowerSystemSummary(ownerApp, deserializedPowerSystem);
            return newSummary;
        }
    }
}
