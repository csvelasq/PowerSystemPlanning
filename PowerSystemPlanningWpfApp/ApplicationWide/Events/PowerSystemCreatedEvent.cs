using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace PowerSystemPlanningWpfApp.ApplicationWide.Events
{
    public class PowerSystemCreatedEvent : PubSubEvent<PowerSystemSummary> { }
    public class RequestPowerSystemEditionEvent : PubSubEvent<PowerSystemSummary> { }
    public class DocumentClosedEvent : PubSubEvent<BaseDocumentViewModel> { }
    //public class WindowLoadedEvent : PubSubEvent<object> { }
}
