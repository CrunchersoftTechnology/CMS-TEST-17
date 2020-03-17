using System.Web;

namespace CMS.Common
{
    public class ImageValidation
    {
        public bool Validate(HttpPostedFileBase image)
        {
            //var imageTypes = new string[]{
            //        "image/jpg",
            //        "image/jpeg",
            //        "image/png"
            //    };
            //if (image.FileName == null || image.ContentLength == 0)
            //{
            //    ModelState.AddModelError("ImageUpload", "This field is required");
            //}
            //else if (!imageTypes.Contains(image.ContentType))
            //{
            //    ModelState.AddModelError("ImageUpload", "Please choose either a GIF, JPG or PNG image.");
            //}
            return true;
        }
    }
}
