using NLog;
using Ookii.Dialogs.Wpf;
using PowerSystemPlanning.BindingModels.BaseDataBinding;
using PowerSystemPlanningWpfApp.ApplicationWide.ViewModels;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide
{
    public class PowerSysViewModel : BaseDocumentOneXmlViewModel
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The event aggregator of this app to comunicate between viewmodels.
        /// </summary>
        protected readonly IEventAggregator _eventAggregator;

        PowerSystem _MyPowerSystem;
        public PowerSystem MyPowerSystem
        {
            get { return _MyPowerSystem; }
            private set
            {
                if (_MyPowerSystem != value)
                {
                    _MyPowerSystem = value;
                    OnPropertyChanged();
                }
            }
        }

        #region UI properties
        public override string Title => "Power System Editor";
        #endregion

        public PowerSysViewModel()
        {
            _eventAggregator = ApplicationService.Instance.EventAggregator;
        }

        public PowerSysViewModel(string xmlPath) : base(xmlPath)
        {
            MyPowerSystem = PowerSystem.OpenFromXml(XmlAbsolutePath);
        }

        public PowerSysViewModel(PowerSystem pws) : this()
        {
            MyPowerSystem = pws;
        }

        #region Open&Save
        public override string DefaultFileName => MyPowerSystem.Name;
        public override void SaveToXml()
        {
            MyPowerSystem.SaveToXml(XmlAbsolutePath);
            logger.Info($"Power System '{MyPowerSystem.Name}' saved to '{XmlAbsolutePath}'.");
        }
        #endregion
    }
}
