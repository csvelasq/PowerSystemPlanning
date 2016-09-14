using System.Collections.Generic;

namespace PowerSystemPlanning
{
    public interface IPowerSystem
    {
        IList<Node> Nodes { get; }
        IList<GeneratingUnit> GeneratingUnits { get; }
        IList<InelasticLoad> InelasticLoads { get; }
        double LoadSheddingCost { get; }
        IList<TransmissionLine> TransmissionLines { get; }
    }
}