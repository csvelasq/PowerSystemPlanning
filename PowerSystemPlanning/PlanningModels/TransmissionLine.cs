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
        /// Serie reactance of the transmission line, in ohms.
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public double ReactanceOhm
        {
            get
            {
                return _ReactanceOhm;
            }

            set
            {
                _ReactanceOhm = value;
                _SusceptanceMho = 1 / value;
            }
        }

        private double _SusceptanceMho;

        /// <summary>
        /// Serie sucsceptance of the transmission line, in mhos.
        /// </summary>
        public double SusceptanceMho
        {
            get
            {
                return _SusceptanceMho;
            }

            set
            {
                _SusceptanceMho = value;
                _ReactanceOhm = 1 / value;
            }
        }

        /// <summary>
        /// Empty constructor, not meant to be used but rather included only to allow serialization.
        /// </summary>
        public TransmissionLine() : base() { }

        public TransmissionLine(PowerSystem power_system) : base(power_system)
        {
            this.Id = this._PowerSystem.NumberOfTransmissionLines;
        }
    }
}
