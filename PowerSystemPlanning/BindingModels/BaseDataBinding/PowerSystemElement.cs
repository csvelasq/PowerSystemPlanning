using Prism.Mvvm;
using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData;

namespace PowerSystemPlanning.BindingModels.BaseDataBinding
{
    [DataContract()]
    public abstract class PowerSystemElement : BindableBase, IPowerSystemElement
    {
        public IPowerSystem MyPowerSystem { get; set; }

        protected int _Id;

        /// <summary>
        /// ID of this element (unique within the given power system).
        /// </summary>
        /// <remarks>
        /// ID starts from 0 and increments until N. ID's must be unique. The ID must also indicate the position of the element in the containing list in the Power System object.
        /// </remarks>
        [DataMember()]
        public int Id
        {
            get { return _Id; }
            set { SetProperty<int>(ref _Id, value); }
        }

        private string _Name;

        /// <summary>
        /// Name of this element (arbitrary string set by the user).
        /// </summary>
        [DataMember()]
        public string Name
        {
            get { return _Name; }
            set { SetProperty<string>(ref _Name, value); }
        }

        public PowerSystemElement(IPowerSystem pws, int id, string name)
        {
            MyPowerSystem = pws;
            Id = id;
            Name = name;
        }

        public PowerSystemElement(IPowerSystem pws, int id)
            : this(pws, id, "")
        { }
    }
}
