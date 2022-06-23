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
    public class Session_ReportsController : Controller
    {
        private MeetingVLEntities db = new MeetingVLEntities();

        // GET: Session_Reports
        public ActionResult Index(int? project_id)
        {
            var sessionReports = db.SessionReports.Include(s => s.Project).Where(p => p.Project_ID == project_id);
            
            Project project = db.Projects.Find(project_id);
            TempData["project_id"] = project_id;
            TempData["project_Name"] = project.Name;
            TempData["project_Description"] = project.Description;
            Category category = db.Categories.Find(project.Category_ID);
            TempData["category_id"] = category.ID;
            TempData["category_Name"] = category.Name;
            return View(sessionReports.ToList());
        }

        // GET: Session_Reports/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SessionReport sessionReport = db.SessionReports.Find(id);
            if (sessionReport == null)
            {
                return HttpNotFound();
            }
            return View(sessionReport);
        }

        // GET: Session_Reports/Create
        public ActionResult Create()
        {
            ViewBag.Project_ID = new SelectList(db.Projects, "ID", "Name");
            return View();
        }

        // POST: Session_Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Project_ID,Name,Date_Start,Date_End,State")] SessionReport sessionReport)
        {
            if (ModelState.IsValid)
            {
                db.SessionReports.Add(sessionReport);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Project_ID = new SelectList(db.Projects, "ID", "Name", sessionReport.Project_ID);
            return View(sessionReport);
        }

        // GET: Session_Reports/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SessionReport sessionReport = db.SessionReports.Find(id);
            if (sessionReport == null)
            {
                return HttpNotFound();
            }
            ViewBag.Project_ID = new SelectList(db.Projects, "ID", "Name", sessionReport.Project_ID);
            return View(sessionReport);
        }

        // POST: Session_Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Project_ID,Name,Date_Start,Date_End,State")] SessionReport sessionReport)
        {
            if (ModelState.IsValid)
            {
                db.Entry(sessionReport).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Project_ID = new SelectList(db.Projects, "ID", "Name", sessionReport.Project_ID);
            return View(sessionReport);
        }

        // GET: Session_Reports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SessionReport sessionReport = db.SessionReports.Find(id);
            if (sessionReport == null)
            {
                return HttpNotFound();
            }
            return View(sessionReport);
        }

        // POST: Session_Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SessionReport sessionReport = db.SessionReports.Find(id);
            db.SessionReports.Remove(sessionReport);
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
