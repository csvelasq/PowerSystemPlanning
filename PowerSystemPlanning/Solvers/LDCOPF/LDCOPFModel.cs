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

        public IEnumerable<IPowerSystemState> MyPowerSystemStates { get; protected set; }

        /// <summary>
        /// The OPF model in each block of the duration curve.
        /// </summary>
        public List<OPFModel> OpfByBlock { get; protected set; }

        /// <summary>
        /// The results of the OPF model in each block of the duration curve.
        /// </summary>
        public List<OPFModelResult> OpfResultsByBlock { get; protected set; }

        /// <summary>
        /// Detailed results for this LDCOPF, set by calling <see cref="BuildLDCOPFModelResults"/> (after the model is solved).
        /// </summary>
        public LDCOPFModelResults MyDetailedLDCOPFModelResults { get; protected set; }

        public LDCOPFModel(IEnumerable<IPowerSystemState> powerSystemStates)
            : base()
        {
            this.MyPowerSystemStates = powerSystemStates;
        }

        public LDCOPFModel(IEnumerable<IPowerSystemState> powerSystemStates, GRBEnv grbEnv, GRBModel grbModel)
            : base(grbEnv, grbModel)
        {
            this.MyPowerSystemStates = powerSystemStates;
        }

        public override void BuildGRBOptimizationModel()
        {
            //creates opf model for each block in the LDC
            OpfByBlock = new List<OPFModel>();
            foreach (var state in MyPowerSystemStates)
            {
                var opfBlock = new OPFModel(state, MyGrbEnv, MyGrbModel);
                opfBlock.BuildGRBOptimizationModel();
                OpfByBlock.Add(opfBlock);
            }
        }

        /// <summary>
        /// Build detailed results for this LDC OPF.
        /// </summary>
        /// <returns>An encapsulator of the detailed results of this LDC OPF.</returns>
        public LDCOPFModelResults BuildLDCOPFModelResults()
        {
            int status = MyGrbModel.Get(GRB.IntAttr.Status);
            OpfResultsByBlock = new List<OPFModelResult>();
            foreach (OPFModel opfModel in this.OpfByBlock)
            {
                OpfResultsByBlock.Add(opfModel.BuildOPFModelResults());
            }
            MyDetailedLDCOPFModelResults = new LDCOPFModelResults(MyPowerSystemStates, status, ObjVal, OpfResultsByBlock);
            return MyDetailedLDCOPFModelResults;
        }
    }
}
