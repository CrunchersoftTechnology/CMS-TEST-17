using CMS.Common.GridModels;
using CMS.Web;
using MVCGrid.Interfaces;
using MVCGrid.Models;
using MVCGrid.Web;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CMS.Web.Models.Rendering
{
    public class CustomCsvRenderingEngine : IMVCGridRenderingEngine
    {
        public bool AllowsPaging
        {
            get { return false; }
        }

        public virtual string GetFilename()
        {
            return "export.csv";
        }

        public virtual void PrepareResponse(System.Web.HttpResponse httpResponse)
        {
            httpResponse.Clear();
            httpResponse.ContentType = "text/csv";
            httpResponse.AddHeader("content-disposition", "attachment; filename=\"" + GetFilename() + "\"");
            httpResponse.BufferOutput = false;
        }

        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }

        public void Render(RenderingModel model, GridContext gridContext, TextWriter outputStream)
        {
            if (MVCGridConfig.GridDictonary.ContainsKey(gridContext.GridName))
            {
                var n = MVCGridConfig.GridDictonary[gridContext.GridName];

                Type type = GetType(n);
                if (type != null)
                {
                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    string[] propertyNames = (from PropertyInfo property in properties
                                              where property.GetCustomAttributes(typeof(ExcludeAttribute), true).Length > 0
                                              select property.Name).ToArray();
                    foreach (var item in propertyNames)
                    {
                        var delete = model.Columns.FirstOrDefault(x => x.Name == item);
                        if (delete != null)
                        {
                            model.Columns.Remove(delete);
                        }
                    }
                }
            }
            var sw = outputStream;

            StringBuilder sbHeaderRow = new StringBuilder();
            foreach (var col in model.Columns)
            {
                if (sbHeaderRow.Length != 0)
                {
                    sbHeaderRow.Append(",");
                }
                sbHeaderRow.Append(CsvEncode(col.Name));
            }
            sbHeaderRow.AppendLine();
            sw.Write(sbHeaderRow.ToString());

            foreach (var item in model.Rows)
            {
                StringBuilder sbRow = new StringBuilder();
                foreach (var col in model.Columns)
                {
                    var cell = item.Cells[col.Name];

                    if (sbRow.Length != 0)
                    {
                        sbRow.Append(",");
                    }

                    string val = cell.PlainText;

                    sbRow.Append(CsvEncode(val));
                }
                sbRow.AppendLine();
                sw.Write(sbRow.ToString());
            }

        }

        private string CsvEncode(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
            {
                return "\"\"";
            }

            string esc = s.Replace("\"", "\"\"");

            return String.Format("\"{0}\"", esc);
        }


        public void RenderContainer(ContainerRenderingModel model, TextWriter outputStream)
        {
            throw new NotImplementedException("Csv Rendering Engine has no container");
        }
    }
}
