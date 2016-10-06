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
    public class StudyInLocalFolder : SerializableBindableBase
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        PowerSystemDefinitionInLocalFolder _MyOwnerPowerSys;
        public PowerSystemDefinitionInLocalFolder MyOwnerPowerSys
        {
            get
            {
                return _MyOwnerPowerSys;
            }

            set
            {
                SetProperty<PowerSystemDefinitionInLocalFolder>(ref _MyOwnerPowerSys, value);
            }
        }

        TepScenarioStudy _MyTepScenarioStudy;
        public TepScenarioStudy MyTepScenarioStudy
        {
            get { return _MyTepScenarioStudy; }
            set { SetProperty<TepScenarioStudy>(ref _MyTepScenarioStudy, value); }
        }

        #region Folder & paths properties
        /// <summary>
        /// The absolute path to the folder where this power system is saved.
        /// </summary>
        public string FolderAbsolutePath => Path.Combine(MyOwnerPowerSys.FolderAbsolutePath, MyTepScenarioStudy.InstanceName);

        /// <summary>
        /// The absolute path to the local XML file where this power system is saved.
        /// </summary>
        public string MyXmlAbsolutePath => Path.Combine(FolderAbsolutePath, MyTepScenarioStudy.InstanceName + ".xml");
        #endregion

        public StudyInLocalFolder()
        {

        }

        public StudyInLocalFolder(string folderAbsolutePath) : this()
        {
            var file = new DirectoryInfo(folderAbsolutePath).Name;
            var xmlFile = Path.Combine(folderAbsolutePath, file + ".xml");
            MyTepScenarioStudy = TepScenarioStudy.OpenFromXML(xmlFile);
            MyTepScenarioStudy.MyPowerSystem = MyPowerSys.MyPowerSystem;
        }

        public void SaveToXml()
        {
            // TODO handle changes in power system's name
            //Create folder if it does not exist
            if (!Directory.Exists(FolderAbsolutePath))
            {
                Directory.CreateDirectory(FolderAbsolutePath);
            }
            //Save power system (overwrites if file existed)
            MyTepScenarioStudy.SaveToXml(MyXmlAbsolutePath);
            logger.Info($"Saved study '{MyTepScenarioStudy.InstanceName}' to '{MyXmlAbsolutePath}'.");
        }
    }
}
