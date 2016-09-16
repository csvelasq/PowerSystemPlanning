using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning
{
    public interface ITransmissionLine
    {
        //TODO only require a Node object, not the id and name as well
        int Id { get; set; }
        string Name { get; set; }
        double ThermalCapacityMW { get; set; }
        double ReactanceOhm { get; }
        double SusceptanceMho { get; set; }
        int NodeFromID { get; set; }
        int NodeToID { get; set; }
    }

    public class TransmissionLine : ITransmissionLine
    {
        protected PowerSystem MyPowerSystem;
        protected virtual IList<Node> Nodes
        {
            get { return MyPowerSystem.Nodes; }
        }

        /// <summary>
        /// Unique identifier of this transmission element within a given power system.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the transmission element (arbitrarily set by the user).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The origin node to which this transmission element is connected.
        /// </summary>
        public Node NodeFrom;

        /// <summary>
        /// ID of the node from which this transmission element begins.
        /// </summary>
        /// <remarks>
        /// ID starts from 0 and increments until N. ID's must be unique. The ID must also indicate the position of the element in the containing list in the Power System object.
        /// </remarks
        public int NodeFromID
        {
            get
            {
                if (this.NodeFrom != null)
                    return this.NodeFrom.Id;
                else return -1;
            }
            set
            {
                if (this.NodeFrom != null)
                {
                    if (this.NodeFrom.Id != value)
                        this.NodeFrom = Nodes.SingleOrDefault(x => x.Id == value);
                }
                else this.NodeFrom = Nodes.SingleOrDefault(x => x.Id == value);
            }
        }
        
        /// <summary>
        /// The destination node to which this transmission element is connected.
        /// </summary>
        public Node NodeTo;

        /// <summary>
        /// ID of the node to which this transmission element arrives.
        /// </summary>
        public int NodeToID
        {
            get
            {
                if (this.NodeTo != null)
                    return this.NodeTo.Id;
                else
                    return -1;
            }
            set
            {
                if (this.NodeTo != null)
                {
                    if (this.NodeTo.Id != value)
                        this.NodeTo = Nodes.SingleOrDefault(x => x.Id == value);
                }
                else this.NodeTo = Nodes.SingleOrDefault(x => x.Id == value);
            }
        }
        
        /// <summary>
        /// Maximum thermal capacity of the transmission line, in MW.
        /// </summary>
        public double ThermalCapacityMW { get; set; }

        /// <summary>
        /// Serie reactance of the transmission line, in ohms.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public double ReactanceOhm { get; set; }

        private double _SusceptanceMho;
        /// <summary>
        /// Serie sucsceptance of the transmission line, in mhos.
        /// </summary>
        public double SusceptanceMho
        {
            get
            {
                return _SusceptanceMho;
            }

            set
            {
                _SusceptanceMho = value;
                ReactanceOhm = 1 / value;
            }
        }

        /// <summary>
        /// Empty constructor, not meant to be used but rather included only to allow serialization.
        /// </summary>
        public TransmissionLine() { }

        public TransmissionLine(PowerSystem power_system)
        {
            this.MyPowerSystem = power_system;
            this.Id = this.MyPowerSystem.TransmissionLines.Count;
        }
    }
}
