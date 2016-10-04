using PowerSystemPlanningWpfApp.ApplicationWide;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PowerSystemPlanningWpfApp
{

    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// The event aggregator of this app to comunicate between viewmodels.
        /// </summary>
        protected readonly IEventAggregator _eventAggregator;

        public PowerSystemPlanningApplication MyPowerSystemPlanningApplication { get; private set; }
        public DockManagerViewModel MyDockManagerViewModel { get; private set; }

        public MainWindowViewModel()
        {
            _eventAggregator = ApplicationService.Instance.EventAggregator;
            /*
            //Add one sample power system to the docking view-model
            var powerSystem = new PowerSystemPlanning.BindingModels.BaseDataBinding.PowerSystem();
            powerSystem.Name = "New Power System";
            var documents = new List<BaseDocumentViewModel>();
            documents.Add(new ApplicationWide.PowerSystemSummary(powerSystem));
            this.MyDockManagerViewModel = new DockManagerViewModel(documents);*/
            this.MyDockManagerViewModel = new DockManagerViewModel();
            //Create app
            MyPowerSystemPlanningApplication = new PowerSystemPlanningApplication();
        }

        public void DockingManager_DocumentClosed(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            //Publish event
            _eventAggregator.GetEvent<ApplicationWide.Events.DocumentClosedEvent>().Publish((BaseDocumentViewModel)e.Document.Content);
        }
    }
}
