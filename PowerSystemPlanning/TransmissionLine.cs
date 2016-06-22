using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning
{
    public class TransmissionLine : TransmissionElement
    {
        private double _ThermalCapacityMW;

        /// <summary>
        /// Maximum thermal capacity of the transmission line, in MW.
        /// </summary>
        public double ThermalCapacityMW
        {
            get
            {
                return _ThermalCapacityMW;
            }

            set
            {
                _ThermalCapacityMW = value;
            }
        }

        private double _ReactanceOhm;

        /// <summary>
        /// Series reactance of the transmission line, in ohms.
        /// </summary>
        public double ReactanceOhm
        {
            get
            {
                return _ReactanceOhm;
            }

            set
            {
                _ReactanceOhm = value;
            }
        }

        /// <summary>
        /// Empty constructor, not meant to be used but rather included only to allow serialization.
        /// </summary>
        public TransmissionLine() : base() { }

        public TransmissionLine(PowerSystem power_system) : base(power_system)
        {
        }
    }
}
