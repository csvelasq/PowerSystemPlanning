using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using PowerSystemPlanning.PlanningModels.Planning;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PowerSystemPlanning.PlanningModels
{
    public class ScenarioTEPModel : ScenarioPowerSystemPlanningModel
    {
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public BindingList<CandidateTransmissionLine> _MyCandidateTransmissionLines;
        /// <summary>
        /// The list of candidate transmission lines for TEP.
        /// </summary>
        public BindingList<CandidateTransmissionLine> MyCandidateTransmissionLines
        {
            get { return _MyCandidateTransmissionLines; }
            set
            {
                if (_MyCandidateTransmissionLines != value)
                {
                    _MyCandidateTransmissionLines = value;
                    // New candidate transmission lines are bound to the power system in the first scenario 
                    // (in order to automatically assign a unique ID to each Candidate Transmission Line)
                    // TODO find better solution for adding new candidate transmission lines
                    _MyCandidateTransmissionLines.AddingNew += (sender, e) =>
                    {
                        e.NewObject = new CandidateTransmissionLine(MyScenarios[0].MyPowerSystem.Nodes, MyCandidateTransmissionLines);
                    };
                    NotifyPropertyChanged();
                }
            }
        }
        public long TransmissionExpansionPlansCount
        {
            get
            {
                return (1 << MyCandidateTransmissionLines.Count); //2^N
            }
        }

        /// <summary>
        /// XML serializer for this class.
        /// </summary>
        // overrides inherited serializer in order to use the same inherited save methods
        public override XmlSerializer MyXmlSerializer { get { return new XmlSerializer(typeof(ScenarioTEPModel)); } }

        public ScenarioTEPModel() : base()
        {
            MyCandidateTransmissionLines = new BindingList<CandidateTransmissionLine>();
        }

        public ScenarioTEPModel(string name, double yearlyDiscountRate, LoadDurationCurveByBlocks myLoadDurationCurve) : base(name, yearlyDiscountRate, myLoadDurationCurve)
        {
            MyCandidateTransmissionLines = new BindingList<CandidateTransmissionLine>();
        }

        /// <summary>
        /// Enumerates all possible alternative transmission expansion plans (combining candidate transmission lines to be built).
        /// </summary>
        /// <returns>A list containing all possible alternative transmission expansion plans.</returns>
        public List<TransmissionExpansionPlan> EnumerateAlternativeTransmissionExpansionPlans()
        {
            MOOScenarioTEP myMOOScenarioTEP = new MOOScenarioTEP(this);
            List<TransmissionExpansionPlan> alternatives = new List<TransmissionExpansionPlan>();
            foreach (var builtLines in ScenarioTEPModel.GetPowerSet<CandidateTransmissionLine>(MyCandidateTransmissionLines))
            {
                TransmissionExpansionPlan alternative = new TransmissionExpansionPlan(builtLines.ToList(), this, myMOOScenarioTEP);
                alternatives.Add(alternative);
            }
            return alternatives;
        }

        public static IEnumerable<IEnumerable<T>> GetPowerSet<T>(IList<T> list)
        {
            // code from https://rosettacode.org/wiki/Power_set#C.23
            // an enumerator can be easily implemented by iterating through the range;
            // and returning the "select from..." in each iteration
            return from m in Enumerable.Range(0, 1 << list.Count) //range 0..2^N
                   select
                       from i in Enumerable.Range(0, list.Count) //range 0..N
                       where (m & (1 << i)) != 0 //binary coding
                       select list[i];
        }

        /// <summary>
        /// Creates a Power System Model by deserializing an XML file.
        /// </summary>
        /// <param name="xmlStream">Full file path of the XML file where the power system model was serialized.</param>
        /// <returns>A new power system model object with the contents serialized in the XML file.</returns>
        /// <remarks>
        /// The provided stream must is not closed within this method.
        /// IOException and other exceptions are not managed.</remarks>
        public static ScenarioTEPModel readFromXMLFile(string filename)
        {
            ScenarioTEPModel retval = null;
            using (StreamReader xmlStream = new StreamReader(filename))
            {
                XmlSerializer reader = new XmlSerializer(typeof(ScenarioTEPModel));
                retval = (ScenarioTEPModel)reader.Deserialize(xmlStream);
                foreach (PowerSystemScenario scenario in retval.MyScenarios)
                {
                    foreach (Node node in scenario.MyPowerSystem.Nodes)
                    {
                        node.PowerSystem = scenario.MyPowerSystem;
                    }
                }
            }
            return retval;
        }
    }
}
