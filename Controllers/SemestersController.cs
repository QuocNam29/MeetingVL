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
    public class SemestersController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: Semesters
        public ActionResult Index(int project_id)
        {
            var semesters = db.Semesters.Include(s => s.User).Where(s => s.State != "Deleted" && s.Project_ID == project_id);
           
            string ID_User = Session["ID_User"].ToString();
            var Check = db.ProjectParticipants.Where(r => r.User_ID == ID_User && r.Project_ID == project_id
            && r.Group.State != "Deleted").FirstOrDefault();
            TempData["roles_Project"] = Check.Role;
            TempData["date_start"] = Check.Project.Date_Start;
            return View(semesters.ToList());
        }

        // GET: Semesters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Semester semester = db.Semesters.Find(id);
            if (semester == null)
            {
                return HttpNotFound();
            }
            return View(semester);
        }

        // GET: Semesters/Create
        public ActionResult Create(string name, DateTime date_Start, DateTime date_End, int project_id)
        {
            string ID_User = Session["ID_User"].ToString();
            Semester semester = new Semester();
            semester.User_ID = ID_User;
            semester.Name = name;
            semester.Date = DateTime.Now;
            semester.Date_start = date_Start;
            semester.Date_end = date_End;
            semester.Project_ID = project_id;
            db.Semesters.Add(semester);
            db.SaveChanges();

            var check = db.SessionReports.Where(s => s.Date_End <= date_End && s.Date_Start >= date_Start 
                                        && s.Project_ID == project_id  && s.State != "Deleted").ToArray();
            for (int i = 0; i < check.Length; i++)
            {
                var session = check[i];
                Session_Semester session_Semester = new Session_Semester();
                session_Semester.Semester_ID = semester.ID;
                session_Semester.SessionReport_ID = session.ID;
                db.Session_Semester.Add(session_Semester);
                db.SaveChanges();
            }
            Session["notification"] = "Successfully Create Semester";

            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 4 });
        }

        

        // GET: Semesters/Edit/5
        public ActionResult Edit(int? id, string name, DateTime date_Start, DateTime date_End, int project_id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Semester semester = db.Semesters.Find(id);
            if (semester == null)
            {
                return HttpNotFound();
            }
            semester.Name = name;
            semester.Date_start = date_Start;
            semester.Date_end = date_End;

            var session_semester = db.Session_Semester.Where(s => s.Semester_ID == id).Count();
            for (int i = 0; i < session_semester; i++)
            {
                var delete_session = db.Session_Semester.Where(s => s.Semester_ID == id).FirstOrDefault();
                db.Session_Semester.Remove(delete_session);
                db.SaveChanges();
            }
            


            var check = db.SessionReports.Where(s => s.Date_End <= date_End && s.Date_Start >= date_Start
                                       && s.Project_ID == project_id && s.State != "Deleted").ToArray();
            for (int i = 0; i < check.Length; i++)
            {
                var session = check[i];                
                 Session_Semester session_Semester = new Session_Semester();
                                session_Semester.Semester_ID = semester.ID;
                                session_Semester.SessionReport_ID = session.ID;
                    db.Session_Semester.Add(session_Semester);                          
                
            }
            db.SaveChanges();

            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", semester.User_ID);
            db.Entry(semester).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 4 });
        }

        

        // GET: Semesters/Delete/5
        public ActionResult Delete(int? id, int project_id)
        {            
            Semester semester = db.Semesters.Find(id);
            semester.State = "Deleted";
            db.Entry(semester).State = EntityState.Modified;
            db.SaveChanges();
            Session["notification"] = "Successfully Delete Semester";
            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 4 });
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
