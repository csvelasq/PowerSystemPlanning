using PowerSystemPlanning.BindingModels;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PowerSystemPlanningWpfApp.ApplicationWide.AppModels
{
    public class TepScenarioStudy : SerializableBindableBase
    {
        public string GenericName => "TEP under Scenarios";

        string _InstanceName;
        public string InstanceName
        {
            get { return _InstanceName; }
            set { SetProperty<string>(ref _InstanceName, value); }
        }

        public double MyNum { get; set; } = 287;

        public TepScenarioStudy()
        {
            InstanceName = "tep1";
        }

        internal void SaveToXml(string myXmlAbsolutePath)
        {
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            DataContractSerializer dcs = new DataContractSerializer(typeof(TepScenarioStudy), dcsSettings);
            var xmlSettings = new XmlWriterSettings { Indent = true };
            using (var myXmlWriter = XmlWriter.Create(myXmlAbsolutePath, xmlSettings))
            {
                dcs.WriteObject(myXmlWriter, this);
            }
        }

        public static TepScenarioStudy OpenFromXML(string xmlPath)
        {
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            DataContractSerializer dcs = new DataContractSerializer(typeof(TepScenarioStudy), dcsSettings);
            FileStream fs = new FileStream(xmlPath, FileMode.Open);
            XmlDictionaryReader reader =
            XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

            //Deserialize the study
            var deserializedStudy = (TepScenarioStudy)dcs.ReadObject(reader);

            return deserializedStudy;
        }
    }
}
