using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PowerSystemPlanning
{
    /// <summary>
    /// Represents the technical and economic model of a Power System for the objectives of medium to long term planning.
    /// </summary>
    public class PowerSystem
    {
        private string name;

        /// <summary>
        /// Name of the power system.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Nodes of the power system.
        /// </summary>
        public List<Node> nodes;

        /// <summary>
        /// Number of nodes in the power system.
        /// </summary>
        public int NumberOfNodes
        {
            get
            {
                return nodes.Count;
            }
        }

        /// <summary>
        /// All Generating units in the power system.
        /// </summary>
        public GeneratingUnit[] generatingUnits;
        private int numberOfGeneratingUnits;

        public int NumberOfGeneratingUnits
        {
            get
            {
                return numberOfGeneratingUnits;
            }
        }

        /// <summary>
        /// All Inelastic loads in the power system.
        /// </summary>
        public InelasticLoad[] inelasticLoads;
        private int numberOfInelasticLoads;

        public int NumberOfInelasticLoads
        {
            get
            {
                return numberOfInelasticLoads;
            }
        }

        /// <summary>
        /// Total MW of inelastic loads.
        /// </summary>
        public double TotalMWInelasticLoads
        {
            get
            {
                return (from load in this.inelasticLoads select load.ConsumptionMW).Sum();
            }
        }

        /// <summary>
        /// All Transmission lines in the power sytem.
        /// </summary>
        public TransmissionLine[] transmissionLines;
        private int numberOfTransmissionLines;

        public int NumberOfTransmissionLines
        {
            get
            {
                return numberOfTransmissionLines;
            }
        }

        public PowerSystem()
        {
        }

        public PowerSystem(string name)
        {
            this.Name = name;
        }

        public PowerSystem(string name, List<GeneratingUnit> generatingUnits, List<InelasticLoad> inelasticLoads,
            List<TransmissionLine> transmissionLines)
            : this(name)
        {
            this.generatingUnits = generatingUnits.ToArray<GeneratingUnit>();
            this.inelasticLoads = inelasticLoads.ToArray<InelasticLoad>();
            this.transmissionLines = transmissionLines.ToArray<TransmissionLine>();
        }

        /// <summary>
        /// Saves the current power sysem to an XML file, provided its path.
        /// IOException and other exceptions are not managed.
        /// </summary>
        /// <param name="full_file_path">The full file path where the current power system will be saved</param>
        public void saveToXMLFile(string full_file_path)
        {
            System.IO.FileStream file = System.IO.File.Create(full_file_path); //overwrites the file if it existed
            this.saveToXMLFile(file);
            file.Close();
        }

        /// <summary>
        /// Saves the current power sysem to an XML file, provided the stream.
        /// The provided stream must is not closed within this method.
        /// IOException and other exceptions are not managed.
        /// </summary>
        /// <param name="saveStream"></param>
        public void saveToXMLFile(Stream saveStream)
        {
            System.Xml.Serialization.XmlSerializer writer = new System.Xml.Serialization.XmlSerializer(typeof(PowerSystem));
            writer.Serialize(saveStream, this);
        }

        /// <summary>
        /// Creates a Power System by reading an XML file.
        /// IOException and other exceptions are not managed.
        /// </summary>
        /// <param name="full_file_path">Full file path of the XML file where the power system was stored.</param>
        /// <returns>A new power system object with the contents serialized in the XML file.</returns>
        public static PowerSystem readFromXMLFile(string full_file_path)
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PowerSystem));
            System.IO.StreamReader file = new System.IO.StreamReader(full_file_path);
            PowerSystem retval = (PowerSystem)reader.Deserialize(file);
            file.Close();
            return retval;
        }

        /// <summary>
        /// Creates a Power System by reading an XML file, provided the stream.
        /// The provided stream must is not closed within this method.
        /// IOException and other exceptions are not managed.
        /// </summary>
        /// <param name="xmlStream">Full file path of the XML file where the power system was stored.</param>
        /// <returns>A new power system object with the contents serialized in the XML file.</returns>
        public static PowerSystem readFromXMLFile(Stream xmlStream)
        {
            System.Xml.Serialization.XmlSerializer reader = new System.Xml.Serialization.XmlSerializer(typeof(PowerSystem));
            PowerSystem retval = (PowerSystem)reader.Deserialize(xmlStream);
            return retval;
        }
    }
}
