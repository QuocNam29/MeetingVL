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
        public ActionResult Index(int project_id, string keyword, int active)
        {
            var sessionReports = db.SessionReports.Include(s => s.Project).Where(p => p.Project_ID == project_id);

            var links = from l in db.SessionReports.Include(s => s.Project)
                        .Where(p => p.Project_ID == project_id).Where(s => s.State != "Deleted")
                        select l;
            if (active == 1)
            { 
                Session["List_SessionReport"] = "active";
                Session["List_Member"] = ""; 
                Session["List_Group"] = "";
            }
            else if (active == 2)
            {
                Session["List_SessionReport"] = "";
                Session["List_Member"] = "active";
                Session["List_Group"] = "";
            }
            else
            {
                Session["List_SessionReport"] = "";
                Session["List_Member"] = "";
                Session["List_Group"] = "active";
            }
           

            if (!string.IsNullOrEmpty(keyword))
            {
                links = links.Where(b => b.Name.ToLower().Contains(keyword.ToLower().Trim()));
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
        public ActionResult List_SessionReport(int project_id, string keyword)
        {

            var links = from l in db.SessionReports.Include(s => s.Project)
                        .Where(p => p.Project_ID == project_id).Where(s => s.State != "Deleted")
                        select l;
            string ID_User = Session["ID_User"].ToString();
            var Check = db.ProjectParticipants.Where(r => r.User_ID == ID_User && r.Project_ID == project_id).FirstOrDefault();
            if (Check != null)
            {
            TempData["roles_Project"] = Check.Role;
            }
          

            if (!string.IsNullOrEmpty(keyword))
            {
                links = links.Where(b => b.Name.ToLower().Contains(keyword.ToLower().Trim()));
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
            ViewBag.SessionReport = true;

            
            return View(links.ToList());
        }
        public ActionResult Student_SReport(int project_id, string keyword, int active)
        {
            var sessionReports = db.SessionReports.Include(s => s.Project).Where(p => p.Project_ID == project_id);

            var links = from l in db.SessionReports.Include(s => s.Project)
                        .Where(p => p.Project_ID == project_id).Where(s => s.State != "Deleted")
                        select l;
            if (active == 1)
            {
                Session["List_SessionReport"] = "active";
                Session["List_Member"] = "";
                Session["List_Group"] = "";
            }
            else if (active == 2)
            {
                Session["List_SessionReport"] = "";
                Session["List_Member"] = "active";
                Session["List_Group"] = "";
            }
            else
            {
                Session["List_SessionReport"] = "";
                Session["List_Member"] = "";
                Session["List_Group"] = "active";
            }


            if (!string.IsNullOrEmpty(keyword))
            {
                links = links.Where(b => b.Name.ToLower().Contains(keyword.ToLower().Trim()));
                TempData["keyword"] = keyword;

                Project project1 = db.Projects.Find(project_id);
                TempData["project_id"] = project_id;
                TempData["project_Name"] = project1.Name;
                TempData["project_Description"] = project1.Description;
               


                return View(links.ToList());
            }

            Project project = db.Projects.Find(project_id);
            TempData["project_id"] = project_id;
            TempData["project_Name"] = project.Name;
            TempData["project_Description"] = project.Description;
            

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
            int start = 0;
            int location = -1;
            var resule = db.SessionReports.OrderByDescending(s => s.ID).FirstOrDefault(p => p.Project_ID == project_id && p.State != "Deleted" && p.Name.StartsWith(tilte_Cycles));
            if (resule!= null)
            {
                 location = resule.Name.LastIndexOf(tilte_Cycles);
            }           
            if (location >=0)
            {
                string hihi = resule.Name.Substring(tilte_Cycles.Length);
                if (!String.IsNullOrEmpty(hihi))
                {
                    start = int.Parse(hihi);
                }
            }
            
            if (option == 1)
            {
                start++;
                string finish_tilte_Cycles = tilte_Cycles + " " + start;
                SessionReport sessionReport = new SessionReport();
                sessionReport.Project_ID = project_id;
                sessionReport.Name = finish_tilte_Cycles;
                sessionReport.Date_Start = date_Start;
                sessionReport.Date_End = date_End;
                db.SessionReports.Add(sessionReport);
                db.SaveChanges();
                Session["notification"] = "Successfully Added Session Report";
                return RedirectToAction("Index", new { project_id = project_id });
            }

            else if (option == 2)
            {                           
                TimeSpan Time = date_End - date_Start;
                int SumTime = Time.Days + 1;
                if (time == "Day")
                {
                    int CyclesperTime = SumTime / cycles + start;
                    start++; 
                    for (int i = start; i <= CyclesperTime; i++)
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
                    int CyclesperTime = SumTime / (7 * cycles) +start;
                    start++;
                    for (int i = start; i <= CyclesperTime; i++)
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
                    int CyclesperTime = SumTime / (28 * cycles) + start;
                    start++;
                    for (int i = start; i <= CyclesperTime; i++)
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
                Session["notification"] = "Successfully Added New Session Report";
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
            Session["notification"] = "Successfully Edited Session Report";
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
            Session["notification"] = "Successfully Deleted Session Report";
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
