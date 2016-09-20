using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PowerSystemPlanningWpfApp.ControlUtils
{
    // Enables double-click in datagrids: code from https://social.msdn.microsoft.com/Forums/vstudio/en-US/632ea875-a5b8-4d47-85b3-b30f28e0b827/mousedouble-click-or-singleclick-on-datagrid-in-mvvm-how-can-i-do-that?forum=wpf
    public static class MyCommands
    {
        public static readonly DependencyProperty DataGridDoubleClickProperty =
          DependencyProperty.RegisterAttached("DataGridDoubleClickCommand", typeof(ICommand), typeof(MyCommands),
                            new PropertyMetadata(new PropertyChangedCallback(AttachOrRemoveDataGridDoubleClickEvent)));

        public static ICommand GetDataGridDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DataGridDoubleClickProperty);
        }

        public static void SetDataGridDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DataGridDoubleClickProperty, value);
        }

        public static void AttachOrRemoveDataGridDoubleClickEvent(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            DataGrid dataGrid = obj as DataGrid;
            if (dataGrid != null)
            {
                ICommand cmd = (ICommand)args.NewValue;

                if (args.OldValue == null && args.NewValue != null)
                {
                    dataGrid.MouseDoubleClick += ExecuteDataGridDoubleClick;
                }
                else if (args.OldValue != null && args.NewValue == null)
                {
                    dataGrid.MouseDoubleClick -= ExecuteDataGridDoubleClick;
                }
            }
        }

        private static void ExecuteDataGridDoubleClick(object sender, MouseButtonEventArgs args)
        {
            DependencyObject obj = sender as DependencyObject;
            ICommand cmd = (ICommand)obj.GetValue(DataGridDoubleClickProperty);
            if (cmd != null)
            {
                if (cmd.CanExecute(obj))
                {
                    cmd.Execute(obj);
                }
            }
        }

    }
}
