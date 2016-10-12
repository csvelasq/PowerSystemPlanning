using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using NLog;
using PowerSystemPlanningWpfApp.ApplicationWide.ViewModels;
using Prism.Events;
using PowerSystemPlanningWpfApp.ApplicationWide.Events;

namespace PowerSystemPlanningWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// The event aggregator of this app to comunicate between viewmodels.
        /// </summary>
        protected readonly IEventAggregator _eventAggregator;

        public MainWindowViewModel MyMainWindowViewModel
        {
            get { return (MainWindowViewModel)this.DataContext; }
        }

        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            _eventAggregator = ApplicationService.Instance.EventAggregator;
            _eventAggregator.GetEvent<PowerSystemOpenedEvent>().Subscribe(OnPowerSystemOpened);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Configures the richtextbox target for showing log
            Dispatcher.Invoke(() =>
            {
                var target = new Helper.WpfRichTextBoxTarget
                {
                    Name = "RichText",
                    Layout =
                        "[${longdate:useUTC=false}] :: [${level:uppercase=true}] :: ${logger}:${callsite} :: ${message} ${exception:innerFormat=tostring:maxInnerExceptionLevel=10:separator=,:format=tostring}",
                    ControlName = LogRichTextBox.Name,
                    FormName = GetType().Name,
                    AutoScroll = true,
                    MaxLines = 100000,
                    UseDefaultRowColoringRules = true,
                };
                var asyncWrapper = new NLog.Targets.Wrappers.AsyncTargetWrapper { Name = "RichTextAsync", WrappedTarget = target };

                LogManager.Configuration.AddTarget(asyncWrapper.Name, asyncWrapper);
                LogManager.Configuration.LoggingRules.Insert(0, new NLog.Config.LoggingRule("*", NLog.LogLevel.Debug, asyncWrapper));
                LogManager.ReconfigExistingLoggers();

            });
        }

        private void DockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            bottomAnchor.ToggleAutoHide();
            var root = (Xceed.Wpf.AvalonDock.Layout.LayoutAnchorablePane)bottomAnchor.Parent;
            root.DockHeight = new GridLength(150);
        }

        private void OnNewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            // Configure the message box to be displayed
            string caption = "Power system planning";
            string messageBoxText = "Do you want to save changes to this document before creating a new model? Click 'Yes' to save and close, 'No' to close without saving, or 'Cancel' to not close.";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage icon = MessageBoxImage.Warning;// Display message box
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
            // Process message box results
            switch (result)
            {
                case MessageBoxResult.Yes:
                    // User pressed Yes button: save and then create new
                    MyMainWindowViewModel.SaveFileCommand.Execute();
                    MyMainWindowViewModel.NewFileCommand.Execute();
                    break;
                case MessageBoxResult.No:
                    // User pressed No button: create new immediately
                    MyMainWindowViewModel.NewFileCommand.Execute();
                    break;
                case MessageBoxResult.Cancel:
                    // User pressed Cancel button: don't do anythin
                    break;
            }
        }

        private void OnOpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyMainWindowViewModel.OpenFileCommand.Execute();
        }

        private void OnPowerSystemOpened(PowerSysViewModel obj)
        {
            RecentFileList.InsertFile(obj.XmlAbsolutePath);
        }

        private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyMainWindowViewModel.SaveFileCommand.Execute();
        }

        private void OnSaveAsExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyMainWindowViewModel.SaveFileAsCommand.Execute();
        }

        private void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyMainWindowViewModel.ExitCommand.Execute();
        }
    }
}
