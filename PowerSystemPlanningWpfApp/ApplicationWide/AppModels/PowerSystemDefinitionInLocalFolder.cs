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
    public class PowerSystemDefinitionInLocalFolder : SerializableBindableBase
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        PowerSystem _MyPowerSystem;
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

        public PowerSystemDefinitionInLocalFolder()
        {
        }

        public PowerSystemDefinitionInLocalFolder(PowerSystem pws) : this()
        {
            MyPowerSystem = pws;
        }

        public PowerSystemDefinitionInLocalFolder(string absoluteXmlPath):this()
        {
            MyPowerSystem = PowerSystem.OpenFromXml(absoluteXmlPath);
            FolderAbsolutePath = Path.GetDirectoryName(absoluteXmlPath);
            OpenStudiesInFolder();
        }

        public StudyInLocalFolder NewStudy()
        {
            var study = new StudyInLocalFolder() { MyTepScenarioStudy = new TepScenarioStudy() { InstanceName = $"TEP study {MyStudies.Count + 1}" } };
            MyStudies.Add(study);
            return study;
        }

        #region Save&Open
        private void OpenStudiesInFolder()
        {
            foreach (var folder in Directory.EnumerateDirectories(FolderAbsolutePath))
            {
                var study = new StudyInLocalFolder(folder);
                MyStudies.Add(study);
            }
        }

        /// <summary>
        /// Recursively deletes the folder where this power system is saved.
        /// </summary>
        public void DeleteFolderIfExists()
        {
            if (Directory.Exists(FolderAbsolutePath))
                Directory.Delete(FolderAbsolutePath, true);
        }

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
