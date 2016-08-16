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
    public class OPFModel
    {
        public const string OPFModelName = "Linear Optimal (DC) Power Flow";

        protected PowerSystem powerSystem;

        protected GRBEnv env;
        protected GRBModel model;

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
        /// Gets the total generation cost (the model objective value) of this OPF.
        /// </summary>
        protected double TotalGenerationCost
        {
            get
            {
                return this.model.Get(GRB.DoubleAttr.ObjVal);
            }
        }
        /// <summary>
        /// Gets the power generated by each generator in the current solution of the optimal power flow.
        /// </summary>
        protected double[] PGen_Solution
        {
            get
            {
                return this.model.Get(GRB.DoubleAttr.X, this.PGen);
            }
        }
        /// <summary>
        /// Gets the power flow through each transmission line in the current solution of the OPF.
        /// </summary>
        protected double[] PFlow_Solution
        {
            get
            {
                return this.model.Get(GRB.DoubleAttr.X, this.PFlow);
            }
        }
        /// <summary>
        /// Gets the load shedding in each node in the current solution of the OPF.
        /// </summary>
        protected double[] LShed_Solution
        {
            get
            {
                return this.model.Get(GRB.DoubleAttr.X, this.LoadShed);
            }
        }
        protected double[] BusAng_Solution
        {
            get
            {
                return this.model.Get(GRB.DoubleAttr.X, this.BusAngle);
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
                return this.model.Get(GRB.DoubleAttr.Pi, this.NodalPowerBalance);
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
            this.env = new GRBEnv();
            this.model = new GRBModel(env);
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
            this.env = env;
            this.model = model;
            this.BuildOPFModel();
        }

        /// <summary>
        /// Adds variables, constraints and objective function for simple OPF.
        /// </summary>
        public void BuildOPFModel()
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
            this.model.Update();
            // Sets objective: minimize total generation costs
            this.model.Set(GRB.IntAttr.ModelSense, GRB.MINIMIZE);
            // Creates power balance constraint in each node
            this.AddGRBConstrPowerBalance();
            // Creates constraint for defining power flow in each transmission line
            this.AddGRBConstrDCPowerFlow();
        }

        /// <summary>
        /// Creates Gurobi constraints of DC Power flow.
        /// </summary>
        /// <param name="powerSystem"></param>
        /// <remarks>The DC power flow constraints relate active power flow through branches to node angles:
        /// \f[
        ///     P_{flow,i \to j} = B_{ij} \left( \theta_i - \theta_j \right)
        /// \f]</remarks>
        protected void AddGRBConstrDCPowerFlow()
        {
            this.DCPowerFlow = new GRBConstr[powerSystem.NumberOfTransmissionLines];
            for (int t = 0; t < powerSystem.NumberOfTransmissionLines; t++)
            {
                TransmissionLine tl = powerSystem.TransmissionLines[t];
                GRBLinExpr powerFlowLHS = new GRBLinExpr();
                powerFlowLHS.AddTerm(1, this.PFlow[t]);
                GRBLinExpr powerFlowRHS = new GRBLinExpr();
                powerFlowRHS.AddTerm(+tl.SusceptanceMho, this.BusAngle[tl.NodeFromID]);
                powerFlowRHS.AddTerm(-tl.SusceptanceMho, this.BusAngle[tl.NodeToID]);
                this.DCPowerFlow[t] = this.model.AddConstr(powerFlowLHS, GRB.EQUAL, powerFlowRHS, "PowerFlowTL" + t);
            }
        }

        protected virtual void AddGRBConstrPowerBalance()
        {
            this.NodalPowerBalance = new GRBConstr[powerSystem.NumberOfNodes];
            int load_shed_counter = 0;
            for (int i = 0; i < powerSystem.NumberOfNodes; i++)
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
                this.NodalPowerBalance[i] = this.model.AddConstr(powerBalanceLHS, GRB.EQUAL, powerBalanceRHS, "PowerBalanceNode" + i);
            }
        }

        protected void AddGRBVarsPFlow()
        {
            PFlow = new GRBVar[powerSystem.NumberOfTransmissionLines];
            for (int i = 0; i < powerSystem.NumberOfTransmissionLines; i++)
            {
                TransmissionLine tl = powerSystem.TransmissionLines[i];
                PFlow[i] = model.AddVar(-tl.ThermalCapacityMW, tl.ThermalCapacityMW, 0, GRB.CONTINUOUS, "PFlow" + tl.Id);
            }
        }

        protected virtual void AddGRBVarsLoadShed()
        {
            List<GRBVar> load_shed = new List<GRBVar>();
            foreach (Node node in powerSystem.Nodes)
            {
                if (node.TotalLoad > 0)
                {
                    load_shed.Add(model.AddVar(0, node.TotalLoad, powerSystem.LoadSheddingCost, GRB.CONTINUOUS, "LS" + node.Id));
                }
            }
            this.LoadShed = load_shed.ToArray<GRBVar>();
        }

        protected void AddGRBVarsBusAngles()
        {
            BusAngle = new GRBVar[powerSystem.NumberOfNodes];
            for (int i = 0; i < powerSystem.NumberOfNodes; i++)
            {
                BusAngle[i] = model.AddVar(-GRB.INFINITY, GRB.INFINITY, 0, GRB.CONTINUOUS, "theta" + powerSystem.Nodes[i].Id);
            }
        }

        protected void AddGRBVarsPGen()
        {
            PGen = new GRBVar[powerSystem.NumberOfGeneratingUnits];
            for (int i = 0; i < powerSystem.NumberOfGeneratingUnits; i++)
            {
                GeneratingUnit gen = powerSystem.GeneratingUnits[i];
                PGen[i] = model.AddVar(0, gen.InstalledCapacityMW, gen.MarginalCost, GRB.CONTINUOUS, "PGen" + gen.Id);
            }
        }

        /// <summary>
        /// Builds the object with the full results of this OPF model.
        /// </summary>
        /// <returns></returns>
        /// <remarks>This method should be called upon successful solution of the model.</remarks>
        public OPFModelResult BuildOPFModelResults()
        {
            int status = this.model.Get(GRB.IntAttr.Status);
            return new OPFModelResult(powerSystem, status, TotalGenerationCost, PGen_Solution, PFlow_Solution, LShed_Solution, BusAng_Solution, NodalSpotPrice);
        }
    }
    
    /// <summary>
    /// Linear programming OPF model considering a constant variation in load.
    /// </summary>
    /// <remarks>
    /// An OPF model (same as <see cref="OPFModel"/>) where each load is multiplied by the same constant factor.
    /// </remarks>
    public class OPFModelLoadChange : OPFModel
    {
        double _LoadMultiplier;
        public double LoadMultiplier { get { return this._LoadMultiplier; } }

        public OPFModelLoadChange(PowerSystem powerSystem, GRBEnv env, GRBModel model, double loadMultiplier)
            : base(powerSystem, env, model)
        {
            this._LoadMultiplier = loadMultiplier;
        }
        
        protected override void AddGRBVarsLoadShed()
        {
            List<GRBVar> load_shed = new List<GRBVar>();
            foreach (Node node in powerSystem.Nodes)
            {
                if (node.TotalLoad > 0)
                {
                    load_shed.Add(model.AddVar(0, node.TotalLoad * LoadMultiplier, powerSystem.LoadSheddingCost, GRB.CONTINUOUS, "LS" + node.Id));
                }
            }
            this.LoadShed = load_shed.ToArray<GRBVar>();
        }

        protected override void AddGRBConstrPowerBalance()
        {
            this.NodalPowerBalance = new GRBConstr[powerSystem.NumberOfNodes];
            int load_shed_counter = 0;
            for (int i = 0; i < powerSystem.NumberOfNodes; i++)
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
                powerBalanceRHS.AddConstant(node.TotalLoad * LoadMultiplier);
                if (node.TotalLoad > 0)
                {
                    powerBalanceRHS.AddTerm(-1, LoadShed[load_shed_counter]);
                    load_shed_counter++;
                }
                this.NodalPowerBalance[i] = this.model.AddConstr(powerBalanceLHS, GRB.EQUAL, powerBalanceRHS, "PowerBalanceNode" + i);
            }
        }

    }
}
