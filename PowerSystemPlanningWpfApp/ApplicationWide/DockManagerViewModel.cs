using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    public class DockManagerViewModel : BindableBase
    {
        /// <summary>
        /// The event aggregator of this app to comunicate between viewmodels.
        /// </summary>
        protected readonly IEventAggregator _eventAggregator;

        public ObservableCollection<BaseDocumentViewModel> MyOpenDocuments { get; private set; }

        BaseDocumentViewModel _ActiveDocumentViewModel;
        public BaseDocumentViewModel ActiveDocumentViewModel
        {
            get { return _ActiveDocumentViewModel; }
            set { SetProperty<BaseDocumentViewModel>(ref _ActiveDocumentViewModel, value); }
        }

        #region Commands
        #endregion

        public DockManagerViewModel()
        {
            _eventAggregator = ApplicationService.Instance.EventAggregator;
            //this._eventAggregator.GetEvent<Events.PowerSystemCreatedEvent>()
            //    .Subscribe((item) => { this.MyOpenDocuments.Add(item); });
            this._eventAggregator.GetEvent<Events.RequestPowerSystemEditionEvent>()
                .Subscribe(OnRequestPowerSystemEditionEvent);
            this._eventAggregator.GetEvent<Events.DocumentClosedEvent>()
                .Subscribe(OnDocumentClosedEvent);
            MyOpenDocuments = new ObservableCollection<BaseDocumentViewModel>();
        }

        private void OnDocumentClosedEvent(BaseDocumentViewModel obj)
        {
            MyOpenDocuments.Remove(obj);
        }

        private void OnRequestPowerSystemEditionEvent(PowerSystemSummary obj)
        {
            //Focus existing edition document
            if (MyOpenDocuments.Contains(obj))
            {
                ActiveDocumentViewModel = obj;
            }
            //New edition document
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
    }
}
