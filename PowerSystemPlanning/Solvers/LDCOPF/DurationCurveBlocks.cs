using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.LDCOPF
{

    public class DurationCurveBlocks
    {
        BindingList<DurationBlock> _DurationBlocks;

        public BindingList<DurationBlock> DurationBlocks
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

        public DurationCurveBlocks()
        {
            this.DurationBlocks = new BindingList<DurationBlock>();
        }
    }

    public struct DurationBlock
    {
        public double Duration;
        public double LoadMultiplier;
    }
}
