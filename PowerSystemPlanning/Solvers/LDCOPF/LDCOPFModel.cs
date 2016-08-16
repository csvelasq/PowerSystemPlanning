using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.LDCOPF
{
    public class LDCOPFModel
    {
        List<DurationBlock> _DurationBlocks;

        public List<DurationBlock> DurationBlocks
        {
            get
            {
                return _DurationBlocks;
            }

            set
            {
                _DurationBlocks = value;
            }
        }
    }

    public struct DurationBlock
    {
        double Duration;
        double LoadMultiplier;
    }
}
