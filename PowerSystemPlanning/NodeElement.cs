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
        protected PowerSystem _PowerSystem;

        protected int _Id;

        /// <summary>
        /// ID of this element (unique within the given power system).
        /// </summary>
        /// <remarks>
        /// ID starts from 0 and increments until N. ID's must be unique. The ID must also indicate the position of the element in the containing list in the Power System object.
        /// </remarks>
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
        /// Name of this element (arbitrary string set by the user).
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
        /// ID of the node to which this element is connected
        /// </summary>
        public int ConnectionNodeId
        {
            get
            {
                if (this.ConnectionNode != null)
                    return ConnectionNode.Id;
                else return -1;
            }
            set
            {
                if (this.ConnectionNode!=null)
                {
                    if (this.ConnectionNode.Id != value) this.ConnectionNode = _PowerSystem._Nodes.SingleOrDefault(x=> x.Id==value);
                }
                else this.ConnectionNode = _PowerSystem._Nodes.SingleOrDefault(x => x.Id == value);
            }
        }

        /// <summary>
        /// Name of the node to which this element is connected
        /// </summary>
        public string ConnectionNodeName
        {
            get
            {
                if (this.ConnectionNode != null)
                    return ConnectionNode.Name;
                else return "";
            }
            set
            {
                if (this.ConnectionNode != null)
                {
                    if (this.ConnectionNode.Name != value) this.ConnectionNode = _PowerSystem._Nodes.SingleOrDefault(x => x.Name == value);
                }
                else this.ConnectionNode = _PowerSystem._Nodes.SingleOrDefault(x => x.Name == value);
            }
        }

        /// <summary>
        /// The node to which this element is connected.
        /// </summary>
        public Node ConnectionNode;

        /// <summary>
        /// Empty constructor, not meant to be used but rather included only to allow serialization.
        /// </summary>
        public NodeElement() { }

        public NodeElement(PowerSystem power_system)
        {
            this._PowerSystem = power_system;
        }
    }
}
