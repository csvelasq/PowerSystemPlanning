using System.Runtime.Serialization;
using PowerSystemPlanning.BindingModels.BaseDataBinding.Nodes;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Load;

namespace PowerSystemPlanning.BindingModels.BaseDataBinding.Load
{
    /// <summary>
    /// An inelastic load connected to a given power system.
    /// </summary>
    /// <remarks>
    /// An inelastic load has fixed consumption (in MW) regardless of the price of electricity.
    /// A high cost is incurred if the load is to be shed.
    /// </remarks>
    [DataContract()]
    public class InelasticLoad : NodeElement, IInelasticLoad
    {
        [DataMember()]
        protected double _Consumption;
        /// <summary>
        /// Consumption of the load (in MW).
        /// </summary>
        public double Consumption
        {
            get
            {
                return _Consumption;
            }

            set
            {
                SetProperty<double>(ref _Consumption, value);
            }
        }

        /// <summary>
        /// Creates a new inelastic load within the provided power system with an automatically assigned ID.
        /// </summary>
        /// <param name="pws">The power system to which this load belongs.</param>
        public InelasticLoad(PowerSystem pws)
            : base(pws, pws.InelasticLoads.Count)
        { }
    }
}
