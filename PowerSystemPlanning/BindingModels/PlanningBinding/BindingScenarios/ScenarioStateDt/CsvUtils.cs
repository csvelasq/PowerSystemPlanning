using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerSystemPlanning.BindingModels.PlanningBinding.BindingTepScenarios
{
    /// <summary>
    /// Extension method to export datatables to csv
    /// </summary>
    public static class CsvUtils
    {
        /// <summary>
        /// Writes a DataTable to a CSV file.
        /// </summary>
        /// <param name="dt">The datatable to export.</param>
        /// <param name="strAbsoluteFilePath">The absolute path to the file to which the contents of the datatable will be written.</param>
        /// <remarks>
        /// Code from http://www.c-sharpcorner.com/uploadfile/deveshomar/export-datatable-to-csv-using-extension-method/
        /// </remarks>
        public static void SaveToCsv(this DataTable dt, string strAbsoluteFilePath)
        {
            StreamWriter sw = new StreamWriter(strAbsoluteFilePath, false);
            //headers  
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sw.Write(dt.Columns[i]);
                if (i < dt.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < dt.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }
    }
}
