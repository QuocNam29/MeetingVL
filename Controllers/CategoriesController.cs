using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingVL.Middleware;
using MeetingVL.Models;

namespace MeetingVL.Controllers
{
    [LoginVerification]

    public class CategoriesController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        public ActionResult After_Login()
        {
            /*Session["ID_User"] = "chau.lth@vlu.edu.vn";*/
            string ID_User = Session["ID_User"].ToString();
            User user = db.Users.Find(ID_User);
            Session["Role"] = user.Role;
            Session["Avt"] = user.Avt;
            return View();
        }
        // GET: Categories
        public ActionResult Index(string keyword)
        {

            string ID_User = Session["ID_User"].ToString();
            User user = db.Users.Find(ID_User);
            Session["Role"] = user.Role;
            Session["Avt"] = user.Avt;
            var links = from l in db.Categories.Where(c => c.ID_User == ID_User && c.State != "Deleted")
                        select l;
           
           

            if (!string.IsNullOrEmpty(keyword))
            {
                links = links.Where(b => b.Name.ToLower().Contains(keyword.ToLower().Trim()));
                TempData["keyword"] = keyword;
                return View(links.ToList());
            }

            return View(links.ToList());
        }
       
       
       
        // GET: Categories/Create
        public ActionResult Create(string Name)
        {
            Category category = new Category();
            category.Name = Name.Trim();
            category.ID_User = Session["ID_User"].ToString();
            db.Categories.Add(category);
            db.SaveChanges();
            Session["notification"] = "Successfully added new Category";
            return RedirectToAction("Index");
        }

       
        // GET: Categories/Edit/5
        public ActionResult Edit(int? id, string Name)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            category.Name = Name.Trim();
            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
            Session["notification"] = "Successfully Edited Category";
            return RedirectToAction("Index");
        }

        

        // GET: Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            category.State = "Deleted";
            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
            Session["notification"] = "Successfully Deleted Category";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
