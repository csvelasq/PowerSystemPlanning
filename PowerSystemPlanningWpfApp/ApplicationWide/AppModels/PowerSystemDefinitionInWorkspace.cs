using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide.AppModels
{
    public class PowerSystemDefinitionInWorkspace : PowerSystemDefinitionInLocalFolder
    {
        /// <summary>
        /// The workspace where this power system is saved
        /// </summary>
        public PowerSystemWorkspaceInLocalFolder MyOwnerWorkspace { get; set; }
    }
}
