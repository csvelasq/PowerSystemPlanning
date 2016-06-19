using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning
{
    /// <summary>
    /// Represents any element that is connected to a single node.
    /// </summary>
    public abstract class NodeElement
    {
        private int id;

        /// <summary>
        /// ID of this element (unique within the given power system).
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
        /// Name of this element (arbitrary string set by the user).
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

        private int connectionNodeId;

        /// <summary>
        /// ID of the node to which this element is connected
        /// </summary>
        public int ConnectionNodeId
        {
            get
            {
                return connectionNodeId;
            }

            set
            {
                connectionNodeId = value;
            }
        }

        /// <summary>
        /// The node to which this element is connected.
        /// </summary>
        public Node connectionNode;
    }
}
