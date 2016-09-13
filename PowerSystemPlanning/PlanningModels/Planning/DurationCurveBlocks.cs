using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels
{
    /// <summary>
    /// 
    /// </summary>
    public class LoadDurationCurveByBlocks
    {
        /// <summary>
        /// A list detailing each block which approximate the load duration curve.
        /// </summary>
        public BindingList<LoadBlock> DurationBlocks;

        /// <summary>
        /// Duration of each block (in hours, is a number between 0 and 8760).
        /// </summary>
        public List<double> BlockDurations
        {
            get
            {
                return (from DurationBlock in this.DurationBlocks select DurationBlock.Duration).ToList<double>();
            }
        }

        /// <summary>
        /// Relative duration of each block (hours / 8760, is a number between 0 and 1).
        /// </summary>
        public List<double> RelativeBlockDurations
        {
            get
            {
                return (from DurationBlock in this.DurationBlocks select (DurationBlock.Duration / 8760)).ToList<double>();
            }
        }

        public LoadDurationCurveByBlocks()
        {
            this.DurationBlocks = new BindingList<LoadBlock>();
        }
    }

    public class LoadBlock
    {
        /// <summary>
        /// The duration of this block (in hours).
        /// </summary>
        public double Duration { get; set; }
        
        /// <summary>
        /// The factor which multiplies peak demand in this block (a number between 0 and 1).
        /// </summary>
        public double LoadMultiplier { get; set; }

        public LoadBlock()
        {
            this.Duration = 0;
            this.LoadMultiplier = 0;
        }

        public LoadBlock(double duration, double loadMultiplier)
        {
            this.Duration = duration;
            this.LoadMultiplier = loadMultiplier;
        }

        public override string ToString()
        {
            return String.Format("Load block of {0:N0} hours and {1:p1} of demand", Duration, LoadMultiplier);
        }
    }
}
