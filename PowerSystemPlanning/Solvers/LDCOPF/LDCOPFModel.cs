using Gurobi;
using PowerSystemPlanning.Solvers.OPF;
using System.Collections.Generic;
using System.Linq;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Solvers.GRBOptimization;
using PowerSystemPlanning.Solvers.OPF.OpfResults;
using PowerSystemPlanning.Solvers.LDCOPF.LdcOpfResults;

namespace PowerSystemPlanning.Solvers.LDCOPF
{
    /// <summary>
    /// Linear programming optimal power flow model for a load-duration curve representation of demand.
    /// </summary>
    public class LdcOpfModel : BaseGRBOptimizationModel
    {
        public override string GRBOptimizationModelName { get { return "Linear Optimal (DC) Power Flow, with Load Duration Curve Representation of Demand"; } }

        public List<IPowerSystemState> MyPowerSystemStates { get; protected set; }

        /// <summary>
        /// The OPF model in each block of the duration curve.
        /// </summary>
        public List<OpfModel> OpfByBlock { get; protected set; }

        /// <summary>
        /// The results of the OPF model in each block of the duration curve.
        /// </summary>
        public List<OpfModelResult> OpfResultsByBlock { get; protected set; }

        /// <summary>
        /// Detailed results for this LDCOPF, set by calling <see cref="BuildLDCOPFModelResults"/> (after the model is solved).
        /// </summary>
        public LdcOpfModelResults MyDetailedLDCOPFModelResults { get; protected set; }

        public LdcOpfModel(IEnumerable<IPowerSystemState> powerSystemStates)
            : base()
        {
            this.MyPowerSystemStates = new List<IPowerSystemState>(powerSystemStates);
        }

        public LdcOpfModel(IEnumerable<IPowerSystemState> powerSystemStates, GRBEnv grbEnv, GRBModel grbModel)
            : base(grbEnv, grbModel)
        {
            this.MyPowerSystemStates = new List<IPowerSystemState>(powerSystemStates);
        }

        public override void BuildGRBOptimizationModel()
        {
            //creates opf model for each block in the LDC
            OpfByBlock = new List<OpfModel>();
            foreach (var state in MyPowerSystemStates)
            {
                var opfBlock = new OpfModel(state, MyGrbEnv, MyGrbModel);
                opfBlock.BuildGRBOptimizationModel();
                OpfByBlock.Add(opfBlock);
            }
        }

        /// <summary>
        /// Build detailed results for this LDC OPF.
        /// </summary>
        /// <returns>An encapsulator of the detailed results of this LDC OPF.</returns>
        public LdcOpfModelResults BuildLDCOPFModelResults()
        {
            int status = MyGrbModel.Get(GRB.IntAttr.Status);
            //detailed results for each block
            OpfResultsByBlock = new List<OpfModelResult>();
            foreach (OpfModel opfModel in this.OpfByBlock)
            {
                OpfResultsByBlock.Add(opfModel.BuildOPFModelResults());
            }
            //detailed results for this LDC OPF model (aggregate and in per node)
            MyDetailedLDCOPFModelResults = new LdcOpfModelResults(status, ObjVal, OpfResultsByBlock, MyPowerSystemStates[0].MyPowerSystem.Nodes);
            return MyDetailedLDCOPFModelResults;
        }
    }
}
