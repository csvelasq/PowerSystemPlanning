using PowerSystemPlanning.Models.SystemBaseData;

namespace PowerSystemPlanning.Models.SystemState
{
    public interface IPowerSystemElementState
    {
        IPowerSystemState MyPowerSystemState { get; }
        IPowerSystemElement MyPowerSystemElement { get; }
    }
}
