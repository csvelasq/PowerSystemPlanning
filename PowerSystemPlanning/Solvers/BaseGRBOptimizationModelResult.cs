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
        protected int _GRBStatus;

        /// <summary>
        /// Status of the Gurobi optimization (defined in GRB.Status)
        /// </summary>
        public int GRBStatus
        {
            get
            {
                return _GRBStatus;
            }

            set
            {
                _GRBStatus = value;
            }
        }

        /// <summary>
        /// True if the model was proved to be infeasible, false otherwise.
        /// </summary>
        public bool IsModelInfeasible { get { return _GRBStatus == GRB.Status.INFEASIBLE; } }

        /// <summary>
        /// True if the model was solved to optimality, false otherwise.
        /// </summary>
        public bool IsModelSolved { get { return _GRBStatus == GRB.Status.OPTIMAL; } }

        /// <summary>
        /// True if the model was proved to be unbounded, false otherwise.
        /// </summary>
        public bool IsModelUnbounded { get { return _GRBStatus == GRB.Status.UNBOUNDED; } }

        protected double _ObjVal;

        /// <summary>
        /// The value of the objective function.
        /// </summary>
        public double ObjVal
        {
            get
            {
                return this._ObjVal;
            }
        }

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
            this._GRBStatus = status;
            this._ObjVal = Double.NaN;
        }

        public BaseGRBOptimizationModelResult(int status, double objVal)
        {
            this._GRBStatus = status;
            this._ObjVal = objVal;
        }
    }
}
