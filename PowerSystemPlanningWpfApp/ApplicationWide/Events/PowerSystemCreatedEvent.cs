using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using PowerSystemPlanningWpfApp.ApplicationWide.ViewModels;

namespace PowerSystemPlanningWpfApp.ApplicationWide.Events
{
    public class PowerSystemOpenedEvent : PubSubEvent<PowerSysViewModel> { }
    public class RequestDocumentOpenEvent : PubSubEvent<BaseDocumentViewModel> { }
    public class DocumentClosedEvent : PubSubEvent<BaseDocumentViewModel> { }
}
