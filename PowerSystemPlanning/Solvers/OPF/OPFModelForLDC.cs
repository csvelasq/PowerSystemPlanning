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
    
        /// <summary>
        /// The results of this OPF for LDC model (set by <see cref="BuildOPFModelResultsForLDC"/>).
        /// </summary>
        public OPFModelResultForLDC MyOPFModelResultForLDC { get; protected set; }

        public OPFModelForLDC(IPowerSystem powerSystem, GRBEnv env, GRBModel model, LoadBlock loadBlock)
            : base(powerSystem, env, model)
        {
            this.LoadBlock = loadBlock;
        }

        protected override void AddGRBVarsPGen()
        {
            PGen = new GRBVar[MyPowerSystem.GeneratingUnits.Count];
            for (int i = 0; i < MyPowerSystem.GeneratingUnits.Count; i++)
            {
                GeneratingUnit gen = MyPowerSystem.GeneratingUnits[i];
                PGen[i] = MyGrbModel.AddVar(0, gen.InstalledCapacityMW,
                    gen.MarginalCost * this.LoadBlock.Duration, GRB.CONTINUOUS, "PGen" + gen.Id);
            }
        }

        protected override void AddGRBVarsLoadShed()
        {
            List<GRBVar> load_shed = new List<GRBVar>();
            foreach (Node node in MyPowerSystem.Nodes)
            {
                if (node.TotalLoad > 0)
                {
                    load_shed.Add(MyGrbModel.AddVar(0, node.TotalLoad * LoadBlock.LoadMultiplier,
                        MyPowerSystem.LoadSheddingCost * LoadBlock.Duration, GRB.CONTINUOUS, "LS" + node.Id));
                }
            }
            this.LoadShed = load_shed.ToArray<GRBVar>();
        }

        protected override void AddGRBConstrPowerBalance()
        {
            this.NodalPowerBalance = new GRBConstr[MyPowerSystem.Nodes.Count];
            int load_shed_counter = 0;
            for (int i = 0; i < MyPowerSystem.Nodes.Count; i++)
            {
                Node node = MyPowerSystem.Nodes[i];
                GRBLinExpr powerBalanceLHS = new GRBLinExpr();
                foreach (GeneratingUnit gen in node.GeneratingUnits)
                {
                    powerBalanceLHS.AddTerm(1, this.PGen[gen.Id]);
                }
                foreach (TransmissionLine tl in node.IncomingTransmissionLines)
                {
                    powerBalanceLHS.AddTerm(+1, PFlow[PFlow_TLsIDs[tl.Id]]); //incoming power flow
                }
                foreach (TransmissionLine tl in node.OutgoingTransmissionLines)
                {
                    powerBalanceLHS.AddTerm(-1, PFlow[PFlow_TLsIDs[tl.Id]]); //outgoing power flow
                }
                GRBLinExpr powerBalanceRHS = new GRBLinExpr();
                powerBalanceRHS.AddConstant(node.TotalLoad * LoadBlock.LoadMultiplier);
                if (node.TotalLoad > 0)
                {
                    powerBalanceRHS.AddTerm(-1, LoadShed[load_shed_counter]);
                    load_shed_counter++;
                }
                this.NodalPowerBalance[i] = MyGrbModel.AddConstr(powerBalanceLHS, GRB.EQUAL, powerBalanceRHS, "PowerBalanceNode" + i);
            }
        }

        public OPFModelResultForLDC BuildOPFModelResultsForLDC()
        {
            MyOPFModelResultForLDC = new OPFModelResultForLDC(MyPowerSystem, GRBModelStatus, ObjVal, PGen_Solution, PFlow_Solution, LShed_Solution, BusAng_Solution, NodalSpotPrice, PFlow_TLsIDs, LoadBlock);
            return MyOPFModelResultForLDC;
        }
    }
}
