using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole)]
    public class AppDownloadController : Controller
    {
        // GET: AppDownload
        public ActionResult Index()
        {
            ViewBag.IsFileExists = "yes";
            return View();
        }

        public FileResult DownloadApp()
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"G:\PRITI\CMS APP\CMS APP PACKAGES\CMS APK\CMS APK 18 AUG.apk");
            string fileName = "CMS APK 18 AUG.apk";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}