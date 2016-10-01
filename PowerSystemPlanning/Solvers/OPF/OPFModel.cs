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
    public class OPFModel : BaseGRBOptimizationModel
    {
        public override string GRBOptimizationModelName { get { return "Linear Optimal (DC) Power Flow"; } }

        protected IPowerSystemState MyPowerSystemState;

        /// <summary>
        /// The results of this simple OPF model (set by <see cref="BuildOPFModelResults"/>).
        /// </summary>
        public OPFModelResult MyOPFModelResult { get; protected set; }

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

        #region Results of the model: vars and shadow prices
        /// <summary>
        /// Gets the power generated by each generator in the current solution of the optimal power flow.
        /// </summary>
        protected double[] PGen_Solution
        {
            get
            {
                return MyGrbModel.Get(GRB.DoubleAttr.X, this.PGen);
            }
        }
        /// <summary>
        /// Gets the power flow through each transmission line in the current solution of the OPF.
        /// </summary>
        protected double[] PFlow_Solution
        {
            get
            {
                return MyGrbModel.Get(GRB.DoubleAttr.X, this.PFlow);
            }
        }
        /// <summary>
        /// Gets the load shedding in each load in the current solution of the OPF.
        /// </summary>
        protected double[] LShed_Solution
        {
            get
            {
                return MyGrbModel.Get(GRB.DoubleAttr.X, this.LoadShed);
            }
        }
        /// <summary>
        /// Gets the bus angle in each node in the current solution of the OPF.
        /// </summary>
        protected double[] BusAng_Solution
        {
            get
            {
                return MyGrbModel.Get(GRB.DoubleAttr.X, this.BusAngle);
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
                return MyGrbModel.Get(GRB.DoubleAttr.Pi, this.NodalPowerBalance);
            }
        }
        #endregion

        /// <summary>
        /// Creates the Gurobi OPF model for the provided power system.
        /// </summary>
        /// <param name="powerSystem">The power system for which the OPF will be solved.</param>
        public OPFModel(IPowerSystemState powerSystem) : base()
        {
            this.MyPowerSystemState = powerSystem;
        }

        /// <summary>
        /// Adds the Gurobi OPF model for the provided power system, to the provided gurobi model.
        /// </summary>
        /// <param name="powerSystem"></param>
        /// <param name="env"></param>
        /// <param name="model"></param>
        public OPFModel(IPowerSystemState powerSystem, GRBEnv env, GRBModel model) : base(env, model)
        {
            this.MyPowerSystemState = powerSystem;
        }

        /// <summary>
        /// Adds variables, constraints and objective function for simple OPF.
        /// </summary>
        public override void BuildGRBOptimizationModel()
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
            this.MyGrbModel.Update();
            // Sets objective: minimize total generation costs
            this.MyGrbModel.Set(GRB.IntAttr.ModelSense, GRB.MINIMIZE);
            // Creates power balance constraint in each node
            this.AddGRBConstrPowerBalance();
            // Creates constraint for defining power flow in each transmission line
            this.AddGRBConstrDCPowerFlow();
        }

        protected void AddGRBVarsPGen()
        {
            PGen = new GRBVar[MyPowerSystemState.GeneratingUnits.Count];
            int k = 0;
            foreach (GeneratingUnit gen in MyPowerSystemState.GeneratingUnits)
            {
                PGen[k] = MyGrbModel.AddVar(0,
                    gen.InstalledCapacityMW,
                    gen.MarginalCost * MyPowerSystemState.DurationHours,
                    GRB.CONTINUOUS,
                    "PGen" + gen.Id);
                k++;
            }
        }

        protected void AddGRBVarsPFlow()
        {
            PFlow = new GRBVar[MyPowerSystemState.TransmissionLines.Count];
            int l = 0;
            foreach (TransmissionLine tl in MyPowerSystemState.TransmissionLines)
            {
                PFlow[l] = MyGrbModel.AddVar(-tl.ThermalCapacityMW,
                    tl.ThermalCapacityMW,
                    0,
                    GRB.CONTINUOUS,
                    "PFlow" + tl.Id);
                l++;
            }
        }

        protected void AddGRBVarsLoadShed()
        {
            this.LoadShed = new GRBVar[MyPowerSystemState.InelasticLoads.Count];
            int d = 0;
            foreach (InelasticLoad load in MyPowerSystemState.InelasticLoads)
            {
                LoadShed[d] = MyGrbModel.AddVar(0,
                    load.ConsumptionMW,
                    MyPowerSystemState.LoadSheddingCost * MyPowerSystemState.DurationHours,
                    GRB.CONTINUOUS,
                    "LS" + load.Id);
                d++;
            }
        }

        protected void AddGRBVarsBusAngles()
        {
            BusAngle = new GRBVar[MyPowerSystemState.Nodes.Count];
            //Adds reference bus angle (equals 0)
            BusAngle[0] = MyGrbModel.AddVar(0, 0, 0, GRB.CONTINUOUS,
                "theta" + MyPowerSystemState.Nodes[0].Id);
            //Adds the rest of the bus angles
            for (int i = 1; i < MyPowerSystemState.Nodes.Count; i++)
            {
                BusAngle[i] = MyGrbModel.AddVar(-GRB.INFINITY, GRB.INFINITY, 0, GRB.CONTINUOUS, "theta" + MyPowerSystemState.Nodes[i].Id);
            }
        }

        protected void AddGRBConstrPowerBalance()
        {
            this.NodalPowerBalance = new GRBConstr[MyPowerSystemState.Nodes.Count];
            int b = 0;
            foreach (Node node in MyPowerSystemState.Nodes)
            {
                GRBLinExpr powerBalanceLHS = new GRBLinExpr();
                //Power generated in this node
                foreach (GeneratingUnit gen in node.GeneratingUnits)
                {
                    powerBalanceLHS.AddTerm(1,
                        PGen[MyPowerSystemState.GeneratingUnits.IndexOf(gen)]);
                }
                //Incoming power flow (to this node)
                foreach (TransmissionLine tl in node.IncomingTransmissionLines)
                {
                    powerBalanceLHS.AddTerm(+1,
                        PFlow[MyPowerSystemState.TransmissionLines.IndexOf(tl)]);
                }
                //Outgoing power flow (from this node)
                foreach (TransmissionLine tl in node.OutgoingTransmissionLines)
                {
                    powerBalanceLHS.AddTerm(-1,
                        PFlow[MyPowerSystemState.TransmissionLines.IndexOf(tl)]); //outgoing power flow
                }
                //Inelastic loads connected to this node
                GRBLinExpr powerBalanceRHS = new GRBLinExpr();
                powerBalanceRHS.AddConstant(node.TotalLoad);
                //Load shedding
                foreach (InelasticLoad load in node.InelasticLoads)
                {
                    powerBalanceRHS.AddTerm(-1,
                        LoadShed[MyPowerSystemState.InelasticLoads.IndexOf(load)]);
                }
                this.NodalPowerBalance[b] = MyGrbModel.AddConstr(powerBalanceLHS, 
                    GRB.EQUAL, 
                    powerBalanceRHS, 
                    "PowerBalanceNode" + b);
            }
        }

        protected void AddGRBConstrDCPowerFlow()
        {
            DCPowerFlow = new GRBConstr[MyPowerSystemState.TransmissionLines.Count];
            int l = 0;
            foreach (TransmissionLine tl in MyPowerSystemState.TransmissionLines)
            {
                GRBLinExpr powerFlowLHS = new GRBLinExpr();
                powerFlowLHS.AddTerm(1,
                    PFlow[MyPowerSystemState.TransmissionLines.IndexOf(tl)]);
                GRBLinExpr powerFlowRHS = new GRBLinExpr();
                powerFlowRHS.AddTerm(+tl.SusceptanceMho,
                    BusAngle[MyPowerSystemState.Nodes.IndexOf(tl.NodeFrom)]);
                powerFlowRHS.AddTerm(-tl.SusceptanceMho,
                    BusAngle[MyPowerSystemState.Nodes.IndexOf(tl.NodeTo)]);
                DCPowerFlow[l] = MyGrbModel.AddConstr(powerFlowLHS, 
                    GRB.EQUAL, 
                    powerFlowRHS, 
                    "PowerFlowTL" + l);
            }
        }

        /// <summary>
        /// Builds the object with the full results of this OPF model.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This method should be called upon successful solution of the model.</remarks>
        public OPFModelResult BuildOPFModelResults()
        {
            MyOPFModelResult = new OPFModelResult(MyPowerSystemState, GRBModelStatus, ObjVal, PGen_Solution, PFlow_Solution, LShed_Solution, BusAng_Solution, NodalSpotPrice);
            return MyOPFModelResult;
        }
    }
}
