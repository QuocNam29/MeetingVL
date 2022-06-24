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
    public class ProjectsController : Controller
    {
        private MeetingVLEntities db = new MeetingVLEntities();

        // GET: Projects
        public ActionResult Index(int category_id)
        {
            var projects = db.Projects.Include(p => p.Category).Where(p => p.Category_ID == category_id);
            Category category = db.Categories.Find(category_id);
            TempData["category_id"] = category_id;
            TempData["category_Name"] = category.Name;
            
            return View(projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create(int category_id,
                string name, string tilte_Cycles, 
                int cycles, string time, string description,
                DateTime date_Start, DateTime date_End)
                
        {
            Project project = new Project();
            project.Category_ID = category_id;
            project.Name = name;
            project.Date_Start = date_Start;
            project.Date_End = date_End;
            db.Projects.Add(project);
 

            TimeSpan Time = date_End - date_Start;
            int SumTime = Time.Days + 1;
            if (time == "Day")
            {
                int CyclesperTime = SumTime / cycles;
                for (int i = 1; i <= CyclesperTime; i++)
                {
                    
                    SessionReport sessionReport = new SessionReport();
                    sessionReport.Project_ID = project.ID;
                    sessionReport.Name = tilte_Cycles + " "+ i;
                    sessionReport.Date_Start = date_Start;
                    sessionReport.Date_End = date_Start.AddDays(cycles - 1);
                    db.SessionReports.Add(sessionReport);

                    date_Start = date_Start.AddDays(cycles);
                }
            }
            else if (time == "Week")
            {
                int CyclesperTime = SumTime / (7 * cycles);
                for (int i = 1; i <= CyclesperTime; i++)
                {

                    SessionReport sessionReport = new SessionReport();
                    sessionReport.Project_ID = project.ID;
                    sessionReport.Name = tilte_Cycles + " " + i;
                    sessionReport.Date_Start = date_Start;
                    sessionReport.Date_End = date_Start.AddDays((7 * cycles) - 1);
                    db.SessionReports.Add(sessionReport);

                    date_Start = date_Start.AddDays((7 * cycles));
                }
            }
            else if (time == "Month")
            {
                int CyclesperTime = SumTime / (28 * cycles);
                for (int i = 1; i <= CyclesperTime; i++)
                {

                    SessionReport sessionReport = new SessionReport();
                    sessionReport.Project_ID = project.ID;
                    sessionReport.Name = tilte_Cycles + " " + i;
                    sessionReport.Date_Start = date_Start;
                    sessionReport.Date_End = date_Start.AddMonths(cycles).AddDays(-1);
                    db.SessionReports.Add(sessionReport);

                    date_Start = date_Start.AddMonths(cycles);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index", new {category_id = category_id });
            
        }


        
        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ViewBag.Category_ID = new SelectList(db.Categories, "ID", "Name", project.Category_ID);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Category_ID,Name,Description,Date_Start,Date_End,State")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category_ID = new SelectList(db.Categories, "ID", "Name", project.Category_ID);
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id, int? category_id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);      
            project.State = "Deleted";
            db.Entry(project).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { category_id = category_id });
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
