using Gurobi;
using PowerSystemPlanning.Solvers.OPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.LDCOPF
{
    public class LDCOPFModel
    {
        PowerSystem _PowerSystem;

        DurationCurveBlocks _DurationCurveBlocks;

        public DurationCurveBlocks DurationCurveBlocks
        {
            get
            {
                return this._DurationCurveBlocks;
            }
            set
            {
                this._DurationCurveBlocks = value;
            }
        }

        public const string OPFModelName = "Linear Optimal (DC) Power Flow, with Load Duration Curve for Demand";

        protected PowerSystem powerSystem;

        protected GRBEnv env;
        protected GRBModel model;

        List<OPFModel> _OpfByBlock;

        /// <summary>
        /// The OPF model in each block of the duration curve.
        /// </summary>
        public List<OPFModel> OpfByBlock
        {
            get
            {
                return _OpfByBlock;
            }

            set
            {
                _OpfByBlock = value;
            }
        }

        public PowerSystem PowerSystem
        {
            get
            {
                return _PowerSystem;
            }

            set
            {
                _PowerSystem = value;
            }
        }

        public LDCOPFModel(PowerSystem powerSystem, DurationCurveBlocks durationCurveBlocks)
        {
            this._PowerSystem = powerSystem;
            this.env = new GRBEnv();
            this.model = new GRBModel(env);
            this.DurationCurveBlocks = durationCurveBlocks;
            this.OpfByBlock = new List<OPFModel>();
            foreach (DurationBlock durationBlock in this.DurationCurveBlocks.DurationBlocks)
            {
                this.OpfByBlock.Add(new OPFModelLoadChange(this.PowerSystem, this.env, this.model, durationBlock.LoadMultiplier));
            }
        }
    }
}
