using System;
using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemState;
using Prism.Mvvm;

namespace PowerSystemPlanning.BindingModels.StateBinding
{
    /// <summary>
    /// Base class for describing a particular state of some element within a power system.
    /// </summary>
    /// <remarks>
    /// Concrete implementations should override <see cref="MyPowerSystemElement"/> with some specific interface.
    /// </remarks>
    [DataContract()]
    public abstract class PowerSystemElementState : BindableBase, IPowerSystemElementState
    {
        /// <summary>
        /// The power system state to which this object belongs.
        /// </summary>
        public IPowerSystemState MyPowerSystemState { get; protected set; }

        /// <summary>
        /// The underlying power system element whose state this object describes.
        /// </summary>
        public virtual IPowerSystemElement MyPowerSystemElement { get; }

        protected PowerSystemElementState(IPowerSystemState state)
        {
            MyPowerSystemState = state;
        }
    }
}
