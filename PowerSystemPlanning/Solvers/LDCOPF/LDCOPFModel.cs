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
    /// <summary>
    /// Linear programming optimal power flow model for a load-duration curve representation of demand.
    /// </summary>
    public class LDCOPFModel : IGRBOptimizationModel
    {
        public string GRBOptimizationModelName { get { return "Linear Optimal (DC) Power Flow, with Load Duration Curve Representation of Demand"; } }

        protected PowerSystem _PowerSystem;
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

        LoadDurationCurveByBlocks _DurationCurveBlocks;

        public LoadDurationCurveByBlocks DurationCurveBlocks
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

        protected GRBEnv _grbEnv;
        protected GRBModel _grbModel;
        public GRBEnv grbEnv { get { return this._grbEnv; } }
        public GRBModel grbModel { get { return this._grbModel; } }

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

        List<OPFModelResult> _OpfResultsByBlock;
        /// <summary>
        /// The results of the OPF model in each block of the duration curve.
        /// </summary>
        public List<OPFModelResult> OpfResultsByBlock
        {
            get { return _OpfResultsByBlock; }
            set { _OpfResultsByBlock = value; }
        }

        public LDCOPFModel(PowerSystem powerSystem, LoadDurationCurveByBlocks durationCurveBlocks, GRBEnv grbEnv, GRBModel grbModel)
        {
            this._PowerSystem = powerSystem;
            this.DurationCurveBlocks = durationCurveBlocks;
            this._grbEnv = grbEnv;
            this._grbModel = grbModel;
        }

        public void BuildGRBOptimizationModel()
        {
            //creates opf model for each block in the LDC
            this.OpfByBlock = new List<OPFModel>();
            foreach (LoadBlock durationBlock in this.DurationCurveBlocks.DurationBlocks)
            {
                OPFModelForLDC opfBlock = new OPFModelForLDC(this.PowerSystem, this._grbEnv, this._grbModel, durationBlock.LoadMultiplier, durationBlock.Duration);
                opfBlock.BuildGRBOptimizationModel();
                this.OpfByBlock.Add(opfBlock);
            }
        }

        public LDCOPFModelResults BuildLDCOPFModelResults()
        {
            int status = this._grbModel.Get(GRB.IntAttr.Status);
            OpfResultsByBlock = new List<OPFModelResult>();
            foreach (OPFModel opfModel in this.OpfByBlock)
            {
                OpfResultsByBlock.Add(opfModel.BuildOPFModelResults());
            }
            LDCOPFModelResults LDCOPFModelResults = new LDCOPFModelResults(PowerSystem, status, _grbModel.Get(GRB.DoubleAttr.ObjVal), DurationCurveBlocks, OpfResultsByBlock);
            return LDCOPFModelResults;
        }

        public BaseGRBOptimizationModelResult BuildGRBOptimizationModelResults()
        {
            return this.BuildLDCOPFModelResults();
        }
    }
}
