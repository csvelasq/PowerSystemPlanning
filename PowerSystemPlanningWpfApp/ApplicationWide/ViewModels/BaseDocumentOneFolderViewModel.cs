using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide.ViewModels
{
    public abstract class BaseDocumentOneFolderViewModel : BaseDocumentViewModel
    {
        public string FolderAbsolutePath { get; set; }

        public bool FolderExists => Directory.Exists(FolderAbsolutePath);

        public override void Save()
        {
            if (FolderExists)
            {
                SaveToFolder();
            }
            else
            {
                SaveAs();
            }
        }

        public override void SaveAs()
        {
            var dlg = new VistaFolderBrowserDialog();
            bool? result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                FolderAbsolutePath = dlg.SelectedPath;
                SaveToFolder();
            }
        }

        public abstract void SaveToFolder();

        protected BaseDocumentOneFolderViewModel() { }

        protected BaseDocumentOneFolderViewModel(string folderPath) : this()
        {
            FolderAbsolutePath = folderPath;
        }
    }
}
