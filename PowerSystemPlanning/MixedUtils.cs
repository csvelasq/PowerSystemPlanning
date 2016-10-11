using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios;

namespace PowerSystemPlanning
{
    public static class MixedUtils
    {
        public static T LoadFromXml<T>(string xmlPath)
        {
            //Opens the scenarios/states definition
            var dcsSettings = new DataContractSerializerSettings { PreserveObjectReferences = true };
            var dcs = new DataContractSerializer(typeof(BindingTepModel), dcsSettings);
            var fs = new FileStream(xmlPath, FileMode.Open);
            var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
            var deserializedObject = (T)dcs.ReadObject(reader);
            return deserializedObject;
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
    }
}
