using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.CustomAttributes
{
    public class RequreSecureConnectionFilter : RequireHttpsAttribute
    {
        //public bool IsLocal
        //{
        //    get
        //    {
        //        String remoteAddress = UserHostAddress;

        //        // if unknown, assume not local
        //        if (String.IsNullOrEmpty(remoteAddress))
        //            return false;

        //        // check if localhost
        //        if (remoteAddress == "127.0.0.1" || remoteAddress == "::1")
        //            return true;

        //        // compare with local address
        //        if (remoteAddress == LocalAddress)
        //            return true;

        //        return false;
        //    }
        //}

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                // when connection to the application is local, don't do any HTTPS stuff
                return;
            }

            base.OnAuthorization(filterContext);
        }
    }
}