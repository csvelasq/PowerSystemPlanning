using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning
{
    public class TransmissionLine : TransmissionElement
    {
        private double thermalCapacityMW;

        /// <summary>
        /// Maximum thermal capacity of the transmission line, in MW.
        /// </summary>
        public double ThermalCapacityMW
        {
            get
            {
                return thermalCapacityMW;
            }

            set
            {
                thermalCapacityMW = value;
            }
        }

        /// <summary>
        /// Series reactance of the transmission line, in ohms.
        /// </summary>
        public double ReactanceOhm
        {
            get
            {
                return reactanceOhm;
            }

            set
            {
                reactanceOhm = value;
            }
        }

        private double reactanceOhm;

        public TransmissionLine()
        {

        }
    }
}
