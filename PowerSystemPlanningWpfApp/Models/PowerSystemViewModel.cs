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
        // TODO Copy to excel including headers
        // TODO Datagrid validation
        // TODO Program Configuration

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

        #region DelegateCommands
        // TODO separate these delegates to another class?
        /// <summary>
        /// Shows the window for running the LDC OPF model.
        /// </summary>
        public DelegateCommand RunLDC { get; private set; }
        private void OnSubmitRunLDC()
        {
            ControlUtils.WindowDatagridTest dgtest = new ControlUtils.WindowDatagridTest();
            dgtest.Show();
        }
        private bool CanSubmitRunLDC() { return true; }
        /// <summary>
        /// Shows the about window of this application.
        /// </summary>
        public DelegateCommand ShowAboutWindow { get; private set; }
        private void OnSubmitShowAboutWindow()
        {
            Help.About about = new Help.About();
            about.Show();
        }
        private bool CanSubmitShowAboutWindow() { return true; }
        /// <summary>
        /// Shows the window to run an Optimal Power Flow.
        /// </summary>
        public DelegateCommand ShowOPFWindow { get; private set; }
        private void OnSubmitShowOPFWindow()
        {
            OPF.RunOPFWindow runOPFWindow = new OPF.RunOPFWindow();
            runOPFWindow.Show();
        }
        private bool CanSubmitShowOPFWindow() { return true; }
        #endregion

        public PowerSystemViewModel()
        {
            this._PowerSystem = new PowerSystem("Unnamed Power System");
            this.bindToPowerSystem();
            this.RunLDC = new DelegateCommand(OnSubmitRunLDC, CanSubmitRunLDC);
            this.ShowAboutWindow = new DelegateCommand(OnSubmitShowAboutWindow, CanSubmitShowAboutWindow);
            this.ShowOPFWindow = new DelegateCommand(OnSubmitShowOPFWindow, CanSubmitShowOPFWindow);
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

        /// <summary>
        /// Saves the power system model to the given location.
        /// </summary>
        /// <param name="fileName">The XML filename (path included) to which the model is saved.</param>
        public void saveModel(string fileName)
        {
            this.full_fileName = fileName;
            this.saveModel();
        }

        /// <summary>
        /// Saves the power system model to the previously set location (in 'this.full_fileName').
        /// The location is set when a call to saveModel(string) is made.
        /// This overload should not be called if fileName has not been properly set.
        /// </summary>
        public void saveModel()
        {
            using (TextWriter myStream = new StreamWriter(this.full_fileName))
            {
                // Save document
                this._PowerSystem.saveToXMLFile(myStream);
            }
            logger.Info("Current power system (named '{0}') saved in {1}.", this.powerSystemName, this.full_fileName);
        }

        /// <summary>
        /// Loads a power system model from the given XML file.
        /// </summary>
        /// <param name="fileName">The filename (path included) of the XML file with the serialized power system.</param>
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
