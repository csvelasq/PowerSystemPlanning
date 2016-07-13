using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.PlanningModels
{
    /// <summary>
    /// Decorator of a Power System, allowing flexible and nested modifications to the basic model.
    /// </summary>
    public class PowerSystemDecorator : IPowerSystem
    {
        /// <summary>
        /// The underlying power system model, which will be (indirectly) modified by this decorator.
        /// </summary>
        /// <remarks>
        /// The basic power system model set up by the end-user. This object should not be directly modified, but rather modifications should be introduced by extending this class and altering the public properties of the <see cref="IPowerSystem"/> interface. Multiple decorators can be nested in order to introduce multiple modifications to the underlying model (e.g. disabling a transmission line, decreasing the maximum output of a generator, increasing the consumption of an inelastic load, and so forth).
        /// </remarks>
        protected IPowerSystem powerSystem;
        
        public PowerSystemDecorator(IPowerSystem powerSystem)
        {
            this.powerSystem = powerSystem;
        }

        public IList<GeneratingUnit> GeneratingUnits
        {
            get
            {
                return this.powerSystem.GeneratingUnits;
            }
        }

        public IList<InelasticLoad> InelasticLoads
        {
            get
            {
                return this.powerSystem.InelasticLoads;
            }
        }

        public string Name
        {
            get
            {
                return this.powerSystem.Name;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public IList<Node> Nodes
        {
            get
            {
                return this.powerSystem.Nodes;
            }
        }

        public int NumberOfGeneratingUnits
        {
            get
            {
                return this.powerSystem.NumberOfGeneratingUnits;
            }
        }

        public int NumberOfInelasticLoads
        {
            get
            {
                return this.powerSystem.NumberOfInelasticLoads;
            }
        }

        public int NumberOfNodes
        {
            get
            {
                return this.powerSystem.NumberOfNodes;
            }
        }

        public int NumberOfTransmissionLines
        {
            get
            {
                return this.powerSystem.NumberOfTransmissionLines;
            }
        }

        public double TotalMWInelasticLoads
        {
            get
            {
                return this.powerSystem.TotalMWInelasticLoads;
            }
        }

        public IList<TransmissionLine> TransmissionLines
        {
            get
            {
                return this.powerSystem.TransmissionLines;
            }
        }
    }
}
