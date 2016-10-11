using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Models.SystemState
{
    public interface IStaticStateCollection : IPowerSystemStateCollection
    {
        IList<IPowerSystemState> MyStaticStates { get; }
    }
}
