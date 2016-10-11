namespace PowerSystemPlanning.Models.SystemBaseData
{
    /// <summary>
    /// An element within a power system (e.g. node, load, transmission line).
    /// </summary>
    public interface IPowerSystemElement : IHaveIdAndName, IHavePowerSystem
    {
    }
}