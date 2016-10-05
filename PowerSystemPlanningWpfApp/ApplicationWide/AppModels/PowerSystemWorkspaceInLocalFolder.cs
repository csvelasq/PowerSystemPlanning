using PowerSystemPlanning.BindingModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide.AppModels
{
    [DataContract()]
    public class PowerSystemWorkspaceInLocalFolder : SerializableBindableBase
    {
        string _WorkspaceFolderAbsolutePath;
        /// <summary>
        /// The absolute path to the workspace for this application instance
        /// </summary>
        [DataMember()]
        public string WorkspaceFolderAbsolutePath
        {
            get { return _WorkspaceFolderAbsolutePath; }
            set { SetProperty<string>(ref _WorkspaceFolderAbsolutePath, value); }
        }

        /// <summary>
        /// Each of the power systems which are saved in this workspace (subfolders in <see cref="WorkspaceFolderAbsolutePath"/>
        /// </summary>
        [DataMember()]
        public BindingList<PowerSystemDefinitionInLocalFolder> MyPowerSystemsDefinition { get; protected set; }
    }
}
