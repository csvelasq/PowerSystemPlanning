using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerSystemPlanning.Models.SystemBaseData;

namespace PowerSystemPlanning.Models.Planning.ExpansionState
{
    /// <summary>
    /// Represents the current state of an investment element.
    /// </summary>
    public interface IInvestmentElementState
    {
        /// <summary>
        /// The investment element this object describes.
        /// </summary>
        IInvestmentElement UnderlyingInvestmentElement { get; }
        /// <summary>
        /// True if <see cref="UnderlyingInvestmentElement"/> is built in the 
        /// current state.
        /// </summary>
        bool IsBuilt { get; set; }
    }
}
