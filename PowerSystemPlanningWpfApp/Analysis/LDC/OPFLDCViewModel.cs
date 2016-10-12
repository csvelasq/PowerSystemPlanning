using PowerSystemPlanning.Models.Planning.LDC;
using PowerSystemPlanning.Models.Planning.ScenarioTEP;
using PowerSystemPlanning.Solvers.LDCOPF;
using PowerSystemPlanning.Solvers.LDCOPF.LdcOpfResults;
using PowerSystemPlanning.Solvers.OPF.OpfResults;
using PowerSystemPlanningWpfApp.Analysis.OPF;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerSystemPlanningWpfApp.Analysis.LDC
{
    /// <summary>
    /// View-model for analizing results of the LDC OPF model (these results are already constructed, not built in this view-model).
    /// </summary>
    public class OPFLDCViewModel : BindableBase
    {
        #region internal fields
        LdcOpfModelResults _MyLDCOPFModelResults;
        private OpfModelResult currentlySelectedLoadBlockResults;
        #endregion

        /// <summary>
        /// LDC OPF results displayed
        /// </summary>
        public LdcOpfModelResults MyLDCOPFModelResults
        {
            get { return _MyLDCOPFModelResults; }
            set
            {
                SetProperty<LdcOpfModelResults>(ref this._MyLDCOPFModelResults, value);
            }
        }

        public OpfModelResult CurrentlySelectedLoadBlockResults
        {
            get { return currentlySelectedLoadBlockResults; }
            set { SetProperty<OpfModelResult>(ref currentlySelectedLoadBlockResults, value); }
        }

        public OPFLDCViewModel()
        {
            DgLDC_DoubleClick = new DelegateCommand(ViewDetailedOpfResults,
                delegate () { return CurrentlySelectedLoadBlockResults != null; });
        }

        #region Commands
        /// <summary>
        /// Handles a double click in the datagrid with summarized results for each LDC block (shows detailed OPF results window).
        /// </summary>
        public ICommand DgLDC_DoubleClick { get; private set; }
        private void ViewDetailedOpfResults()
        {
            /*
            OPFResultsWindow opfDetailsWindow = new OPFResultsWindow();
            opfDetailsWindow.DataContext = CurrentlySelectedLoadBlockResults;
            opfDetailsWindow.Show();*/
        }
        #endregion
    }
}
