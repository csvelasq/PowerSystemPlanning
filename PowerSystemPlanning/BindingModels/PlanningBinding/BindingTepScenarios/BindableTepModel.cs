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
using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using PowerSystemPlanning.Models.Planning.Scenarios;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios
{
    [DataContract()]
    public class BindableTepModel : SerializableBindableBase, IStaticScenarioTepModel
    {
        #region internal fields
        BindingList<CandidateTransmissionLine> _MyCandidateTransmissionLines;
        private BindableStaticScenarioCollection _MyScenarios;
        private double _OperationCostsMultiplierInObjectiveFunction;
        private double _InvestmentCostsMultiplierInObjectiveFunction;
        #endregion

        #region Interface implementation
        IList<ICandidateTransmissionLine> IStaticScenarioTepModel.MyCandidateTransmissionLines
            => (from line in MyBindableCandidateTransmissionLines
                select (ICandidateTransmissionLine)line).ToList();

        IStaticScenarioCollection IStaticScenarioTepModel.MyScenarios
            => MyBindableScenarios;
        #endregion

        [DataMember()]
        public PowerSystem BindablePowerSystem { get; protected set; }

        /// <summary>
        /// The list of candidate transmission lines for TEP.
        /// </summary>
        [DataMember()]
        public BindingList<CandidateTransmissionLine> MyBindableCandidateTransmissionLines
        {
            get { return _MyCandidateTransmissionLines; }
            set
            {
                SetProperty<BindingList<CandidateTransmissionLine>>(ref _MyCandidateTransmissionLines, value);
            }
        }

        /// <summary>
        /// The scenarios under which transmission expansion plans will be assessed.
        /// </summary>
        [DataMember()]
        public BindableStaticScenarioCollection MyBindableScenarios
        {
            get { return _MyScenarios; }
            set { SetProperty<BindableStaticScenarioCollection>(ref _MyScenarios, value); }
        }

        /// <summary>
        /// Adimensional factor that multiplies operation costs in objective function (e.g. NPV factor).
        /// </summary>
        [DataMember()]
        public double OperationCostsMultiplierInObjectiveFunction
        {
            get { return _OperationCostsMultiplierInObjectiveFunction; }
            set { SetProperty<double>(ref _OperationCostsMultiplierInObjectiveFunction, value); }
        }

        /// <summary>
        /// Adimensional factor that multiplies investment costs in objective function.
        /// </summary>
        [DataMember()]
        public double InvestmentCostsMultiplierInObjectiveFunction
        {
            get { return _InvestmentCostsMultiplierInObjectiveFunction; }
            set { SetProperty<double>(ref _InvestmentCostsMultiplierInObjectiveFunction, value); }
        }

        public BindableTepModel()
        {
        }

        public BindableTepModel(PowerSystem system) : this()
        {
            BindablePowerSystem = system;
            MyBindableScenarios = new BindableStaticScenarioCollection(system);
            MyBindableCandidateTransmissionLines = new BindingList<CandidateTransmissionLine>();
            foreach (var transmissionLine in system.BindingTransmissionLines)
            {
                MyBindableCandidateTransmissionLines.Add(new CandidateTransmissionLine(transmissionLine));
            }
        }

        public void SaveToXml(string xmlPath)
        {
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            var dcs = new DataContractSerializer(typeof(BindableTepModel), dcsSettings);
            var xmlSettings = new XmlWriterSettings { Indent = true };
            using (var myXmlWriter = XmlWriter.Create(xmlPath, xmlSettings))
            {
                dcs.WriteObject(myXmlWriter, this);
            }
        }

        public static BindableTepModel LoadFromXml(string xmlPath) =>
            MixedUtils.LoadFromXml<BindableTepModel>(xmlPath);
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
