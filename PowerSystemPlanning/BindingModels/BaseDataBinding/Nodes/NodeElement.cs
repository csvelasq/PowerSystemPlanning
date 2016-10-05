using System.Linq;
using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Nodes;

namespace PowerSystemPlanning.BindingModels.BaseDataBinding.Nodes
{
    /// <summary>
    /// Represents any element that is connected to a single node.
    /// </summary>
    [DataContract()]
    public abstract class NodeElement : PowerSystemElement, INodeElement
    {
        /// <summary>
        /// Name of the node to which this element is connected, 
        /// used to change connection node from within the GUI (not from the backend)
        /// </summary>
        public string ConnectionNodeName
        {
            get { return ConnectionNode?.Name; }
            set
            {
                var newNode = MyBindingPowerSystem.BindingNodes.First(x => x.Name == value);
                MyConnectionNode = newNode;
            }
        }

        [DataMember()]
        protected Node _ConnectionNode;
        public Node MyConnectionNode
        {
            get { return _ConnectionNode; }
            set { SetProperty<Node>(ref _ConnectionNode, value); }
        }
        /// <summary>
        /// The node to which this element is connected.
        /// </summary>
        public INode ConnectionNode => MyConnectionNode;

        public NodeElement(PowerSystem pws, int id, string name)
            : base(pws, id, name)
        { }

        public NodeElement(PowerSystem pws, int id)
            : base(pws, id)
        { }
    }
}
