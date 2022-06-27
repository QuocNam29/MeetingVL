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
    public class Session_ReportsController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: Session_Reports
        public ActionResult Index(int project_id, string keyword)
        {
            var sessionReports = db.SessionReports.Include(s => s.Project).Where(p => p.Project_ID == project_id);

            var links = from l in db.SessionReports.Include(s => s.Project)
                        .Where(p => p.Project_ID == project_id).Where(s => s.State != "Deleted")
                        select l;

            if (!string.IsNullOrEmpty(keyword))
            {
                links = links.Where(b => b.Name.ToLower().Contains(keyword.ToLower()));
                TempData["keyword"] = keyword;

                Project project1 = db.Projects.Find(project_id);
                TempData["project_id"] = project_id;
                TempData["project_Name"] = project1.Name;
                TempData["project_Description"] = project1.Description;
                Category category1 = db.Categories.Find(project1.Category_ID);
                TempData["category_id"] = category1.ID;
                TempData["category_Name"] = category1.Name;
                return View(links.ToList());
            }

            Project project = db.Projects.Find(project_id);
            TempData["project_id"] = project_id;
            TempData["project_Name"] = project.Name;
            TempData["project_Description"] = project.Description;
            Category category = db.Categories.Find(project.Category_ID);
            TempData["category_id"] = category.ID;
            TempData["category_Name"] = category.Name;
            return View(links.ToList());
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
        public ActionResult Create(string tilte_Cycles, int project_id,
            DateTime date_Start, DateTime date_End,
            int cycles, string time, int option)
        {
           
            if (option == 1)
            {
                SessionReport sessionReport = new SessionReport();
                sessionReport.Project_ID = project_id;
                sessionReport.Name = tilte_Cycles;
                sessionReport.Date_Start = date_Start;
                sessionReport.Date_End = date_End;
                db.SessionReports.Add(sessionReport);
                db.SaveChanges();
                return RedirectToAction("Index", new { project_id = project_id });
            }

            else if (option == 2)
            {                           
                TimeSpan Time = date_End - date_Start;
                int SumTime = Time.Days + 1;
                if (time == "Day")
                {
                    int CyclesperTime = SumTime / cycles;
                    for (int i = 1; i <= CyclesperTime; i++)
                    {

                        SessionReport sessionReport = new SessionReport();
                        sessionReport.Project_ID = project_id;
                        sessionReport.Name = tilte_Cycles + " " + i;
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
                        sessionReport.Project_ID = project_id;
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
                        sessionReport.Project_ID = project_id;
                        sessionReport.Name = tilte_Cycles + " " + i;
                        sessionReport.Date_Start = date_Start;
                        sessionReport.Date_End = date_Start.AddMonths(cycles).AddDays(-1);
                        db.SessionReports.Add(sessionReport);

                        date_Start = date_Start.AddMonths(cycles);
                    }
                }
                db.SaveChanges();
                return RedirectToAction("Index", new { project_id = project_id });
            }
            
            return View();
        }


        // GET: Session_Reports/Edit/5
        public ActionResult Edit(int? id, string name,
             DateTime date_Start, DateTime date_End)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SessionReport sessionReport = db.SessionReports.Find(id);
            sessionReport.Name = name;
            sessionReport.Date_Start = date_Start;
            sessionReport.Date_End = date_End;
            db.Entry(sessionReport).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { project_id = sessionReport.Project_ID });
        }

        // GET: Session_Reports/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SessionReport sessionReport = db.SessionReports.Find(id);
            sessionReport.State = "Deleted";
            db.Entry(sessionReport).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", new { project_id = sessionReport.Project_ID });
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
