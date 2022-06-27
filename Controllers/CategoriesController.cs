using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingVL.Models;

namespace MeetingVL.Controllers
{
    public class CategoriesController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: Categories
        public ActionResult Index(string keyword)
        {
           
            var links = from l in db.Categories
                        select l;

            if (!string.IsNullOrEmpty(keyword))
            {
                links = links.Where(b => b.Name.ToLower().Contains(keyword.ToLower()));
                TempData["keyword"] = keyword;
                return View(links.ToList());
            }

            return View(links.ToList());
        }
       
        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create(string Name)
        {
            Category category = new Category();
            category.Name = Name;
            db.Categories.Add(category);
            db.SaveChanges();
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
            category.Name = Name;
            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
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
