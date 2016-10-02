using PowerSystemPlanning.Models.SystemBaseData.Branch;

namespace PowerSystemPlanning.Models.SystemState.Branch
{
    /// <summary>
    /// A particular state of a transmission line.
    /// </summary>
    public interface ISimpleTransmissionLineState : IElementWithAvailability
    {
        /// <summary>
        /// The transmission line whose state this object describes.
        /// </summary>
        ISimpleTransmissionLine UnderlyingTransmissionLine { get; }

        double AvailableThermalCapacity { get; }
    }
}
