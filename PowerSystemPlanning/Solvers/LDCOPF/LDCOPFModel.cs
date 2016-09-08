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
        
        public PowerSystem PowerSystem { get; protected set; }

        public LoadDurationCurveByBlocks DurationCurveBlocks { get; protected set; }
        
        public GRBEnv grbEnv { get; protected set; }
        public GRBModel grbModel { get; protected set; }

        /// <summary>
        /// The OPF model in each block of the duration curve.
        /// </summary>
        public List<OPFModelForLDC> OpfByBlock { get; protected set; }

        /// <summary>
        /// The results of the OPF model in each block of the duration curve.
        /// </summary>
        public List<OPFModelResultForLDC> OpfResultsByBlock { get; protected set; }

        public LDCOPFModel(PowerSystem powerSystem, LoadDurationCurveByBlocks durationCurveBlocks, GRBEnv grbEnv, GRBModel grbModel)
        {
            this.PowerSystem = powerSystem;
            this.DurationCurveBlocks = durationCurveBlocks;
            this.grbEnv = grbEnv;
            this.grbModel = grbModel;
        }

        public void BuildGRBOptimizationModel()
        {
            //creates opf model for each block in the LDC
            this.OpfByBlock = new List<OPFModelForLDC>();
            foreach (LoadBlock durationBlock in this.DurationCurveBlocks.DurationBlocks)
            {
                OPFModelForLDC opfBlock = new OPFModelForLDC(this.PowerSystem, this.grbEnv, this.grbModel, durationBlock);
                opfBlock.BuildGRBOptimizationModel();
                this.OpfByBlock.Add(opfBlock);
            }
        }

        public LDCOPFModelResults BuildLDCOPFModelResults()
        {
            int status = this.grbModel.Get(GRB.IntAttr.Status);
            OpfResultsByBlock = new List<OPFModelResultForLDC>();
            foreach (OPFModelForLDC opfModel in this.OpfByBlock)
            {
                OpfResultsByBlock.Add(opfModel.BuildOPFModelResultsForLDC());
            }
            LDCOPFModelResults LDCOPFModelResults = new LDCOPFModelResults(PowerSystem, status, grbModel.Get(GRB.DoubleAttr.ObjVal), DurationCurveBlocks, OpfResultsByBlock);
            return LDCOPFModelResults;
        }

        public BaseGRBOptimizationModelResult BuildGRBOptimizationModelResults()
        {
            return this.BuildLDCOPFModelResults();
        }
    }
}
