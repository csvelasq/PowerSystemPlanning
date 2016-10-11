using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.Models.Planning.ExpansionState.Tep
{
    public interface ICandidateTransmissionLineState : IInvestmentElementState
    {
        /// <summary>
        /// The underlying candidate transmission line.
        /// </summary>
        /// <remarks>
        /// Implementations should return <see cref="UnderlyingCandidateTransmissionLine"/> in the getter of the underlying investment element in <see cref="IInvestmentElementState"/>.
        /// </remarks>
        ICandidateTransmissionLineState UnderlyingCandidateTransmissionLine { get; }
    }
}
