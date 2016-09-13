using System.Collections.Generic;

namespace PowerSystemPlanning
{
    public interface IPowerSystem
    {
        IList<GeneratingUnit> GeneratingUnits { get; }
        IList<InelasticLoad> InelasticLoads { get; }
        double LoadSheddingCost { get; }
        string Name { get; set; }
        IList<Node> Nodes { get; }
        int NumberOfGeneratingUnits { get; }
        int NumberOfInelasticLoads { get; }
        int NumberOfNodes { get; }
        int NumberOfTransmissionLines { get; }
        double TotalMWInelasticLoads { get; }
        IList<TransmissionLine> TransmissionLines { get; }
    }
}