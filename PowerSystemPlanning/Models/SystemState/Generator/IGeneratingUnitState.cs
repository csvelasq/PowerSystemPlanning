using PowerSystemPlanning.Models.SystemBaseData.Generator;

namespace PowerSystemPlanning.Models.SystemState.Generator
{
    /// <summary>
    /// A particular state of a generating unit.
    /// </summary>
    public interface IGeneratingUnitState : IElementWithAvailability
    {
        /// <summary>
        /// The generating unit whose state this object describes.
        /// </summary>
        IGeneratingUnit UnderlyingGeneratingUnit { get; }
        /// <summary>
        /// The available capacity (MW) of the generating unit in this state.
        /// </summary>
        double AvailableCapacity { get; set; }
        /// <summary>
        /// The marginal cost (US$/MWh) of the generating unit in this state.
        /// </summary>
        double MarginalCost { get; set; }
    }
}
