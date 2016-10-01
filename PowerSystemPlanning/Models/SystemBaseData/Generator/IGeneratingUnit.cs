using PowerSystemPlanning.Models.SystemBaseData.Nodes;

namespace PowerSystemPlanning.Models.SystemBaseData.Generator
{
    /// <summary>
    /// A generating unit within a power system (able to produce power).
    /// </summary>
    public interface IGeneratingUnit : INodeElement
    {
        /// <summary>
        /// The installed capacity (MW) of this generating unit.
        /// </summary>
        double InstalledCapacity { get; set; }
        /// <summary>
        /// The marginal cost (US$/MWh) of this generating unit.
        /// </summary>
        double MarginalCost { get; set; }
    }
}