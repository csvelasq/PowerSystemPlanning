using Gurobi;
using PowerSystemPlanning.Solvers.LDCOPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.OPF
{
    public class OPFModelSolverForLDC : BaseOptimizationPowerSystemSolver
    {
        public LoadBlock LoadBlock { get; protected set; }

        public OPFModelForLDC MyOPFModelForLDC { get; protected set; }
        public override IGRBOptimizationModel GRBOptimizationModel
        {
            get { return MyOPFModelForLDC; }
        }

        public OPFModelResultForLDC OPFModelResultsForLDC { get; protected set; }
        public override BaseGRBOptimizationModelResult GRBOptimizationModelResults
        {
            get { return OPFModelResultsForLDC; }
        }

        public OPFModelSolverForLDC(PowerSystem powerSystem, LoadBlock loadBlock)
            : base(powerSystem)
        {
            this.LoadBlock = loadBlock;
        }

        public override void Build()
        {
            this.MyOPFModelForLDC = new OPFModelForLDC(powerSystem, _grbEnv, _grbModel, LoadBlock);
            this.MyOPFModelForLDC.BuildGRBOptimizationModel();
        }

        public override void BuildOptimizationModelResults()
        {
            if (GRBModelStatus == GRB.Status.OPTIMAL)
            {
                this.OPFModelResultsForLDC = MyOPFModelForLDC.BuildOPFModelResultsForLDC();
                //basegrb results are automatically pointing to the specific LDCOPF results
            }
            else
            {
                this.OPFModelResultsForLDC = new OPFModelResultForLDC(GRBModelStatus);
            }
        }
    }
}
