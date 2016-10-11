using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Models.SystemBaseData
{
    public interface IHavePowerSystem
    {
        /// <summary>
        /// The power system to which this element belongs.
        /// </summary>
        IPowerSystem MyPowerSystem { get; }
    }
}
