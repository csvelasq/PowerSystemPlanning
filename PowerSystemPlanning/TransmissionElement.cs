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
        /// <summary>
        /// The origin node to which this transmission element is connected.
        /// </summary>
        public Node nodeFrom;

        /// <summary>
        /// The destination node to which this transmission element is connected.
        /// </summary>
        public Node nodeTo;

        private int id;

        /// <summary>
        /// Unique identifier of this transmission element within a given power system.
        /// </summary>
        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        private string name;

        /// <summary>
        /// Name of the transmission element (arbitrarily set by the user).
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

        private int nodeFromID;

        /// <summary>
        /// ID of the node from which this transmission element begins.
        /// </summary>
        public int NodeFromID
        {
            get
            {
                return nodeFromID;
            }

            set
            {
                nodeFromID = value;
            }
        }

        private int nodeToID;

        /// <summary>
        /// ID of the node to which this transmission element arrives.
        /// </summary>
        public int NodeToID
        {
            get
            {
                return nodeToID;
            }

            set
            {
                nodeToID = value;
            }
        }
    }
}
