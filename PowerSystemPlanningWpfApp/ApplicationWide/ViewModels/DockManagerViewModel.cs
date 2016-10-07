using PowerSystemPlanningWpfApp.ApplicationWide.AppModels;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    public class DockManagerViewModel : BindableBase
    {
        /// <summary>
        /// The event aggregator of this app to comunicate between viewmodels.
        /// </summary>
        protected readonly IEventAggregator _eventAggregator;

        public ObservableCollection<BaseDocumentViewModel> MyOpenDocuments { get; private set; }

        PowerSysViewModel _OpenedPowerSystemViewModel;
        public PowerSysViewModel OpenedPowerSystemViewModel
        {
            get { return _OpenedPowerSystemViewModel; }
            set { SetProperty<PowerSysViewModel>(ref _OpenedPowerSystemViewModel, value); }
        }

        BaseDocumentViewModel _ActiveDocumentViewModel;
        public BaseDocumentViewModel ActiveDocumentViewModel
        {
            get { return _ActiveDocumentViewModel; }
            set { SetProperty<BaseDocumentViewModel>(ref _ActiveDocumentViewModel, value); }
        }

        public DockManagerViewModel()
        {
            _eventAggregator = ApplicationService.Instance.EventAggregator;
            this._eventAggregator.GetEvent<Events.PowerSystemOpenedEvent>()
                .Subscribe(OnPowerSystemOpenedEvent);
            this._eventAggregator.GetEvent<Events.RequestDocumentOpenEvent>()
                .Subscribe(OnRequestDocumentOpen);
            this._eventAggregator.GetEvent<Events.DocumentClosedEvent>()
                .Subscribe(OnDocumentClosedEvent);
            MyOpenDocuments = new ObservableCollection<BaseDocumentViewModel>();
        }

        private void OnRequestDocumentOpen(BaseDocumentViewModel obj)
        {
            if (MyOpenDocuments.Contains(obj))
            {
                ActiveDocumentViewModel = obj;
            }
            else
            {
                MyOpenDocuments.Add(obj);
            }
        }

        public DockManagerViewModel(IEnumerable<BaseDocumentViewModel> documents)
            : this()
        {
            foreach (var doc in documents)
            {
                MyOpenDocuments.Add(doc);
            }
        }

        private void OnDocumentClosedEvent(BaseDocumentViewModel obj)
        {
            MyOpenDocuments.Remove(obj);
        }

        private void OnPowerSystemOpenedEvent(PowerSysViewModel obj)
        {
            MyOpenDocuments.Clear();
            OpenedPowerSystemViewModel = obj;
            MyOpenDocuments.Add(obj);
        }
    }
}
