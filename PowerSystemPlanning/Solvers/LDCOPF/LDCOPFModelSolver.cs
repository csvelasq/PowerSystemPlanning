using Gurobi;
using PowerSystemPlanning.Solvers.OPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.LDCOPF
{
    public class LDCOPFModelSolver : BaseOptimizationPowerSystemSolver
    {
        LoadDurationCurveByBlocks _DurationCurveBlocks;
        public LoadDurationCurveByBlocks DurationCurveBlocks
        {
            get { return this._DurationCurveBlocks; }
            protected set { this._DurationCurveBlocks = value; }
        }
        
        public LDCOPFModel LDCOPFModel { get; protected set; }
        public override IGRBOptimizationModel GRBOptimizationModel { get { return this.LDCOPFModel; } }
        
        /// <summary>
        /// The detailed results of this OPF model (per node, generator, and transmission line).
        /// </summary>
        public LDCOPFModelResults LDCOPFResults { get; protected set; }
        public override BaseGRBOptimizationModelResult GRBOptimizationModelResults { get { return this.LDCOPFResults; } }

        public LDCOPFModelSolver(PowerSystem powerSystem, LoadDurationCurveByBlocks durationCurveBlocks)
            : base(powerSystem)
        {
            this.DurationCurveBlocks = durationCurveBlocks;
        }

        public override void Build()
        {
            this.LDCOPFModel = new LDCOPFModel(this.powerSystem, this.DurationCurveBlocks, this._grbEnv, this._grbModel);
            this.LDCOPFModel.BuildGRBOptimizationModel();
        }

        public override void BuildOptimizationModelResults()
        {
            if (GRBModelStatus == GRB.Status.OPTIMAL)
            {
                this.LDCOPFResults = LDCOPFModel.BuildLDCOPFModelResults();
                //basegrb results are automatically pointing to the specific LDCOPF results
            }
            else
            {
                this.LDCOPFResults = new LDCOPFModelResults(GRBModelStatus);
            }
        }
    }
}
