using PowerSystemPlanning.BindingModels;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide.AppModels
{
    /// <summary>
    /// The definition of a power system saved in a local folder, within a workspace
    /// </summary>
    [DataContract()]
    public class PowerSystemDefinitionInLocalFolder : SerializableBindableBase
    {
        public PowerSystem MyPowerSystem { get; set; }

        #region Folder & paths properties
        string _MasterFolderAbsolutePath;
        /// <summary>
        /// The absolute path of the local folder where this power system is saved (within <see cref="SubFolderRelativePath"/>).
        /// </summary>
        public string MasterFolderAbsolutePath
        {
            get { return _MasterFolderAbsolutePath; }
            set { SetProperty<string>(ref _MasterFolderAbsolutePath, value); }
        }

        string _SubFolderRelativePath;
        /// <summary>
        /// The relative path to the subdirectory of <see cref="MasterFolderAbsolutePath"/> where this power system is saved.
        /// </summary>
        public string SubFolderRelativePath
        {
            get
            {
                return _SubFolderRelativePath;
            }

            set
            {
                SetProperty<string>(ref _SubFolderRelativePath, value);
            }
        }

        /// <summary>
        /// The absolute path to the local subdirectory where this power system is saved.
        /// </summary>
        public string MyFolderAbsolutePath => Path.Combine(MasterFolderAbsolutePath, SubFolderRelativePath);

        /// <summary>
        /// The absolute path to the local XML file where this power system is saved.
        /// </summary>
        public string MyXmlAbsolutePath => Path.Combine(MyFolderAbsolutePath, MyPowerSystem.Name + ".xml");
        #endregion
    }
}
