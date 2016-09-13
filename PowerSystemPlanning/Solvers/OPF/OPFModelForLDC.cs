using Gurobi;
using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.Solvers.LDCOPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Solvers.OPF
{
    /// <summary>
    /// Linear programming OPF model considering a constant variation in load and duration (to be part of an LDC production cost simulation).
    /// </summary>
    /// <remarks>
    /// An OPF model (same as <see cref="OPFModel"/>) where each load is multiplied by the same constant factor, and the generation costs of each generator are multiplied by the duration of the block.
    /// </remarks>
    public class OPFModelForLDC : OPFModel
    {
        protected LoadBlock LoadBlock { get; set; }

        public OPFModelForLDC(PowerSystem powerSystem, GRBEnv env, GRBModel model, LoadBlock loadBlock)
            : base(powerSystem, env, model)
        {
            this.LoadBlock = loadBlock;
        }

        protected override void AddGRBVarsPGen()
        {
            PGen = new GRBVar[powerSystem.NumberOfGeneratingUnits];
            for (int i = 0; i < powerSystem.NumberOfGeneratingUnits; i++)
            {
                GeneratingUnit gen = powerSystem.GeneratingUnits[i];
                PGen[i] = _grbModel.AddVar(0, gen.InstalledCapacityMW,
                    gen.MarginalCost * this.LoadBlock.Duration, GRB.CONTINUOUS, "PGen" + gen.Id);
            }
        }

        protected override void AddGRBVarsLoadShed()
        {
            List<GRBVar> load_shed = new List<GRBVar>();
            foreach (Node node in powerSystem.Nodes)
            {
                if (node.TotalLoad > 0)
                {
                    load_shed.Add(_grbModel.AddVar(0, node.TotalLoad * LoadBlock.LoadMultiplier,
                        powerSystem.LoadSheddingCost * LoadBlock.Duration, GRB.CONTINUOUS, "LS" + node.Id));
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
                powerBalanceRHS.AddConstant(node.TotalLoad * LoadBlock.LoadMultiplier);
                if (node.TotalLoad > 0)
                {
                    powerBalanceRHS.AddTerm(-1, LoadShed[load_shed_counter]);
                    load_shed_counter++;
                }
                this.NodalPowerBalance[i] = this._grbModel.AddConstr(powerBalanceLHS, GRB.EQUAL, powerBalanceRHS, "PowerBalanceNode" + i);
            }
        }

        public override OPFModelResult BuildOPFModelResults()
        {
            return this.BuildOPFModelResultsForLDC();
        }

        public OPFModelResultForLDC BuildOPFModelResultsForLDC()
        {
            int status = this._grbModel.Get(GRB.IntAttr.Status);
            return new OPFModelResultForLDC(powerSystem, status, ObjVal_Solution, PGen_Solution, PFlow_Solution, LShed_Solution, BusAng_Solution, NodalSpotPrice, LoadBlock);
        }
    }
}
