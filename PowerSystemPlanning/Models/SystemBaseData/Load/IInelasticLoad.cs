using PowerSystemPlanning.Models.SystemBaseData.Nodes;

namespace PowerSystemPlanning.Models.SystemBaseData.Load
{
    /// <summary>
    /// An inelastic load (with fixed consumption despite the price of power).
    /// </summary>
    public interface IInelasticLoad : INodeElement
    {
        /// <summary>
        /// Consumption (MW) of this inelastic load.
        /// </summary>
        double Consumption { get; set; }
    }
}