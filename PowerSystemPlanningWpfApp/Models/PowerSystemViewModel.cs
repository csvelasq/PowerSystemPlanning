using NLog;
using PowerSystemPlanning;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.Models
{
    public class PowerSystemViewModel
    {
        // TODO Pegar desde excel
        // TODO Copy to excel including headers
        // TODO Configuracion del programa
        // TODO Memoria de archivos abiertos recientemente
        // TODO Validacion de entradas en datagrid
        // TODO Logger to UI

        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private PowerSystem _PowerSystem;
        public BindingList<Node> nodes;
        public BindingList<GeneratingUnit> generatingUnits;
        public BindingList<InelasticLoad> inelasticLoads;
        public BindingList<TransmissionLine> transmissionLines;
        public string powerSystemName
        {
            get
            {
                return this._PowerSystem.Name;
            }
            set
            {
                this._PowerSystem.Name = value;
            }
        }

        public string full_fileName;
        public bool isSaved { get { return File.Exists(this.full_fileName); } }

        public DelegateCommand RunLDC { get; private set; }

        public PowerSystemViewModel()
        {
            this._PowerSystem = new PowerSystem("Unnamed Power System");
            this.bindToPowerSystem();
            this.RunLDC = new DelegateCommand(OnSubmit, CanSubmit);
        }

        private void OnSubmit()
        {
            ControlUtils.DatagridTest dgtest = new ControlUtils.DatagridTest();
            dgtest.Show();
        }
        private bool CanSubmit() { return true; }

        public PowerSystemViewModel(PowerSystem pws)
        {
            this._PowerSystem = pws;
            this.bindToPowerSystem();
        }

        /// <summary>
        /// Connects the BindingLists exposed in this class, to the PowerSystem backend, 
        /// and adds a custom event handler to create new objects for each list
        /// </summary>
        private void bindToPowerSystem()
        {
            this.nodes = this._PowerSystem.nodes;
            this.nodes.AddingNew += (sender, e) => { e.NewObject = new Node(this._PowerSystem.NumberOfNodes); };
            this.generatingUnits = this._PowerSystem.generatingUnits;
            this.generatingUnits.AddingNew += (sender, e) => { e.NewObject = new GeneratingUnit(this._PowerSystem); };
            this.inelasticLoads = this._PowerSystem.inelasticLoads;
            this.inelasticLoads.AddingNew += (sender, e) => { e.NewObject = new InelasticLoad(this._PowerSystem); };
            this.transmissionLines = this._PowerSystem.transmissionLines;
            this.transmissionLines.AddingNew += (sender, e) => { e.NewObject = new TransmissionLine(this._PowerSystem); };
        }

        public void saveModel(string fileName)
        {
            this.full_fileName = fileName;
            this.saveModel();
        }

        public void saveModel()
        {
            using (TextWriter myStream = new StreamWriter(this.full_fileName))
            {
                // Save document
                this._PowerSystem.saveToXMLFile(myStream);
            }
            logger.Info("Current power system (named '{0}') saved in {1}.", this.powerSystemName, this.full_fileName);
        }

        public void loadModel(string fileName)
        {
            using (StreamReader file = new System.IO.StreamReader(fileName))
            {
                this._PowerSystem = PowerSystem.readFromXMLFile(file);
                this.full_fileName = fileName;
            }
            this.bindToPowerSystem();
            logger.Info("Current power system (named '{0}') loaded from {1}.", this.powerSystemName, this.full_fileName);
        }
    }
}
