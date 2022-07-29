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
using Action = MeetingVL.Models.Action;

namespace MeetingVL.Controllers
{

    [LoginVerification]
    public class MeetingMinutesController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();
        private List<Action> Action_list = null;
        public void GetAction_list()
        {
            var session = System.Web.HttpContext.Current.Session;

            if (session["Action_list"] != null)
            {
                Action_list = session["Action_list"] as List<Action>;
            }
            else
            {
                Action_list = new List<Action>();
                session["Action_list"] = Action_list;
            }
        }
        // GET: MeetingMinutes
        public ActionResult Index(int session_id, string keyword)
        {
            var links = from l in db.MeetingMinutes.Include(m => m.User)
                        .Where(m => m.SessionReport_ID == session_id && m.State != "Deleted").OrderBy(m => m.Group.Name)
                        select l;
            if (!string.IsNullOrEmpty(keyword))
            {
                links = links.Where(b => b.Group.Name.ToLower().Contains(keyword.ToLower().Trim())
                && b.User_ID.ToLower().Contains(keyword.ToLower().Trim()));
                SessionReport sessionReport1 = db.SessionReports.Find(session_id);
                TempData["session_id"] = session_id;
                TempData["session_Name"] = sessionReport1.Name;

                Project project1 = db.Projects.Find(sessionReport1.Project_ID);
                TempData["project_id"] = sessionReport1.Project_ID;
                TempData["project_Name"] = project1.Name;
                TempData["project_Description"] = project1.Description;

                Category category1 = db.Categories.Find(project1.Category_ID);
                TempData["category_id"] = category1.ID;
                TempData["category_Name"] = category1.Name;

                string ID_User1 = Session["ID_User"].ToString();
                ProjectParticipant projectParticipant1 = db.ProjectParticipants.FirstOrDefault(p => p.Project_ID == project1.ID
           && p.User_ID == ID_User1 && p.Group_ID != null && p.Group.State != "Deleted");
                if (projectParticipant1 != null)
                {
                    TempData["group_id"] = projectParticipant1.Group_ID;
                    TempData["role"] = projectParticipant1.Role;
                }
               

                return View(links.ToList());
            }
            SessionReport sessionReport = db.SessionReports.Find(session_id);
            TempData["session_id"] = session_id;
            TempData["session_Name"] = sessionReport.Name;

            Project project = db.Projects.Find(sessionReport.Project_ID);
            TempData["project_id"] = sessionReport.Project_ID;
            TempData["project_Name"] = project.Name;
            TempData["project_Description"] = project.Description;

            Category category = db.Categories.Find(project.Category_ID);
            TempData["category_id"] = category.ID;
            TempData["category_Name"] = category.Name;
            string ID_User = Session["ID_User"].ToString();
            ProjectParticipant projectParticipant = db.ProjectParticipants.FirstOrDefault(p => p.Project_ID == project.ID 
            && p.User_ID == ID_User && p.Group_ID != null && p.Group.State != "Deleted");

            if (projectParticipant != null)
            {
                TempData["group_id"] = projectParticipant.Group_ID;
                TempData["role"] = projectParticipant.Role;
            }
           
            

            return View(links.ToList());
        }
        public ActionResult List_Meeting_Semester(int session_id,int group_id)
        {
            var meetingMinutes = db.MeetingMinutes.Include(m => m.User).Where(m => m.SessionReport_ID == session_id && m.State != "Deleted").OrderBy(m => m.Group.Name);
            var links = from l in db.MeetingMinutes.Include(m => m.User)
                        .Where(m => m.SessionReport_ID == session_id && m.Group_ID == group_id && m.State != "Deleted").OrderBy(m => m.Group.Name)
                        select l;
           
            SessionReport sessionReport = db.SessionReports.Find(session_id);
            TempData["session_id"] = session_id;
            TempData["session_Name"] = sessionReport.Name;

            Project project = db.Projects.Find(sessionReport.Project_ID);
            TempData["project_id"] = sessionReport.Project_ID;
            TempData["project_Name"] = project.Name;
            TempData["project_Description"] = project.Description;

            Category category = db.Categories.Find(project.Category_ID);
            TempData["category_id"] = category.ID;
            TempData["category_Name"] = category.Name;
            TempData["group_id"] = group_id;

            return View(links.ToList());
        }

        public ActionResult Content_Meeting()
        {
            var meetingMinutes = db.MeetingMinutes.Include(m => m.User);
            return View(meetingMinutes.ToList());
        }
        public ActionResult Demo_form()
        {
            var meetingMinutes = db.MeetingMinutes.Include(m => m.User);
            return View(meetingMinutes.ToList());
        }

