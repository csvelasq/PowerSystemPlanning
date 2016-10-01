using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Models.SystemBaseData
{
    /// <summary>
    /// An element which can be built by investing a given amount of capital.
    /// </summary>
    public interface IInvestmentElement : IPowerSystemElement
    {
        /// <summary>
        /// The total investment cost (US$) of this new element.
        /// </summary>
        double InvestmentCost { get; set; }
        //int Lifecycle { get; set; }
        //int Leadtime { get; set; }
    }
}
