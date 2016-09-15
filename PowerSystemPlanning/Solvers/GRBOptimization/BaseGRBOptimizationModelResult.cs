using Gurobi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers
{
    /// <summary>
    /// Encapsulates the general results of the Gurobi optimization process for a given model.
    /// </summary>
    /// <remarks>
    /// This class is meant to be instantiated once the optimization model is solved.
    /// </remarks>
    public class BaseGRBOptimizationModelResult
    {
        /// <summary>
        /// Status of the Gurobi optimization (defined in GRB.Status)
        /// </summary>
        public int GRBStatus { get; protected set; }

        /// <summary>
        /// True if the model was proved to be infeasible, false otherwise.
        /// </summary>
        public bool IsModelInfeasible { get { return GRBStatus == GRB.Status.INFEASIBLE; } }

        /// <summary>
        /// True if the model was solved to optimality, false otherwise.
        /// </summary>
        public bool IsModelSolved { get { return GRBStatus == GRB.Status.OPTIMAL; } }

        /// <summary>
        /// True if the model was proved to be unbounded, false otherwise.
        /// </summary>
        public bool IsModelUnbounded { get { return GRBStatus == GRB.Status.UNBOUNDED; } }

        /// <summary>
        /// The value of the objective function.
        /// </summary>
        public double ObjVal { get; protected set; }

        /// <summary>
        /// A message indicating the current state of the model (e.g. solved, infeasible).
        /// </summary>
        public string CurrentStateMessage
        {
            get
            {
                string retmsg = "";
                if (IsModelSolved)
                    retmsg = "Model solved to optimality.";
                if (IsModelInfeasible)
                    retmsg = "Model infeasible.";
                if (IsModelUnbounded)
                    retmsg = "Model unbounded.";
                return retmsg;
            }
        }

        public BaseGRBOptimizationModelResult(int status)
        {
            this.GRBStatus = status;
            this.ObjVal = Double.NaN;
        }

        public BaseGRBOptimizationModelResult(int status, double objVal)
        {
            this.GRBStatus = status;
            this.ObjVal = objVal;
        }
    }
}
