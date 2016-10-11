using PowerSystemPlanning.Models.Planning.InvestmentBranch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;
using System.IO;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios
{
    [DataContract()]
    public class BindingTepModel : SerializableBindableBase
    {
        [DataMember()]
        public PowerSystem MyPowerSystem { get; protected set; }

        BindingList<CandidateTransmissionLine> _MyCandidateTransmissionLines;
        /// <summary>
        /// The list of candidate transmission lines for TEP.
        /// </summary>
        [DataMember()]
        public BindingList<CandidateTransmissionLine> MyCandidateTransmissionLines
        {
            get { return _MyCandidateTransmissionLines; }
            set
            {
                SetProperty<BindingList<CandidateTransmissionLine>>(ref _MyCandidateTransmissionLines, value);
            }
        }

        private BindingScenarioCollection _MyScenarios;
        /// <summary>
        /// The scenarios under which transmission expansion plans will be assessed.
        /// </summary>
        [DataMember()]
        public BindingScenarioCollection MyScenarios
        {
            get { return _MyScenarios; }
            set { SetProperty<BindingScenarioCollection>(ref _MyScenarios, value); }
        }

        private double _OperationCostsMultiplierInObjectiveFunction;
        /// <summary>
        /// Adimensional factor that multiplies operation costs in objective function (e.g. NPV factor).
        /// </summary>
        [DataMember()]
        public double OperationCostsMultiplierInObjectiveFunction
        {
            get { return _OperationCostsMultiplierInObjectiveFunction; }
            set { SetProperty<double>(ref _OperationCostsMultiplierInObjectiveFunction, value); }
        }

        private double _InvestmentCostsMultiplierInObjectiveFunction;
        /// <summary>
        /// Adimensional factor that multiplies investment costs in objective function.
        /// </summary>
        [DataMember()]
        public double InvestmentCostsMultiplierInObjectiveFunction
        {
            get { return _InvestmentCostsMultiplierInObjectiveFunction; }
            set { SetProperty<double>(ref _InvestmentCostsMultiplierInObjectiveFunction, value); }
        }

        public BindingTepModel()
        {
        }

        public BindingTepModel(PowerSystem system) : this()
        {
            MyPowerSystem = system;
        }

        public void SaveToXml(string xmlPath)
        {
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            var dcs = new DataContractSerializer(typeof(BindingTepModel), dcsSettings);
            var xmlSettings = new XmlWriterSettings { Indent = true };
            using (var myXmlWriter = XmlWriter.Create(xmlPath, xmlSettings))
            {
                dcs.WriteObject(myXmlWriter, this);
            }
        }

        public static BindingTepModel LoadFromXml(string xmlPath) =>
            MixedUtils.LoadFromXml<BindingTepModel>(xmlPath);
        /*
    public static BindingTepModel LoadFromXml(string xmlPath)
    {
        //Opens the scenarios/states definition
        var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
        var dcs = new DataContractSerializer(typeof(BindingTepModel), dcsSettings);
        var fs = new FileStream(xmlPath, FileMode.Open);
        var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
        var deserializedTepModel = (BindingTepModel)dcs.ReadObject(reader);
        return deserializedTepModel;
    }
        */
    }
}
