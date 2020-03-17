using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    public class GetFilesController : BaseController
    {
        public ActionResult Get(string pdftype)
        {
            var folderPath = string.Format(@"{0}/{1}", ConfigurationManager.AppSettings["studentAppPDF"], pdftype);
            string dirPath = Server.MapPath(folderPath);
            List<GetFilesList> files = new List<GetFilesList>();
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            foreach (FileInfo fInfo in dirInfo.GetFiles())
            {
                files.Add(new GetFilesList
                {
                    Name = fInfo.ToString()
                });
            }
            return Json(files, JsonRequestBehavior.AllowGet);
        }
    }

    public class GetFilesList
    {
        public string Name { get; set; }
    }
}