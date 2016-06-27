using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning
{
    /// <summary>
    /// Represents a transmission element within a power system, connected to two nodes.
    /// </summary>
    public class TransmissionElement
    {
        protected PowerSystem _PowerSystem;

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
                if (this.NodeFrom!=null)
                {
                    if (this.NodeFrom.Id != value)
                        this.NodeFrom = _PowerSystem.nodes.SingleOrDefault(x => x.Id == value);
                }
                else this.NodeFrom = _PowerSystem.nodes.SingleOrDefault(x => x.Id == value);
            }
        }

        /// <summary>
        /// Name of the node from which this transmission element begins.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public string NodeFromName
        {
            get
            {
                if (this.NodeFrom != null)
                    return NodeFrom.Name;
                else return "";
            }
            set
            {
                if (this.NodeFrom != null)
                {
                    if (this.NodeFrom.Name != value) this.NodeFrom = _PowerSystem.nodes.SingleOrDefault(x => x.Name == value);
                }
                else this.NodeFrom = _PowerSystem.nodes.SingleOrDefault(x => x.Name == value);
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
                        this.NodeTo = _PowerSystem.nodes.SingleOrDefault(x => x.Id == value);
                }
                else this.NodeTo = _PowerSystem.nodes.SingleOrDefault(x => x.Id == value);
            }
        }

        /// <summary>
        /// Name of the node from which this transmission element begins.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public string NodeToName
        {
            get
            {
                if (this.NodeTo != null)
                    return NodeTo.Name;
                else return "";
            }
            set
            {
                if (this.NodeTo != null)
                {
                    if (this.NodeTo.Name != value) this.NodeTo = _PowerSystem.nodes.SingleOrDefault(x => x.Name == value);
                }
                else this.NodeTo = _PowerSystem.nodes.SingleOrDefault(x => x.Name == value);
            }
        }

        private int _Id;

        /// <summary>
        /// Unique identifier of this transmission element within a given power system.
        /// </summary>
        public int Id
        {
            get
            {
                return _Id;
            }

            set
            {
                _Id = value;
            }
        }

        private string _Name;

        /// <summary>
        /// Name of the transmission element (arbitrarily set by the user).
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
            }
        }

        /// <summary>
        /// Empty constructor, not meant to be used but rather included only to allow serialization.
        /// </summary>
        public TransmissionElement() { }

        public TransmissionElement(PowerSystem power_system)
        {
            this._PowerSystem = power_system;
        }
    }
}
