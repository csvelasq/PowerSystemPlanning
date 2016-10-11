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

namespace PowerSystemPlanningWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowViewModel MyMainWindowViewModel
        {
            get { return (MainWindowViewModel)this.DataContext; }
        }

        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            //dockingManager.DocumentClosed += MyMainWindowViewModel.DockingManager_DocumentClosed;
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
            root.DockHeight = new GridLength(200);
        }

        private void OnOpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyMainWindowViewModel.OpenFileCommand.Execute();
        }

        private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MyMainWindowViewModel.SaveFileCommand.Execute();
        }
    }
}
