﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurobi;

namespace PowerSystemPlanning.Solvers.OPF
{
    /// <summary>
    /// Linear programming optimal power flow model (DC power flow, linear generation cost functions).
    /// </summary>
    /// <remarks>
    /// The most simple OPF model based on DC power flow. Load shedding is allowed (modeled as fictitious generation) so the problem is always feasible (by shedding all load in every bus of the power system).
    /// </remarks>
    public class OPFModel : IGRBOptimizationModel
    {
        public string GRBOptimizationModelName { get { return "Linear Optimal (DC) Power Flow"; } }

        protected PowerSystem powerSystem;

        protected GRBEnv _grbEnv;
        protected GRBModel _grbModel;
        public GRBEnv grbEnv { get { return this._grbEnv; } }
        public GRBModel grbModel { get { return this._grbModel; } }

        #region Gurobi model vars and constraints
        /// <summary>
        /// Power generated by each generator (in MW).
        /// </summary>
        protected GRBVar[] PGen;
        /// <summary>
        /// Power flow through each transmission line in the power system (in MW).
        /// </summary>
        protected GRBVar[] PFlow;
        /// <summary>
        /// Load shedding (in MW).
        /// </summary>
        /// <remarks>Including load shedding in the model allows it to be always feasible with null power flows in every branch.</remarks>
        protected GRBVar[] LoadShed;
        /// <summary>
        /// Angle (in radians) of each bus (including reference).
        /// </summary>
        protected GRBVar[] BusAngle;
        /// <summary>
        /// Nodal power balance (for each node).
        /// </summary>
        protected GRBConstr[] NodalPowerBalance;
        /// <summary>
        /// DC power flow equations (for each transmission line).
        /// </summary>
        protected GRBConstr[] DCPowerFlow;
        #endregion

        #region Results of the model: obj value, vars and shadow prices
        /// <summary>
        /// Gets the total operation cost (the model objective value) of this OPF.
        /// </summary>
        protected double ObjVal_Solution
        {
            get
            {
                return this._grbModel.Get(GRB.DoubleAttr.ObjVal);
            }
        }
        /// <summary>
        /// Gets the power generated by each generator in the current solution of the optimal power flow.
        /// </summary>
        protected double[] PGen_Solution
        {
            get
            {
                return this._grbModel.Get(GRB.DoubleAttr.X, this.PGen);
            }
        }
        /// <summary>
        /// Gets the power flow through each transmission line in the current solution of the OPF.
        /// </summary>
        protected double[] PFlow_Solution
        {
            get
            {
                return this._grbModel.Get(GRB.DoubleAttr.X, this.PFlow);
            }
        }
        /// <summary>
        /// Gets the load shedding in each node in the current solution of the OPF.
        /// </summary>
        protected double[] LShed_Solution
        {
            get
            {
                return this._grbModel.Get(GRB.DoubleAttr.X, this.LoadShed);
            }
        }
        protected double[] BusAng_Solution
        {
            get
            {
                return this._grbModel.Get(GRB.DoubleAttr.X, this.BusAngle);
            }
        }
        /// <summary>
        /// The spot price (US$/MW) in each node of the power system.
        /// </summary>
        /// <remarks>
        /// The nodal spot price (also known as locational marginal price) is equal to the shadow price of the power balance constraint. Since all loads are assumed to be inelastic, the market-clearing price is simply the cost of satisfying one additional MW of load in the given node. Absent transmission congestion and load shedding, there will be one unique price in all nodes, and it will be equal to the marginal cost of the most expensive generator running.
        /// </remarks>
        protected double[] NodalSpotPrice
        {
            get
            {
                return this._grbModel.Get(GRB.DoubleAttr.Pi, this.NodalPowerBalance);
            }
        }
        #endregion

        /// <summary>
        /// Creates the Gurobi OPF model for the provided power system.
        /// </summary>
        /// <param name="powerSystem">The power system for which the OPF will be solved.</param>
        public OPFModel(PowerSystem powerSystem)
        {
            this.powerSystem = powerSystem;
            this._grbEnv = new GRBEnv();
            this._grbModel = new GRBModel(_grbEnv);
        }

        /// <summary>
        /// Adds the Gurobi OPF model for the provided power system, to the provided gurobi model.
        /// </summary>
        /// <param name="powerSystem"></param>
        /// <param name="env"></param>
        /// <param name="model"></param>
        public OPFModel(PowerSystem powerSystem, GRBEnv env, GRBModel model)
        {
            this.powerSystem = powerSystem;
            this._grbEnv = env;
            this._grbModel = model;
        }

        /// <summary>
        /// Adds variables, constraints and objective function for simple OPF.
        /// </summary>
        public void BuildGRBOptimizationModel()
        {
            // Add variables to Gurobi model: power generated by each generator
            this.AddGRBVarsPGen();
            // Add variables to Gurobi model: bus angles
            this.AddGRBVarsBusAngles();
            // Add variables to Gurobi model: fictitious power generated in each bus (load shedding)
            this.AddGRBVarsLoadShed();
            // Add variables to Gurobi model: power flow through each transmission line
            this.AddGRBVarsPFlow();
            // Includes variables in model
            this._grbModel.Update();
            // Sets objective: minimize total generation costs
            this._grbModel.Set(GRB.IntAttr.ModelSense, GRB.MINIMIZE);
            // Creates power balance constraint in each node
            this.AddGRBConstrPowerBalance();
            // Creates constraint for defining power flow in each transmission line
            this.AddGRBConstrDCPowerFlow();
        }

