﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurobi;
using System.Diagnostics;

namespace PowerSystemPlanning.Solvers.OPF
{
    /// <summary>
    /// Linear programming optimal power flow model (DC power flow, linear generation cost functions).
    /// </summary>
    public class OPFModel : IPowerSystemSolver
    {
        public const string OPFModelName = "Linear Optimal (DC) Power Flow";

        #region Gurobi model
        PowerSystem powerSystem;
        GRBModel model;
        GRBEnv env;
        /// <summary>
        /// Power generated by each generator (in MW).
        /// </summary>
        GRBVar[] PGen;
        /// <summary>
        /// Power flow through each transmission line in the power system (in MW).
        /// </summary>
        GRBVar[] PFlow;
        /// <summary>
        /// Nodal power balance (for each node).
        /// </summary>
        GRBConstr[] NodalPowerBalance;
        /// <summary>
        /// DC power flow equations (for each transmission line).
        /// </summary>
        GRBConstr[] DCPowerFlow;
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the total generation cost (the model objective value) in the current 
        /// solution of the optimal power flow.
        /// </summary>
        public double TotalGenerationCost
        {
            get
            {
                return this.model.Get(GRB.DoubleAttr.ObjVal);
            }
        }

        /// <summary>
        /// Gets the power generated by each generator in the current solution of the optimal power flow.
        /// </summary>
        public double[] PGen_Solution
        {
            get
            {
                return this.model.Get(GRB.DoubleAttr.X, this.PGen);
            }
        }
        /// <summary>
        /// Gets the power flow through each transmission line in the current solution of the optimal power flow.
        /// </summary>
        public double[] PFlow_Solution
        {
            get
            {
                return this.model.Get(GRB.DoubleAttr.X, this.PFlow);
            }
        }
        #endregion 

        private PowerSystemSolverResults OPFSolverResults;

        /// <summary>
        /// Creates the Gurobi OPF model for the provided power system.
        /// </summary>
        /// <param name="powerSystem">The power system for which the OPF will be solved.</param>
        public OPFModel(PowerSystem powerSystem)
        {
            this.powerSystem = powerSystem;
            this.env = new GRBEnv();
            this.model = new GRBModel(env);
            // Add variables to Gurobi model: power generated by each generator
            PGen = new GRBVar[powerSystem.NumberOfGeneratingUnits];
            double[] coeffsPGenInPowerBalance = new double[powerSystem.NumberOfGeneratingUnits];
            for (int i = 0; i < powerSystem.NumberOfGeneratingUnits; i++)
            {
                GeneratingUnit gen = powerSystem.generatingUnits[i];
                PGen[i] = model.AddVar(0, gen.InstalledCapacityMW, gen.MarginalCost, GRB.CONTINUOUS, "PGen" + gen.Id);
                coeffsPGenInPowerBalance[i] = 1;
            }
            // Add variables to Gurobi model: power flow through each transmission line
            PFlow = new GRBVar[powerSystem.NumberOfTransmissionLines];
            double[] coeffsPFlowInPowerBalance = new double[powerSystem.NumberOfTransmissionLines];
            for (int i = 0; i < powerSystem.NumberOfTransmissionLines; i++)
            {
                TransmissionLine tl = powerSystem.transmissionLines[i];
                PFlow[i] = model.AddVar(0, tl.ThermalCapacityMW, 0, GRB.CONTINUOUS, "PFlow" + tl.Id);
                coeffsPGenInPowerBalance[i] = 1;
            }
            this.model.Update();
            // Sets objective: minimize total generation costs
            this.model.Set(GRB.IntAttr.ModelSense, GRB.MINIMIZE);
            // Creates power balance constraint in each node
            this.NodalPowerBalance = new GRBConstr[powerSystem.NumberOfNodes];
            for (int i = 0; i < powerSystem.NumberOfNodes; i++)
            {
                GRBLinExpr powerBalanceLHS = new GRBLinExpr();
                powerBalanceLHS.AddTerms(coeffsPGenInPowerBalance, PGen);
                GRBLinExpr powerBalanceRHS = new GRBLinExpr();
                powerBalanceRHS.AddConstant(powerSystem.TotalMWInelasticLoads);
                NodalPowerBalance[i] = model.AddConstr(powerBalanceLHS, GRB.EQUAL, powerBalanceRHS, "PowerBalance");
            }
        }

        /// <summary>
        /// Synchronously solves the Gurobi linear optimal (DC) power flow model.
        /// </summary>
        public void Solve()
        {
            // Initializes result reporting
            this.OPFSolverResults = new PowerSystemSolverResults();
            this.OPFSolverResults.SolverName = OPFModel.OPFModelName;
            this.OPFSolverResults.StartTime = DateTime.Now;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            // Solves the model
            this.model.Optimize();
            // Finalizes result reporting
            this.OPFSolverResults.Result = new OPFModelResult(this.TotalGenerationCost, this.PGen_Solution, this.PFlow_Solution);
            stopwatch.Stop();
            this.OPFSolverResults.ExecutionTime = stopwatch.Elapsed;
            this.OPFSolverResults.StopTime = DateTime.Now;
        }

        public void SolveAsync()
        {
            throw new NotImplementedException();
        }

        public PowerSystemSolverResults getResults()
        {
            return this.OPFSolverResults;
        }

        /// <summary>
        /// Dispose of Gurobi model and env
        /// </summary>
        public void Dispose()
        {
            model.Dispose();
            env.Dispose();
        }
    }

    /// <summary>
    /// Encapsulates results of the OPF model.
    /// </summary>
    public class OPFModelResult : PowerSystemSolverResults
    {
        double _TotalGenerationCost;
        double[] _PowerGenerated;
        double[] _PowerFlows;
        /// <summary>
        /// Gets the total generation cost (the model objective value) in the current solution of the optimal power flow.
        /// </summary>
        public double TotalGenerationCost
        {
            get
            {
                return _TotalGenerationCost;
            }
        }

        /// <summary>
        /// Power generated by each generator (in MW).
        /// </summary>
        public double[] PowerGenerated
        {
            get
            {
                return _PowerGenerated;
            }
        }

        /// <summary>
        /// Power flow through each transmission line in the power system (in MW).
        /// </summary>
        public double[] PowerFlows
        {
            get
            {
                return _PowerFlows;
            }

            set
            {
                _PowerFlows = value;
            }
        }

        public OPFModelResult(double obj, double[] pgen, double[] pflows)
        {
            this._TotalGenerationCost = obj;
            this._PowerGenerated = pgen;
            this._PowerFlows = pflows;
        }
    }
}