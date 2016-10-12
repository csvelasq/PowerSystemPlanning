using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanningWpfApp.ApplicationWide.ViewModels
{
    public abstract class BaseDocumentOneXmlViewModel : BaseDocumentViewModel
    {
        public string XmlAbsolutePath { get; set; }

        public bool XmlFileExists => File.Exists(XmlAbsolutePath);

        public virtual string DefaultFileName { get; }

        public override void Save()
        {
            if (XmlFileExists)
            {
                SaveToXml();
            }
            else
            {
                SaveAs();
            }
        }

        public override void SaveAs()
        {
            var dlg = new VistaSaveFileDialog();
            dlg.FileName = DefaultFileName;
            dlg.DefaultExt = ".xml";
            dlg.Filter = "XML file (.xml)|*.xml";
            bool? result = dlg.ShowDialog();
            // Process save file dialog box results
            if (result == true)
            {
                XmlAbsolutePath = dlg.FileName;
                SaveToXml();
            }
        }

        public abstract void SaveToXml();

        protected BaseDocumentOneXmlViewModel() { }

        protected BaseDocumentOneXmlViewModel(string xmlPath) : this()
        {
            XmlAbsolutePath = xmlPath;
        }
    }
}
