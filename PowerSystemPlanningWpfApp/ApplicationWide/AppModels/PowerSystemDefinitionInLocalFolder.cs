using NLog;
using PowerSystemPlanning.BindingModels;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// <remarks>
    /// This class allows for opening / saving a power system in the local folder <see cref="FolderAbsolutePath"/>, 
    /// along with the studies on such power system, which are in turn saved each to a particular subfolder of <see cref="FolderAbsolutePath"/>.
    /// 
    /// Regarding open / save of power system data:
    ///     *To save a new power system, instantiate this class with the constructor that receives a <see cref="PowerSystem"/> object. Then call <see cref="SavePowerSystemXml"/>.
    ///     *To open a power system previously saved with this class, instantiate this class with the constructor that receives a string with the path to the serialized power system.
    /// This class ensures that the XML file is named exactly as <see cref="MyPowerSystem"/> (with an ".xml" extension).
    /// 
    /// Regarding open / save of studies applied to this power system:
    ///     *Each study is saved to one particular subfolder of <see cref="FolderAbsolutePath"/>.
    ///     *The name of the subfolder for each study is "{StudyGenericName}__{StudyInstanceName}".
    ///     *Hence, saved studies can be identified by simply analizing the names of subfolders (needless to say, there should be no external interference with the folder structure where the power system and its studies are saved).
    ///     *Depending on the "StudyGenericName" (first part of the folder name), the appropriate open / save actions will be performed by the derived class of  <see cref="StudyInLocalFolder"/>
    /// </remarks>
    public class PowerSystemInLocalFolder : SerializableBindableBase
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        PowerSystem _MyPowerSystem;
        /// <summary>
        /// The power system in this folder.
        /// </summary>
        /// <remarks>
        /// Either assigned to if this is a new power system, or opened from XML file by calling the constructor with a file path.
        /// </remarks>
        public PowerSystem MyPowerSystem
        {
            get { return _MyPowerSystem; }
            protected set
            {
                if (_MyPowerSystem != value)
                {
                    MyStudies = new BindingList<StudyInLocalFolder>();
                    _MyPowerSystem = value;
                    OnPropertyChanged();
                }
            }
        }

        public BindingList<StudyInLocalFolder> MyStudies { get; protected set; }

        #region Folder & paths properties
        string _FolderAbsolutePath;
        /// <summary>
        /// The absolute path to the folder where this power system is saved.
        /// </summary>
        public string FolderAbsolutePath
        {
            get { return _FolderAbsolutePath; }
            set
            {
                if (_FolderAbsolutePath != value)
                {
                    _FolderAbsolutePath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The absolute path to the local XML file where this power system is saved.
        /// </summary>
        public string MyXmlAbsolutePath => Path.Combine(FolderAbsolutePath, MyPowerSystem.Name + ".xml");
        #endregion

        public PowerSystemInLocalFolder()
        {
        }

        /// <summary>
        /// Initializes with a new power system that is not currently saved to local disk.
        /// </summary>
        /// <param name="pws">The new power system</param>
        public PowerSystemInLocalFolder(PowerSystem pws) : this()
        {
            MyPowerSystem = pws;
        }

        /// <summary>
        /// Initializes by deserializing the power system saved in the provided XML path.
        /// </summary>
        /// <param name="absoluteXmlPath">The path to the XML file where the power system is saved.</param>
        public PowerSystemInLocalFolder(string absoluteXmlPath) : this()
        {
            MyPowerSystem = PowerSystem.OpenFromXml(absoluteXmlPath);
            FolderAbsolutePath = Path.GetDirectoryName(absoluteXmlPath);
            IdentifyStudiesInFolder();
        }

        public TepScenarioStudy NewStudy()
        {
            var study = new TepScenarioStudy(this) { InstanceName = $"TEP study {MyStudies.Count + 1}" };
            MyStudies.Add(study);
            return study;
        }

        #region Save&Open
        /// <summary>
        /// Recursively deletes the folder where this power system is saved.
        /// </summary>
        public void DeleteFolderIfExists()
        {
            if (Directory.Exists(FolderAbsolutePath))
                Directory.Delete(FolderAbsolutePath, true);
        }

        /// <summary>
        /// Save the power system to an XML file in the path <see cref="MyXmlAbsolutePath"/>.
        /// </summary>
        public void SavePowerSystemXml()
        {
            // TODO handle changes in power system's name
            //Create folder if it does not exist
            if (!Directory.Exists(FolderAbsolutePath))
            {
                Directory.CreateDirectory(FolderAbsolutePath);
            }
            //Save power system (overwrites if file existed)
            MyPowerSystem.SaveToXml(MyXmlAbsolutePath);
            logger.Info($"Saved power system {MyPowerSystem.Name} to file {MyXmlAbsolutePath}");
        }
        #endregion

        #region Studies
        private void IdentifyStudiesInFolder()
        {
            foreach (var folder in Directory.EnumerateDirectories(FolderAbsolutePath))
            {
                var study = new StudyInLocalFolder(folder);
                MyStudies.Add(study);
            }
        }

        public void SavePowerSystemAndStudiesToXml()
        {
            this.SavePowerSystemXml();
            foreach (var study in MyStudies)
            {
                study.SaveToXml();
            }
        }
        #endregion
    }
}