        protected void AddGRBVarsPFlow()
        {
            PFlow = new GRBVar[powerSystem.TransmissionLines.Count];
            for (int i = 0; i < powerSystem.TransmissionLines.Count; i++)
            {
                TransmissionLine tl = powerSystem.TransmissionLines[i];
                PFlow[i] = _grbModel.AddVar(-tl.ThermalCapacityMW, tl.ThermalCapacityMW, 0, GRB.CONTINUOUS, "PFlow" + tl.Id);
            }
        }

        protected virtual void AddGRBVarsLoadShed()
        {
            List<GRBVar> load_shed = new List<GRBVar>();
            foreach (Node node in powerSystem.Nodes)
            {
                if (node.TotalLoad > 0)
                {
                    load_shed.Add(_grbModel.AddVar(0, node.TotalLoad, powerSystem.LoadSheddingCost, GRB.CONTINUOUS, "LS" + node.Id));
                }
            }
            this.LoadShed = load_shed.ToArray<GRBVar>();
        }

        protected void AddGRBVarsBusAngles()
        {
            BusAngle = new GRBVar[powerSystem.Nodes.Count];
            //Adds reference bus angle (equals 0)
            BusAngle[0] = _grbModel.AddVar(0, 0, 0, GRB.CONTINUOUS, "theta" + powerSystem.Nodes[0].Id);
            //Adds the rest of the bus angles
            for (int i = 1; i < powerSystem.Nodes.Count; i++)
            {
                BusAngle[i] = _grbModel.AddVar(-GRB.INFINITY, GRB.INFINITY, 0, GRB.CONTINUOUS, "theta" + powerSystem.Nodes[i].Id);
            }
        }

        protected virtual void AddGRBVarsPGen()
        {
            PGen = new GRBVar[powerSystem.GeneratingUnits.Count];
            for (int i = 0; i < powerSystem.GeneratingUnits.Count; i++)
            {
                GeneratingUnit gen = powerSystem.GeneratingUnits[i];
                PGen[i] = _grbModel.AddVar(0, gen.InstalledCapacityMW, gen.MarginalCost, GRB.CONTINUOUS, "PGen" + gen.Id);
            }
        }

        protected virtual void AddGRBConstrPowerBalance()
        {
            this.NodalPowerBalance = new GRBConstr[powerSystem.Nodes.Count];
            int load_shed_counter = 0;
            for (int i = 0; i < powerSystem.Nodes.Count; i++)
            {
                Node node = powerSystem.Nodes[i];
                GRBLinExpr powerBalanceLHS = new GRBLinExpr();
                foreach (GeneratingUnit gen in node.GeneratingUnits)
                {
                    powerBalanceLHS.AddTerm(1, this.PGen[gen.Id]);
                }
                foreach (TransmissionLine tl in node.IncomingTransmissionLines)
                {
                    powerBalanceLHS.AddTerm(+1, PFlow[tl.Id]); //incoming power flow
                }
                foreach (TransmissionLine tl in node.OutgoingTransmissionLines)
                {
                    powerBalanceLHS.AddTerm(-1, PFlow[tl.Id]); //outgoing power flow
                }
                GRBLinExpr powerBalanceRHS = new GRBLinExpr();
                powerBalanceRHS.AddConstant(node.TotalLoad);
                if (node.TotalLoad > 0)
                {
                    powerBalanceRHS.AddTerm(-1, LoadShed[load_shed_counter]);
                    load_shed_counter++;
                }
                this.NodalPowerBalance[i] = this._grbModel.AddConstr(powerBalanceLHS, GRB.EQUAL, powerBalanceRHS, "PowerBalanceNode" + i);
            }
        }

        protected void AddGRBConstrDCPowerFlow()
        {
            this.DCPowerFlow = new GRBConstr[powerSystem.TransmissionLines.Count];
            for (int t = 0; t < powerSystem.TransmissionLines.Count; t++)
            {
                TransmissionLine tl = powerSystem.TransmissionLines[t];
                GRBLinExpr powerFlowLHS = new GRBLinExpr();
                powerFlowLHS.AddTerm(1, this.PFlow[t]);
                GRBLinExpr powerFlowRHS = new GRBLinExpr();
                powerFlowRHS.AddTerm(+tl.SusceptanceMho, this.BusAngle[tl.NodeFromID]);
                powerFlowRHS.AddTerm(-tl.SusceptanceMho, this.BusAngle[tl.NodeToID]);
                this.DCPowerFlow[t] = this._grbModel.AddConstr(powerFlowLHS, GRB.EQUAL, powerFlowRHS, "PowerFlowTL" + t);
            }
        }

        /// <summary>
        /// Builds the object with the full results of this OPF model.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This method should be called upon successful solution of the model.</remarks>
        public virtual OPFModelResult BuildOPFModelResults()
        {
            int status = this._grbModel.Get(GRB.IntAttr.Status);
            return new OPFModelResult(powerSystem, status, ObjVal_Solution, PGen_Solution, PFlow_Solution, LShed_Solution, BusAng_Solution, NodalSpotPrice);
        }
        
        public BaseGRBOptimizationModelResult BuildGRBOptimizationModelResults()
        {
            return this.BuildOPFModelResults();
        }
    }
}
