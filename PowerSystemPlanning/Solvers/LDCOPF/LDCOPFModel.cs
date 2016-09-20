using Gurobi;
using PowerSystemPlanning.PlanningModels;
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
    public class LDCOPFModel : BaseGRBOptimizationModel
    {
        public override string GRBOptimizationModelName { get { return "Linear Optimal (DC) Power Flow, with Load Duration Curve Representation of Demand"; } }

        public IPowerSystem MyPowerSystem { get; protected set; }

        public LoadDurationCurveByBlocks DurationCurveBlocks { get; protected set; }

        /// <summary>
        /// The OPF model in each block of the duration curve.
        /// </summary>
        public List<OPFModelForLDC> OpfByBlock { get; protected set; }

        /// <summary>
        /// The results of the OPF model in each block of the duration curve.
        /// </summary>
        public List<OPFModelResultForLDC> OpfResultsByBlock { get; protected set; }

        /// <summary>
        /// Detailed results for this LDCOPF, set by calling <see cref="BuildLDCOPFModelResults"/> (after the model is solved).
        /// </summary>
        public LDCOPFModelResults MyDetailedLDCOPFModelResults { get; protected set; }

        public LDCOPFModel(IPowerSystem powerSystem, LoadDurationCurveByBlocks durationCurveBlocks)
            : base()
        {
            this.MyPowerSystem = powerSystem;
            this.DurationCurveBlocks = durationCurveBlocks;
        }

        public LDCOPFModel(IPowerSystem powerSystem, LoadDurationCurveByBlocks durationCurveBlocks, GRBEnv grbEnv, GRBModel grbModel)
            : base(grbEnv, grbModel)
        {
            this.MyPowerSystem = powerSystem;
            this.DurationCurveBlocks = durationCurveBlocks;
        }

        public override void BuildGRBOptimizationModel()
        {
            //creates opf model for each block in the LDC
            this.OpfByBlock = new List<OPFModelForLDC>();
            foreach (LoadBlock durationBlock in this.DurationCurveBlocks.DurationBlocks)
            {
                OPFModelForLDC opfBlock = new OPFModelForLDC(this.MyPowerSystem, MyGrbEnv, MyGrbModel, durationBlock);
                opfBlock.BuildGRBOptimizationModel();
                this.OpfByBlock.Add(opfBlock);
            }
        }

        /// <summary>
        /// Build detailed results for this LDC OPF.
        /// </summary>
        /// <returns>An encapsulator of the detailed results of this LDC OPF.</returns>
        public LDCOPFModelResults BuildLDCOPFModelResults()
        {
            int status = MyGrbModel.Get(GRB.IntAttr.Status);
            OpfResultsByBlock = new List<OPFModelResultForLDC>();
            foreach (OPFModelForLDC opfModel in this.OpfByBlock)
            {
                OpfResultsByBlock.Add(opfModel.BuildOPFModelResultsForLDC());
            }
            MyDetailedLDCOPFModelResults = new LDCOPFModelResults(MyPowerSystem, status, ObjVal, DurationCurveBlocks, OpfResultsByBlock);
            return MyDetailedLDCOPFModelResults;
        }
    }
}
