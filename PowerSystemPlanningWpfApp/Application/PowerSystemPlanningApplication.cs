using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.Application
{
    /// <summary>
    /// Encapsulates information on the local workspace directory which contains power systems and studies
    /// </summary>
    /// <remarks>
    /// A local directory is used as workspace for the application. In the workspace, each subdir is a power system.
    /// In the subdir of each power system, there is an XML serialization of the power system, and there are also
    /// subdirs for each saved study on the power system.
    /// For example:
    ///     *Test case 3-bus
    ///         +OPF peak load
    ///         +Stochastic TEP under scenarios
    ///         +Stochastic TEP under scenarios v2
    /// 
    /// Currently all power systems in the <see cref="WorkspaceDirectoryFullPath"/> are always opened in the application.
    /// If necessary, it may be useful to extract an interface of <see cref="PowerSystemSummary"/> 
    /// and provide two implementations: one of an unopened power system, and one of an opened power system.
    /// </remarks>
    public class PowerSystemPlanningApplication
    {
        /// <summary>
        /// The full absolute path to the workspace directory.
        /// </summary>
        public string WorkspaceDirectoryFullPath { get; set; }

        /// <summary>
        /// True if <see cref="WorkspaceDirectoryFullPath"/> refers to an existing directory.
        /// </summary>
        public bool WorkspaceExists => Directory.Exists(WorkspaceDirectoryFullPath);

        /// <summary>
        /// A collection of the topmost directories in <see cref="WorkspaceDirectoryFullPath"/>.
        /// </summary>
        public IEnumerable<string> WorkspaceSubDirs => Directory.EnumerateDirectories(WorkspaceDirectoryFullPath);

        /// <summary>
        /// The list of power systems in the current workspace, all of which are currently opened.
        /// </summary>
        public List<PowerSystemSummary> MyPowerSystemsSummary { get; protected set; }
    }
}
