using NLog;
using PowerSystemPlanning.BindingModels;
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
    /// Provides base logic for saving a study to a subfolder of the folder where the owning power system is saved.
    /// </summary>
    /// <remarks>
    /// Override <see cref="GenericName"/>, <see cref="SaveStudy"/> and <see cref="Open"/>.
    /// </remarks>
    public abstract class StudyInLocalFolder : SerializableBindableBase, IPersistentStudy
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        PowerSystemInLocalFolder _MyOwnerPowerSys;
        public PowerSystemInLocalFolder MyOwnerPowerSys
        {
            get { return _MyOwnerPowerSys; }
            set { SetProperty<PowerSystemInLocalFolder>(ref _MyOwnerPowerSys, value); }
        }

        public virtual string GenericName { get; }

        string _InstanceName;
        public string InstanceName
        {
            get { return _InstanceName; }
            set { SetProperty<string>(ref _InstanceName, value); }
        }

        #region Folder & paths properties
        /// <summary>
        /// The name of the Subfolder to which this study will be saved.
        /// </summary>
        /// <remarks>
        /// This allows for easy identification of existing studies based on naming conventions. 
        /// Not a robust solution but enough if a limited (and known in compile-time) variety of studies can be conducted.
        /// </remarks>
        public string SubFolderName => $"{GenericName}__{InstanceName}";

        /// <summary>
        /// The absolute path to the folder where this power system is saved.
        /// </summary>
        public string FolderAbsolutePath => Path.Combine(MyOwnerPowerSys.FolderAbsolutePath, SubFolderName);
        #endregion

        public StudyInLocalFolder(PowerSystemInLocalFolder owner)
        {
            MyOwnerPowerSys = owner;
        }

        #region Open&Save methods
        /// <summary>
        /// Save this study to <see cref="FolderAbsolutePath"/>, do not override (override <see cref="SaveStudy"/>).
        /// </summary>
        public void Save()
        {
            // TODO handle changes in power system's name
            //Create folder if it does not exist
            if (!Directory.Exists(FolderAbsolutePath))
            {
                Directory.CreateDirectory(FolderAbsolutePath);
            }
            //Save power system (overwrites if file existed)
            SaveStudy();
            logger.Info($"Saved study '{InstanceName}' to '{FolderAbsolutePath}'.");
        }
        /// <summary>
        /// Implementation-dependent logic for saving the study.
        /// </summary>
        public abstract void SaveStudy();

        public void Identify()
        {
            throw new NotImplementedException();
        }

        public abstract void Open();
        #endregion
    }
}
