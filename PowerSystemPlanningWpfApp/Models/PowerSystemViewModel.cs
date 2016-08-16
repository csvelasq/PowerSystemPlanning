using NLog;
using PowerSystemPlanning;
using PowerSystemPlanningWpfApp.LDC;
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
    public class PowerSystemViewModel : INotifyPropertyChanged
    {
        // TODO Remove this middleman, interact directly with backend
        // TODO Copy to excel including headers
        // TODO Datagrid validation
        // TODO Program Configuration

        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

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
                if (this._PowerSystem.Name != value)
                {
                    this._PowerSystem.Name = value;
                    NotifyPropertyChanged("powerSystemName");
                }
            }
        }

        public double LoadSheddingCost
        {
            get
            {
                return _PowerSystem.LoadSheddingCost;
            }
            set
            {
                if (_PowerSystem.LoadSheddingCost != value)
                {
                    _PowerSystem.LoadSheddingCost = value;
                    NotifyPropertyChanged("LoadSheddingCost");
                }
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
            OptimizeOPFLDC optOPFLDC = new OptimizeOPFLDC();
            optOPFLDC.Show();
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
            OPF.RunOPFWindow runOPFWindow = new OPF.RunOPFWindow(this._PowerSystem);
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
        /// Connects the BindingLists exposed in this class, to the PowerSystem backend
        /// </summary>
        /// <remarks>
        /// Adds a custom event handler to create new objects for each list, so as to automatically manage IDs.</remarks>
        private void bindToPowerSystem()
        {
            this.nodes = this._PowerSystem._Nodes;
            this.nodes.AddingNew += (sender, e) => { e.NewObject = new Node(this._PowerSystem); };
            this.generatingUnits = this._PowerSystem._GeneratingUnits;
            this.generatingUnits.AddingNew += (sender, e) => { e.NewObject = new GeneratingUnit(this._PowerSystem); };
            this.inelasticLoads = this._PowerSystem._InelasticLoads;
            this.inelasticLoads.AddingNew += (sender, e) => { e.NewObject = new InelasticLoad(this._PowerSystem); };
            this.transmissionLines = this._PowerSystem._TransmissionLines;
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
        /// </summary>
        /// <remarks>
        /// The location is set when a call to saveModel(string) is made.
        /// This overload should not be called if fileName has not been properly set.</remarks>
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
