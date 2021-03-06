﻿using PowerSystemPlanning;
using PowerSystemPlanning.PlanningModels;
using PowerSystemPlanning.Solvers.OPF;
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

namespace PowerSystemPlanningWpfApp.Analysis.OPF
{
    /// <summary>
    /// Interaction logic for OPFRunControl.xaml
    /// </summary>
    public partial class OPFRunControl : UserControl
    {
        PowerSystem _MyPowerSystem;
        public PowerSystem MyPowerSystem {
            get
            {
                return _MyPowerSystem;
            }
            set
            {
                _MyPowerSystem = value;
                MyLoadBlock = new LoadBlock(1, 1);
                pnlRun.DataContext = MyLoadBlock;
            }
        }
        LoadBlock MyLoadBlock { get; set; }
        OPFModelSolverForLDC OPFSolverForLDC;

        OPFModelResultForLDC _OPFResultsForLDC;
        OPFModelResultForLDC OPFResultsForLDC
        {
            get
            {
                return _OPFResultsForLDC;
            }

            set
            {
                _OPFResultsForLDC = value;
                this.opfResultsControl.OPFResultsForLDC = value;
            }
        }

        public OPFRunControl()
        {
            InitializeComponent();
        }
        
        private void btnRunOpf_Click(object sender, RoutedEventArgs e)
        {
            //builds the model
            OPFSolverForLDC = new OPFModelSolverForLDC(this.MyPowerSystem, this.MyLoadBlock);
            //solves the model
            OPFSolverForLDC.Solve();
            //binds results
            OPFResultsForLDC = OPFSolverForLDC.MyOPFModelResultsForLDC;
        }
    }
}