        // GET: MeetingMinutes/Details/5
        public ActionResult Details(int? id, int? group_id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MeetingMinute meetingMinute = db.MeetingMinutes.Find(id);
            if (meetingMinute == null)
            {
                return HttpNotFound();
            }
            SessionReport sessionReport = db.SessionReports.Find(meetingMinute.SessionReport_ID);
            TempData["session_id"] = sessionReport.ID;
            TempData["session_Name"] = sessionReport.Name;

           
            TempData["project_id"] = sessionReport.Project_ID;
            TempData["project_Name"] = sessionReport.Project.Name;
            TempData["project_Description"] = sessionReport.Project.Description;

            TempData["category_id"] = sessionReport.Project.Category.ID;
            TempData["category_Name"] = sessionReport.Project.Category.Name;
            
            string ID_User = Session["ID_User"].ToString();
            ProjectParticipant projectParticipant = db.ProjectParticipants.FirstOrDefault(p => p.Project_ID == sessionReport.Project_ID
            && p.User_ID == ID_User && p.Group_ID == group_id && p.Group.State != "Deleted");

            if (projectParticipant != null)
            {
                TempData["group_id"] = projectParticipant.Group_ID;
                TempData["role"] = projectParticipant.Role;
            }


            return View(meetingMinute);
        }

        // GET: MeetingMinutes/Create
        public ActionResult Create(int session_id)
        {
            string ID_User = Session["ID_User"].ToString();
            TempData["session_id"] = session_id;

            SessionReport sessionReport = db.SessionReports.Find(session_id);
            TempData["project_id"] = sessionReport.Project_ID;

            var participant = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Where(p => p.Project_ID == sessionReport.Project_ID && p.User_ID == ID_User ).FirstOrDefault();
            TempData["group_id"] = participant.Group_ID;
            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult Create_Meeting(DateTime meetingDate, string location,
             int process, string stages, string content, string objectives,
             string issues, string NA, int session_id, int? group_id)
        {
            var session = System.Web.HttpContext.Current.Session;
            GetAction_list();
            string ID_User = Session["ID_User"].ToString();
            MeetingMinute meetingMinute = new MeetingMinute();
            meetingMinute.User_ID = ID_User;
            meetingMinute.SessionReport_ID = session_id;
            meetingMinute.Date = meetingDate;
            meetingMinute.Location = location;
            if (!String.IsNullOrEmpty(objectives))
            {
                meetingMinute.Objectives = objectives;
            }

            meetingMinute.Content = content;
            meetingMinute.Time = DateTime.Now;
            meetingMinute.Process = process;
            if (!String.IsNullOrEmpty(issues))
            {
                meetingMinute.Issues = issues;
            }
            if (!String.IsNullOrEmpty(NA))
            {
                meetingMinute.NA = NA;
            }
            meetingMinute.Stages = stages;
            if (group_id != 0)
            {
            meetingMinute.Group_ID = group_id;
            }
            
            db.MeetingMinutes.Add(meetingMinute);
            db.SaveChanges();

            foreach (var item in Action_list)
            {
                db.Actions.Add(new Action
                {
                    Work = item.Work,
                    Deadline = item.Deadline,
                    Description = item.Description,
                    Meeting_ID = meetingMinute.ID,
                    User_ID = item.User_ID
                }) ;
            }
            db.SaveChanges();
            session["Action_list"] = null;

            return RedirectToAction("Index", new { session_id = session_id });
        }

        // GET: MeetingMinutes/Edit/5
        public ActionResult Edit(int? id, int? group_id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MeetingMinute meetingMinute = db.MeetingMinutes.Find(id);
            if (meetingMinute == null)
            {
                return HttpNotFound();
            }
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", meetingMinute.User_ID);
            TempData["group_id"] = group_id;
            return View(meetingMinute);
        }

        public ActionResult Edit_form(int meeting_id, DateTime meetingDate, string location,
             int process, string stages, string content, string objectives,
             string issues, string NA, int? group_id)
        {
            MeetingMinute meetingMinute = db.MeetingMinutes.Find(meeting_id);
            
            meetingMinute.Date = meetingDate;
            meetingMinute.Location = location;
            if (!String.IsNullOrEmpty(objectives))
            {
                meetingMinute.Objectives = objectives;
            }
           
            meetingMinute.Content = content;
            meetingMinute.Time = DateTime.Now;
            meetingMinute.Process = process;
            if (!String.IsNullOrEmpty(issues))
            {
                meetingMinute.Issues = issues;
            }
            if (!String.IsNullOrEmpty(NA))
            {
                meetingMinute.NA = NA;
            }
            meetingMinute.Stages = stages;
            db.Entry(meetingMinute).State = EntityState.Modified;
            db.SaveChanges();
            Session["notification"] = "Successfully Edit Meeting Minutes";
            return RedirectToAction("Details", new { id = meeting_id , group_id  = group_id });
        }

      
        public ActionResult Delete(int? id, int? session_id)
        {
            MeetingMinute meetingMinute = db.MeetingMinutes.Find(id);
            meetingMinute.State = "Deleted";
            db.Entry(meetingMinute).State = EntityState.Modified;
            db.SaveChanges();
            Session["notification"] = "Successfully Delete Meeting Minutes";
            return RedirectToAction("Index", new { session_id = session_id });
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
