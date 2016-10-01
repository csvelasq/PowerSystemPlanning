using PowerSystemPlanning.Models.SystemBaseData.Load;

namespace PowerSystemPlanning.Models.SystemState.Load
{
    /// <summary>
    /// The current state of an inelastic load.
    /// </summary>
    public interface IInelasticLoadState
    {
        /// <summary>
        /// The inelastic load whose state this object describes.
        /// </summary>
        IInelasticLoad UnderlyingInelasticLoad { get; }
        /// <summary>
        /// The current consumption of this load.
        /// </summary>
        double Consumption { get; set; }
    }
}