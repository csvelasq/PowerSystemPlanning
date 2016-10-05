using System.Runtime.Serialization;
using PowerSystemPlanning.Models.SystemBaseData;
using PowerSystemPlanning.Models.SystemBaseData.Branch;

namespace PowerSystemPlanning.BindingModels.BaseDataBinding.Branch
{
    [DataContract()]
    public class SimpleTransmissionLine : BranchElement, ISimpleTransmissionLine
    {
        protected double _ThermalCapacity;
        /// <summary>
        /// Maximum thermal capacity of the transmission line, in MW.
        /// </summary>
        [DataMember()]
        public double ThermalCapacity
        {
            get { return _ThermalCapacity; }
            set { SetProperty<double>(ref _ThermalCapacity, value); }
        }

        /// <summary>
        /// Serie reactance of the transmission line, in ohms.
        /// </summary>
        public double ReactanceOhm { get { return 1 / SusceptanceMho; } }

        private double _SusceptanceMho = 1;
        /// <summary>
        /// Serie sucsceptance of the transmission line, in mhos.
        /// </summary>
        [DataMember()]
        public double SusceptanceMho
        {
            get
            {
                return _SusceptanceMho;
            }

            set
            {
                SetProperty<double>(ref _SusceptanceMho, value);
            }
        }

        /// <summary>
        /// Creates a new transmission line within the provided power system with an automatically assigned ID.
        /// </summary>
        /// <param name="pws">The power system to which this transmission line belongs.</param>
        public SimpleTransmissionLine(PowerSystem pws)
            : base(pws, pws.TransmissionLines.Count)
        { }
    }
}
