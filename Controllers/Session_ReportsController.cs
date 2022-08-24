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
            var sessionReports = db.SessionReports.Include(s => s.Project).Where(p => p.Project_ID == project_id && p.State != "Deleted").FirstOrDefault();

          
            if (active == 1)
            { 
                Session["List_SessionReport"] = "active";
                Session["List_Member"] = ""; 
                Session["List_Group"] = "";
                Session["List_Semester"] = "";
            }
            else if (active == 2)
            {
                Session["List_SessionReport"] = "";
                Session["List_Member"] = "active";
                Session["List_Group"] = "";
                Session["List_Semester"] = "";
            }
            else if (active == 3)
            {
                Session["List_SessionReport"] = "";
                Session["List_Member"] = "";
                Session["List_Group"] = "active";
                Session["List_Semester"] = "";
            }
            else
            {
                Session["List_SessionReport"] = "";
                Session["List_Member"] = "";
                Session["List_Group"] = "";
                Session["List_Semester"] = "active";
            }
            Project project = db.Projects.Find(project_id);
            TempData["project_id"] = project_id;
            TempData["project_Name"] = project.Name;
            TempData["project_Description"] = project.Description;
            Category category = db.Categories.Find(project.Category_ID);
            TempData["category_id"] = category.ID;
            TempData["category_Name"] = category.Name;

           

            return View(sessionReports);
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
            int count_group = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User)
                         .Where(p => p.Project_ID == project_id && p.Group_ID != null && p.Group.State != "Deleted").GroupBy(p=> p.Group_ID).Count();
            if (count_group ==0)
            {
                TempData["count_group"] = 1;
            }
            else
            {
                TempData["count_group"] = count_group;
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                links = links.Where(b => b.Name.ToLower().Contains(keyword.ToLower().Trim()));
                TempData["keyword"] = keyword;

              
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
        public ActionResult List_Submit_SessionReport(int project_id,  int? Action)
        {
            string ID_User = Session["ID_User"].ToString();
            var check_group = db.ProjectParticipants.Where(g => g.Project_ID == project_id && g.User_ID == ID_User).FirstOrDefault();
            var links = from l in db.SessionReports.Include(s => s.Project)
                       .Where(p => p.Project_ID == project_id)
                        select l;
            if (Action == 1)
            {
                links = links.Where(s => s.State != "Deleted"
                       && s.MeetingMinutes.Where(c => c.Group_ID == check_group.Group_ID && c.State != "Deleted").Count() > 0);              
            }
            else if(Action == 2)
            {
                links = links.Where(s => s.State != "Deleted"
                       && s.MeetingMinutes.Where(c => c.Group_ID == check_group.Group_ID && c.State != "Deleted").Count() <= 0);
            }
            

           
           
            var Check = db.ProjectParticipants.Where(r => r.User_ID == ID_User && r.Project_ID == project_id).FirstOrDefault();
            if (Check != null)
            {
                TempData["roles_Project"] = Check.Role;
            }
          

            Project project = db.Projects.Find(project_id);
            TempData["project_id"] = project_id;
            TempData["project_Name"] = project.Name;
            TempData["project_Description"] = project.Description;
            Category category = db.Categories.Find(project.Category_ID);
            TempData["category_id"] = category.ID;
            TempData["category_Name"] = category.Name;
            ViewBag.SessionReport = true;
            if (check_group.Group_ID == null)
            {
                TempData["group_id_session"] = 0;
            }
            else
            {
                TempData["group_id_session"] = check_group.Group_ID;
            }
           

            return View(links.ToList());
        }

        public ActionResult List_Submit_Semester(int semester_id, int group_id, int? action)
        {
          
            var links = from l in db.Session_Semester
                       .Where(p => p.Semester_ID == semester_id)
                        select l;
            TempData["group_id"] = group_id;
            TempData["action"] = action;

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
                Session["List_Semester"] = "";
            }
            else if (active == 2)
            {
                Session["List_SessionReport"] = "";
                Session["List_Member"] = "active";
                Session["List_Group"] = "";
                Session["List_Semester"] = "";
            }
            else if (active == 3)
            {
                Session["List_SessionReport"] = "";
                Session["List_Member"] = "";
                Session["List_Group"] = "active";
                Session["List_Semester"] = "";
            }
            else
            {
                Session["List_SessionReport"] = "";
                Session["List_Member"] = "";
                Session["List_Group"] = "";
                Session["List_Semester"] = "active";
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

            string ID_User = Session["ID_User"].ToString();
            var check_group = db.ProjectParticipants.Where(g => g.Project_ID == project_id && g.User_ID == ID_User).FirstOrDefault();
            var count_submit = links.Where(s =>s.MeetingMinutes.Where(c => c.Group_ID == check_group.Group_ID && c.State != "Deleted").Count() > 0);
            TempData["count_submit"] = count_submit.Count();
            TempData["count_all"] = links.Count();

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
                return RedirectToAction("Index", new { project_id = project_id, active = 1 });
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
                return RedirectToAction("Index", new { project_id = project_id, active = 1 });
               
            }

            return RedirectToAction("Index", new { project_id = project_id, active = 1 });
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
