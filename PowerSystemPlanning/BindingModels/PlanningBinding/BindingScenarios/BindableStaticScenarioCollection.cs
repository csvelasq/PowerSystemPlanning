using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios;
using PowerSystemPlanning.BindingModels.StateBinding;
using PowerSystemPlanning.Models.Planning.Scenarios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PowerSystemPlanning.Models.SystemBaseData;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios
{
    /// <summary>
    /// A collection of scenarios bindable to GUI.
    /// </summary>
    [DataContract()]
    public class BindableStaticScenarioCollection : SerializableBindableBase, IStaticScenarioCollection
    {
        #region internal fields
        BindingList<BindableStaticScenario> _MyScenarios;
        ScenarioAndStateDataTableEditor _MyStateCollectionDt;
        #endregion

        #region interface implementation
        public IPowerSystem MyPowerSystem => BindablePowerSystem;

        public IList<IStaticScenario> MyStaticScenarios =>
            (from scenario in MyScenarios select (IStaticScenario)scenario).ToList();
        #endregion

        /// <summary>
        /// The underlying power system.
        /// </summary>
        [DataMember()]
        public PowerSystem BindablePowerSystem { get; protected set; }

        /// <summary>
        /// The set of scenarios and states of the power system.
        /// </summary>
        [DataMember()]
        public BindingList<BindableStaticScenario> MyScenarios
        {
            get { return _MyScenarios; }
            set { base.SetProperty(ref _MyScenarios, value); }
        }

        public BindableStaticScenarioCollection() { }

        public BindableStaticScenarioCollection(PowerSystem system) : this()
        {
            BindablePowerSystem = system;
            MyScenarios = new BindingList<BindableStaticScenario>();
        }

        public BindableStaticScenarioCollection(PowerSystem system,
            IList<BindableStaticScenario> scenarios) : this(system)
        {
            foreach (var scenario in scenarios)
            {
                MyScenarios.Add(scenario);
            }
        }

        #region Easy Edition
        /// <summary>
        /// An object useful for simultaneously editing all scenarios and states.
        /// </summary>
        public ScenarioAndStateDataTableEditor MyStateCollectionDt
        {
            get { return _MyStateCollectionDt; }
            set { SetProperty<ScenarioAndStateDataTableEditor>(ref _MyStateCollectionDt, value); }
        }

        public void CreateStateCollection()
        {
            MyStateCollectionDt = new ScenarioAndStateDataTableEditor(BindablePowerSystem, MyScenarios);
        }

        public void CommitStateCollectionToPowerSystemState()
        {
            MyStateCollectionDt.CommitStateCollectionToPowerSystemState();
        }

        /// <summary>
        /// Saves this study to a local folder.
        /// </summary>
        /// <param name="xmlAbsolutePath">The path to the XML file where <see cref="MyScenarios"/> will be saved.</param>
        /// <param name="csvAbsolutePath">The path to the CSV file where the datatable in <see cref="MyStateCollectionDt"/> will be saved.</param>
        public void SaveToLocalFolderForEdition(string xmlAbsolutePath, string csvAbsolutePath)
        {
            //Save the scenarios/states definition
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            DataContractSerializer dcs = new DataContractSerializer(typeof(BindingList<BindingScenarios.BindableStaticScenario>), dcsSettings);
            var xmlSettings = new XmlWriterSettings { Indent = true };
            using (var myXmlWriter = XmlWriter.Create(xmlAbsolutePath, xmlSettings))
            {
                dcs.WriteObject(myXmlWriter, MyScenarios);
            }
            //Saves the datatable data on states
            MyStateCollectionDt.SaveDtToCsv(csvAbsolutePath);
        }
        #endregion

        public static BindingList<BindableStaticScenario> CreateDefaultScenarios(PowerSystem system)
        {
            var scenarios = new BindingList<BindableStaticScenario>();
            //Default scenarios with default states
            var states = BindableStaticStateCollection.CreateDefaultStateCollection(system);
            scenarios.Add(new BindableStaticScenario(system, "Scenario 1", .5, states.BindableStates));
            states = BindableStaticStateCollection.CreateDefaultStateCollection(system);
            scenarios.Add(new BindableStaticScenario(system, "Scenario 2", .5, states.BindableStates));
            return scenarios;
        }

        /*
        public static BindableStaticScenario LoadFromFolder(PowerSystem system, string xmlAbsolutePath, string csvAbsolutePath)
        {
            //Opens the scenarios/states definition
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            DataContractSerializer dcs = new DataContractSerializer(typeof(BindingList<BindingScenarios.BindableStaticScenario>), dcsSettings);
            FileStream fs = new FileStream(xmlAbsolutePath, FileMode.Open);
            XmlDictionaryReader reader =
            XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            var myScenarios = (BindingList<BindingScenarios.BindableStaticScenario>)dcs.ReadObject(reader);
            //Create the tep object
            var tep = new BindableStaticScenario(system);
            //Open the datatable data on states
            tep.CreateStateCollection();
            tep.MyStateCollectionDt.LoadDtFromCsv(csvAbsolutePath);
            tep.MyStateCollectionDt.CommitStateCollectionToPowerSystemState();

            return tep;
        }
        */
    }
}
