using PowerSystemPlanning.MultiObjective;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios
{
    public class BindableStaticTransmissionExpansionPlan : BaseMultiObjectiveIndividual
    {
        public override List<double> ObjectiveValues
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BindableStaticTransmissionExpansionPlan(BaseMultiObjectiveOptimizationProblem myProblem) : base(myProblem)
        {
        }

        public override List<double> EvaluateObjectives()
        {
            throw new NotImplementedException();
        }
    }
}
