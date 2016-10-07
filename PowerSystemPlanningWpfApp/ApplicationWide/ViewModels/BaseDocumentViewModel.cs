using PowerSystemPlanning.BindingModels;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    /// <summary>
    /// Base class for view-models of a document (e.g. a power system or a specific study)
    /// </summary>
    [DataContract()]
    public abstract class BaseDocumentViewModel : SerializableBindableBase
    {
        /// <summary>
        /// The title of this document.
        /// </summary>
        public virtual string Title { get; }

        /// <summary>
        /// Save the content wrapped by this view-model.
        /// </summary>
        public abstract void Save();
    }
}
