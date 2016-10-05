using System.Linq;
using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Branch;
using PowerSystemPlanning.Models.SystemBaseData.Nodes;

namespace PowerSystemPlanning.BindingModels.BaseDataBinding.Branch
{
    [DataContract()]
    public class BranchElement : PowerSystemElement, IBranchElement
    {
        protected INode _NodeFrom;
        /// <summary>
        /// The origin node to which this transmission element is connected.
        /// </summary>
        [DataMember()]
        public INode NodeFrom
        {
            get { return _NodeFrom; }
            set
            {
                SetProperty<INode>(ref _NodeFrom, value);
            }
        }

        /// <summary>
        /// Name of the node to which this element comes from, 
        /// used to change the node from within the GUI (not from the backend)
        /// </summary>
        public string NodeFromName
        {
            get { return NodeFrom.Name; }
            set
            {
                var newNode = MyPowerSystem.Nodes.First(x => x.Name == value);
                SetProperty<INode>(ref _NodeFrom, newNode);
            }
        }

        protected INode _NodeTo;
        /// <summary>
        /// The destination node to which this transmission element is connected.
        /// </summary>
        [DataMember()]
        public INode NodeTo
        {
            get { return _NodeTo; }
            set { SetProperty<INode>(ref _NodeTo, value); }
        }

        /// <summary>
        /// Name of the node to which this element goes to, 
        /// used to change the node from within the GUI (not from the backend)
        /// </summary>
        public string NodeToName
        {
            get { return NodeTo.Name; }
            set
            {
                var newNode = MyPowerSystem.Nodes.First(x => x.Name == value);
                SetProperty<INode>(ref _NodeTo, newNode);
            }
        }

        public BranchElement(PowerSystem pws, int id, string name)
            : base(pws, id, name)
        { }

        public BranchElement(PowerSystem pws, int id)
            : base(pws, id)
        { }
    }
}
