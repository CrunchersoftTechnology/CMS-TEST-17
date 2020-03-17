using CMS.Common;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers

{
    [Authorize]
    public class ClientAccountController : Controller
    {
        public ActionResult Index()
        {
            using (OurDbContext db = new OurDbContext())
            {

                return View(db.userAccount.ToList());
            }


        }

        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserAccount account)
        {
            if (ModelState.IsValid)
            {
                using (OurDbContext db = new OurDbContext())
                {
                    db.userAccount.Add(account);
                    db.SaveChanges();
                }
                ModelState.Clear();
               
                ViewBag.Message = account.name + " " + "Successfully Registered";
            }
            return View();
        }

        // Login 
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserAccount user)
        {
            using (OurDbContext db = new OurDbContext())
            {
                var usr = db.userAccount.Single(u => u.name == user.name && u.password == user.password);
                if (usr != null)
                {
                    Session["Id"] = usr.Id.ToString();
                    Session["Name"] = usr.name.ToString();
                    //return RedirectToAction("Login", "Account");
                    // return RedirectToAction("LoggedIn");
                    return RedirectToAction("Index", "Admin");

                }
                else
                {
                    ModelState.AddModelError("", "Name or Password is wrong");
                }
            }
            return View();
        }

        public ActionResult LoggedIn()
        {

            if (Session["Id"] != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}
