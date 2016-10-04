using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    /// <summary>
    /// Base class for view-models of a document (e.g. a power system or a specific study)
    /// </summary>
    public abstract class BaseDocumentViewModel : BindableBase
    {
        /// <summary>
        /// The title of this document.
        /// </summary>
        public virtual string Title { get; }
    }
}
