using PowerSystemPlanning;
using PowerSystemPlanning.PlanningModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PowerSystemPlanningWpfApp.Model
{
    /// <summary>
    /// Interaction logic for PlanningModelEditorControl.xaml
    /// </summary>
    public partial class PlanningModelEditorControl : UserControl
    {
        PowerSystem _MyPowerSystem;
        public PowerSystem MyPowerSystem
        {
            get
            {
                return _MyPowerSystem;
            }

            set
            {
                _MyPowerSystem = value;
                DataContext = _MyPowerSystem;
            }
        }

        LDCPowerSystemPlanningModel _MyLDCPowerSystemPlanningModel;
        public LDCPowerSystemPlanningModel MyLDCPowerSystemPlanningModel
        {
            get
            {
                return _MyLDCPowerSystemPlanningModel;
            }

            set
            {
                _MyLDCPowerSystemPlanningModel = value;
                DataContext = _MyLDCPowerSystemPlanningModel;
                ldcEditor.DataContext = _MyLDCPowerSystemPlanningModel.MyLoadDurationCurve.DurationBlocks;
                candidateTLsEditor.DataContext = _MyLDCPowerSystemPlanningModel._MyCandidateTransmissionLines;
            }
        }

        public PlanningModelEditorControl()
        {
            InitializeComponent();
            MyLDCPowerSystemPlanningModel = new LDCPowerSystemPlanningModel();
        }
    }
}
