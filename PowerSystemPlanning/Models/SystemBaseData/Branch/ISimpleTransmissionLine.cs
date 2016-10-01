namespace PowerSystemPlanning.Models.SystemBaseData.Branch
{
    /// <summary>
    /// A simple transmission line within a power system.
    /// </summary>
    public interface ISimpleTransmissionLine : IBranchElement
    {
        /// <summary>
        /// The thermal capacity for transfer of active power (MW).
        /// </summary>
        double ThermalCapacity { get; set; }
        /// <summary>
        /// The reactance of this transmission line (Ohms).
        /// </summary>
        /// <remarks>
        /// The inverse of <see cref="SusceptanceMho"/>.
        /// </remarks>
        double ReactanceOhm { get; }
        /// <summary>
        /// The susceptance of this transmission line (Mhos).
        /// </summary>
        double SusceptanceMho { get; set; }
    }
}
