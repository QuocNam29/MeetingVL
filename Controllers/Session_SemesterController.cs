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
    public class Session_SemesterController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: Session_Semester
        public ActionResult Index(int semester_id)
        {
            var session_Semester = db.Session_Semester.Include(s => s.Semester).Include(s => s.SessionReport).Where(s => s.Semester_ID == semester_id);

            Semester semester = db.Semesters.Find(semester_id);
                int project_id = semester.Project.ID;
                TempData["project_id"] = project_id;
                TempData["semester_id"] = semester_id;
           

            return View(session_Semester.ToList());
        }

        // GET: Session_Semester/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session_Semester session_Semester = db.Session_Semester.Find(id);
            if (session_Semester == null)
            {
                return HttpNotFound();
            }
            return View(session_Semester);
        }

        // GET: Session_Semester/Create
        public ActionResult Create()
        {
            ViewBag.Semester_ID = new SelectList(db.Semesters, "ID", "User_ID");
            ViewBag.SessionReport_ID = new SelectList(db.SessionReports, "ID", "Name");
            return View();
        }

        // POST: Session_Semester/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SessionReport_ID,Semester_ID")] Session_Semester session_Semester)
        {
            if (ModelState.IsValid)
            {
                db.Session_Semester.Add(session_Semester);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Semester_ID = new SelectList(db.Semesters, "ID", "User_ID", session_Semester.Semester_ID);
            ViewBag.SessionReport_ID = new SelectList(db.SessionReports, "ID", "Name", session_Semester.SessionReport_ID);
            return View(session_Semester);
        }

        // GET: Session_Semester/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session_Semester session_Semester = db.Session_Semester.Find(id);
            if (session_Semester == null)
            {
                return HttpNotFound();
            }
            ViewBag.Semester_ID = new SelectList(db.Semesters, "ID", "User_ID", session_Semester.Semester_ID);
            ViewBag.SessionReport_ID = new SelectList(db.SessionReports, "ID", "Name", session_Semester.SessionReport_ID);
            return View(session_Semester);
        }

        // POST: Session_Semester/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SessionReport_ID,Semester_ID")] Session_Semester session_Semester)
        {
            if (ModelState.IsValid)
            {
                db.Entry(session_Semester).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Semester_ID = new SelectList(db.Semesters, "ID", "User_ID", session_Semester.Semester_ID);
            ViewBag.SessionReport_ID = new SelectList(db.SessionReports, "ID", "Name", session_Semester.SessionReport_ID);
            return View(session_Semester);
        }

        // GET: Session_Semester/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Session_Semester session_Semester = db.Session_Semester.Find(id);
            if (session_Semester == null)
            {
                return HttpNotFound();
            }
            return View(session_Semester);
        }

        // POST: Session_Semester/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Session_Semester session_Semester = db.Session_Semester.Find(id);
            db.Session_Semester.Remove(session_Semester);
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
