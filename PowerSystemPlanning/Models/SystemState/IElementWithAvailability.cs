using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Models.SystemState
{
    /// <summary>
    /// An element that can be temporarily unavailable (.eg. due to forced outage).
    /// </summary>
    public interface IElementWithAvailability
    {
        /// <summary>
        /// True if the element is available.
        /// </summary>
        bool IsAvailable { get; set; }
    }
}
