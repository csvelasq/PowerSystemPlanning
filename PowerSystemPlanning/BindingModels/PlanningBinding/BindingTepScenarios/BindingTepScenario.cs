using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingScenarios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios
{
    public class BindingTepScenarios : SerializableBindableBase
    {
        public PowerSystem MyPowerSystem { get; protected set; }

        BindingList<BindingScenario> _MyScenarios;
        public BindingList<BindingScenario> MyScenarios
        {
            get { return _MyScenarios; }
            set { SetProperty<BindingList<BindingScenario>>(ref _MyScenarios, value); }
        }

        StateCollectionDataTable _MyStateCollectionDt;
        public StateCollectionDataTable MyStateCollectionDt
        {
            get { return _MyStateCollectionDt; }
            set { SetProperty<StateCollectionDataTable>(ref _MyStateCollectionDt, value); }
        }

        public BindingTepScenarios() { }

        public BindingTepScenarios(PowerSystem system) : this()
        {
            MyPowerSystem = system;
            MyScenarios = new BindingList<BindingScenario>();
        }

        public void CreateStateCollection()
        {
            MyStateCollectionDt = new StateCollectionDataTable(MyPowerSystem, MyScenarios);
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
        public void Save(string xmlAbsolutePath, string csvAbsolutePath)
        {
            //Save the scenarios/states definition
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            DataContractSerializer dcs = new DataContractSerializer(typeof(BindingList<BindingScenario>), dcsSettings);
            var xmlSettings = new XmlWriterSettings { Indent = true };
            using (var myXmlWriter = XmlWriter.Create(xmlAbsolutePath, xmlSettings))
            {
                dcs.WriteObject(myXmlWriter, MyScenarios);
            }
            //Saves the datatable data on states
            MyStateCollectionDt.SaveDtToCsv(csvAbsolutePath);
        }

        public static BindingTepScenarios Load(PowerSystem system, string xmlAbsolutePath, string csvAbsolutePath)
        {
            //Opens the scenarios/states definition
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            DataContractSerializer dcs = new DataContractSerializer(typeof(BindingList<BindingScenario>), dcsSettings);
            FileStream fs = new FileStream(xmlAbsolutePath, FileMode.Open);
            XmlDictionaryReader reader =
            XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            var myScenarios = (BindingList<BindingScenario>)dcs.ReadObject(reader);
            //Create the tep object
            var tep = new BindingTepScenarios(system);
            //Open the datatable data on states
            tep.CreateStateCollection();
            tep.MyStateCollectionDt.LoadDtFromCsv(csvAbsolutePath);
            tep.MyStateCollectionDt.CommitStateCollectionToPowerSystemState();

            return tep;
        }
    }
}
