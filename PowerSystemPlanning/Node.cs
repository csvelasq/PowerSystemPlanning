using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning
{
    /// <summary>
    /// A node in a given power system.
    /// A node is an element to which generators and loads are connected.
    /// Transmission lines are connected between two nodes.
    /// </summary>
    public class Node
    {
        private int id;

        /// <summary>
        /// The unique ID of this node within the power system.
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
        /// Name of the node (arbitrarily assigned by the user).
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
        /// Generating units connected to this node.
        /// </summary>
        public List<GeneratingUnit> generatingUnits;

        /// <summary>
        /// Inelastic loads connected to this node.
        /// </summary>
        public List<InelasticLoad> inelasticLoads;

        public Node()
        {

        }

        public Node(int id)
        {
            this.id = id;
            this.name = "Node " + id;
        }
    }
}
