using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace CMS.Web.Controllers
{
    public class NotifyController : Controller
    {
        // GET: Notify
        public ActionResult Index()
        {
            string todaydate = DateTime.Now.ToString("dd/MM/yyyy");

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSWebConnection"].ToString());

            string aSql = "select PaymentLists from Students";
            SqlCommand sqlCommand = new SqlCommand(aSql, con);


            con.Open();
            using (SqlDataReader read = sqlCommand.ExecuteReader())
            {
                while (read.Read())
                {
                    string date2 = read["date"].ToString();
                    string price = read["Payment"].ToString();

                    if (todaydate == date2 )//12months i must writte something here)
                    {
                        //int price2 = Convert.ToInt32(price) - ((20 / 100) * Convert.ToInt32(price));
                        return View();
                    }
                }
              
                read.Close();

            }
            con.Close();
            return View();
           
        }

        // GET: Notify/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Notify/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notify/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Notify/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Notify/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Notify/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Notify/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
