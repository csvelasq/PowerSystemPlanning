﻿using System.Linq;
using Gurobi;
using PowerSystemPlanning.Models.SystemState;
using PowerSystemPlanning.Solvers.GRBOptimization;
using PowerSystemPlanning.Solvers.OPF.OpfResults;

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
        public override string GRBOptimizationModelName => "Linear Optimal (DC) Power Flow";

        public IPowerSystemState MyPowerSystemState { get; protected set; }

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
            PGen = new GRBVar[MyPowerSystemState.ActiveGeneratingUnitStates.Count()];
            int k = 0;
            foreach (var gen in MyPowerSystemState.ActiveGeneratingUnitStates)
            {
                PGen[k] = MyGrbModel.AddVar(0,
                    gen.AvailableCapacity,
                    gen.MarginalCost * MyPowerSystemState.Duration,
                    GRB.CONTINUOUS,
                    "PGen" + gen.UnderlyingGeneratingUnit.Id);
                k++;
            }
        }

        protected void AddGRBVarsPFlow()
        {
            PFlow = new GRBVar[MyPowerSystemState.ActiveSimpleTransmissionLineStates.Count()];
            int l = 0;
            foreach (var tl in MyPowerSystemState.ActiveSimpleTransmissionLineStates)
            {
                PFlow[l] = MyGrbModel.AddVar(-tl.AvailableThermalCapacity,
                    tl.AvailableThermalCapacity,
                    0,
                    GRB.CONTINUOUS,
                    "PFlow" + tl.UnderlyingTransmissionLine.Id);
                l++;
            }
        }

        protected void AddGRBVarsLoadShed()
        {
            this.LoadShed = new GRBVar[MyPowerSystemState.InelasticLoadStates.Count];
            int d = 0;
            foreach (var load in MyPowerSystemState.InelasticLoadStates)
            {
                LoadShed[d] = MyGrbModel.AddVar(0,
                    load.Consumption,
                    load.LoadSheddingCost * MyPowerSystemState.Duration,
                    GRB.CONTINUOUS,
                    "LS" + load.UnderlyingInelasticLoad.Id);
                d++;
            }
        }

        protected void AddGRBVarsBusAngles()
        {
            BusAngle = new GRBVar[MyPowerSystemState.NodeStates.Count];
            //Adds reference bus angle (equals 0)
            BusAngle[0] = MyGrbModel.AddVar(0, 0, 0, GRB.CONTINUOUS,
                "theta" + MyPowerSystemState.NodeStates[0].UnderlyingNode.Id);
            //Adds the rest of the bus angles
            for (int i = 1; i < MyPowerSystemState.NodeStates.Count; i++)
            {
                BusAngle[i] = MyGrbModel.AddVar(
                    -GRB.INFINITY,
                    GRB.INFINITY,
                    0,
                    GRB.CONTINUOUS,
                    "theta" + MyPowerSystemState.NodeStates[i].UnderlyingNode.Id);
            }
        }

        protected void AddGRBConstrPowerBalance()
        {
            this.NodalPowerBalance = new GRBConstr[MyPowerSystemState.NodeStates.Count];
            int b = 0; //node number
            foreach (var node in MyPowerSystemState.NodeStates)
            {
                GRBLinExpr powerBalanceLHS = new GRBLinExpr();
                //Power generated in this node
                foreach (var gen in node.ActiveGeneratingUnitStates)
                {
                    int genIndex = MyPowerSystemState.ActiveGeneratingUnitStates.IndexOf(gen);
                    powerBalanceLHS.AddTerm(1,
                        PGen[genIndex]);
                }
                //Incoming power flow (to this node)
                foreach (var tl in node.ActiveIncomingTransmissionLinesStates)
                {
                    powerBalanceLHS.AddTerm(+1,
                        PFlow[MyPowerSystemState.ActiveSimpleTransmissionLineStates.IndexOf(tl)]);
                }
                //Outgoing power flow (from this node)
                foreach (var tl in node.ActiveOutgoingTransmissionLinesStates)
                {
                    powerBalanceLHS.AddTerm(-1,
                        PFlow[MyPowerSystemState.ActiveSimpleTransmissionLineStates.IndexOf(tl)]); //outgoing power flow
                }
                //Inelastic loads connected to this node
                GRBLinExpr powerBalanceRHS = new GRBLinExpr();
                powerBalanceRHS.AddConstant(node.TotalInelasticLoad);
                //Load shedding
                foreach (var load in node.InelasticLoadsStates)
                {
                    powerBalanceRHS.AddTerm(-1,
                        LoadShed[MyPowerSystemState.InelasticLoadStates.IndexOf(load)]);
                }
                this.NodalPowerBalance[b] = MyGrbModel.AddConstr(powerBalanceLHS,
                    GRB.EQUAL,
                    powerBalanceRHS,
                    "PowerBalanceNode" + b);
            }
        }

        protected void AddGRBConstrDCPowerFlow()
        {
            DCPowerFlow = new GRBConstr[MyPowerSystemState.ActiveSimpleTransmissionLineStates.Count];
            int l = 0;
            foreach (var tl in MyPowerSystemState.ActiveSimpleTransmissionLineStates)
            {
                //PFlow variable (power flow throw this line)
                GRBLinExpr powerFlowLHS = new GRBLinExpr();
                powerFlowLHS.AddTerm(1,
                    PFlow[MyPowerSystemState.ActiveSimpleTransmissionLineStates.IndexOf(tl)]);
                //Power flow calculated with susceptance and bus angles
                GRBLinExpr powerFlowRHS = new GRBLinExpr();
                powerFlowRHS.AddTerm(+tl.UnderlyingTransmissionLine.SusceptanceMho,
                    BusAngle[MyPowerSystemState.MyPowerSystem.Nodes.IndexOf(tl.UnderlyingTransmissionLine.NodeFrom)]);
                powerFlowRHS.AddTerm(-tl.UnderlyingTransmissionLine.SusceptanceMho,
                    BusAngle[MyPowerSystemState.MyPowerSystem.Nodes.IndexOf(tl.UnderlyingTransmissionLine.NodeTo)]);
                //Add constraint
                DCPowerFlow[l] = MyGrbModel.AddConstr(
                    powerFlowLHS,
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
