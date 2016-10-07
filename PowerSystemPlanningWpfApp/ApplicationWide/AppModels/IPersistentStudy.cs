using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide.AppModels
{
    /// <summary>
    /// A study (applied to a particular power system) which can saved to local folder and then opened.
    /// </summary>
    public interface IPersistentStudy
    {
        /// <summary>
        /// The generic name of the study (e.g. "OPF", or "TEP under scenarios").
        /// </summary>
        string GenericName { get; }
        /// <summary>
        /// The name of this instance of the study (e.g. "OPF 4bus Peak demand").
        /// </summary>
        string InstanceName { get; set; }
        string FolderAbsolutePath { get; }
        /// <summary>
        /// Saves this study to <see cref="FolderAbsolutePath"/>.
        /// </summary>
        void Save();
        /// <summary>
        /// Identify myself with the study saved to <see cref="FolderAbsolutePath"/>.
        /// </summary>
        /// <remarks>
        /// Should be a constructor of derived classes. Instead, call this after initializing an empty study.
        /// </remarks>
        void Identify();
        /// <summary>
        /// Opens the study information contained in <see cref="FolderAbsolutePath"/>.
        /// </summary>
        void Open();
    }
}
