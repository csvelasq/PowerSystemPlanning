using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// Code from http://stackoverflow.com/questions/4118617/wpf-datagrid-pasting
namespace PowerSystemPlanningWpfApp.ControlUtils
{
    public class CustomDataGrid : DataGrid
    {
        public event ExecutedRoutedEventHandler ExecutePasteEvent;
        public event CanExecuteRoutedEventHandler CanExecutePasteEvent;

        // ******************************************************************
        static CustomDataGrid()
        {
            CommandManager.RegisterClassCommandBinding(
                typeof(CustomDataGrid),
                new CommandBinding(ApplicationCommands.Paste,
                    new ExecutedRoutedEventHandler(OnExecutedPasteInternal),
                    new CanExecuteRoutedEventHandler(OnCanExecutePasteInternal)));
        }

        // ******************************************************************
        #region Clipboard Paste

        // ******************************************************************
        private static void OnCanExecutePasteInternal(object target, CanExecuteRoutedEventArgs args)
        {
            ((CustomDataGrid)target).OnCanExecutePaste(target, args);
        }

        // ******************************************************************
        /// <summary>
        /// This virtual method is called when ApplicationCommands.Paste command query its state.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnCanExecutePaste(object target, CanExecuteRoutedEventArgs args)
        {
            if (CanExecutePasteEvent != null)
            {
                CanExecutePasteEvent(target, args);
                if (args.Handled)
                {
                    return;
                }
            }

            args.CanExecute = CurrentCell != null;
            args.Handled = true;
        }

        // ******************************************************************
        private static void OnExecutedPasteInternal(object target, ExecutedRoutedEventArgs args)
        {
            ((CustomDataGrid)target).OnExecutedPaste(target, args);
        }

        // ******************************************************************
        /// <summary>
        /// This virtual method is called when ApplicationCommands.Paste command is executed.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        protected virtual void OnExecutedPaste(object target, ExecutedRoutedEventArgs args)
        {
            if (ExecutePasteEvent != null)
            {
                ExecutePasteEvent(target, args);
                if (args.Handled)
                {
                    return;
                }
            }

            // parse the clipboard data            
            List<string[]> rowData = ClipboardHelper2.ParseClipboardData();

            // TODO Paste from excel only a subset of columns
            int displayIndexSelectedColumn = (target as CustomDataGrid).CurrentColumn.DisplayIndex;

            bool hasAddedNewRow = false;
            
            int minRowIndex = Items.IndexOf(CurrentItem);
            int maxRowIndex = Items.Count - 1;
            int minColumnDisplayIndex = (SelectionUnit != DataGridSelectionUnit.FullRow) ? Columns.IndexOf(CurrentColumn) : 0;
            int maxColumnDisplayIndex = Columns.Count - 1;
            int rowDataIndex = 0;
            for (int i = minRowIndex; i <= maxRowIndex && rowDataIndex < rowData.Count; i++, rowDataIndex++)
            {
                if (i < this.Items.Count)
                {
                    CurrentItem = Items[i];

                    BeginEditCommand.Execute(null, this);

                    int columnDataIndex = 0;
                    for (int j = minColumnDisplayIndex; j <= maxColumnDisplayIndex && columnDataIndex < rowData[rowDataIndex].Length; j++, columnDataIndex++)
                    {
                        DataGridColumn column = ColumnFromDisplayIndex(j);
                        column.OnPastingCellClipboardContent(Items[i], rowData[rowDataIndex][columnDataIndex]);
                    }

                    CommitEditCommand.Execute(this, this);
                    if (i == maxRowIndex)
                    {
                        maxRowIndex++;
                        hasAddedNewRow = true;
                    }
                }
            }

            // update selection
            if (hasAddedNewRow)
            {
                UnselectAll();
                UnselectAllCells();

                CurrentItem = Items[minRowIndex];

                if (SelectionUnit == DataGridSelectionUnit.FullRow)
                {
                    SelectedItem = Items[minRowIndex];
                }
                else if (SelectionUnit == DataGridSelectionUnit.CellOrRowHeader ||
                         SelectionUnit == DataGridSelectionUnit.Cell)
                {
                    SelectedCells.Add(new DataGridCellInfo(Items[minRowIndex], Columns[minColumnDisplayIndex]));

                }
            }
        }

        // ******************************************************************
        /// <summary>
        ///     Whether the end-user can add new rows to the ItemsSource.
        /// </summary>
        public bool CanUserPasteToNewRows
        {
            get { return (bool)GetValue(CanUserPasteToNewRowsProperty); }
            set { SetValue(CanUserPasteToNewRowsProperty, value); }
        }

        // ******************************************************************
        /// <summary>
        ///     DependencyProperty for CanUserAddRows.
        /// </summary>
        public static readonly DependencyProperty CanUserPasteToNewRowsProperty =
            DependencyProperty.Register("CanUserPasteToNewRows",
                                        typeof(bool), typeof(CustomDataGrid),
                                        new FrameworkPropertyMetadata(true, null, null));

        // ******************************************************************
        #endregion Clipboard Paste

        private void SetGridToSupportManyEditEitherWhenValidationErrorExists()
        {
            this.Items.CurrentChanged += Items_CurrentChanged;


            //Type DatagridType = this.GetType().BaseType;
            //PropertyInfo HasCellValidationProperty = DatagridType.GetProperty("HasCellValidationError", BindingFlags.NonPublic | BindingFlags.Instance);
            //HasCellValidationProperty.
        }

        void Items_CurrentChanged(object sender, EventArgs e)
        {
            //this.Items[0].
            //throw new NotImplementedException();
        }

        // ******************************************************************
        private void SetGridWritable()
        {
            Type DatagridType = this.GetType().BaseType;
            PropertyInfo HasCellValidationProperty = DatagridType.GetProperty("HasCellValidationError", BindingFlags.NonPublic | BindingFlags.Instance);
            if (HasCellValidationProperty != null)
            {
                HasCellValidationProperty.SetValue(this, false, null);
            }
        }

        // ******************************************************************
        public void SetGridWritableEx()
        {
            BindingFlags bindingFlags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Instance;
            PropertyInfo cellErrorInfo = this.GetType().BaseType.GetProperty("HasCellValidationError", bindingFlags);
            PropertyInfo rowErrorInfo = this.GetType().BaseType.GetProperty("HasRowValidationError", bindingFlags);
            cellErrorInfo.SetValue(this, false, null);
            rowErrorInfo.SetValue(this, false, null);
        }

        // ******************************************************************
    }
}
