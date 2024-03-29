﻿using System;
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
    public class ProjectParticipantsController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();
        private List<ProjectParticipant> Action_list = null;
        public ProjectParticipantsController()
        {
            var session = System.Web.HttpContext.Current.Session;
            if (session["Action_list"] != null)
            {
                Action_list = session["Action_list"] as List<ProjectParticipant>;
            }
            else
            {
                Action_list = new List<ProjectParticipant>();
                session["Action_list"] = Action_list;
            }
        }
        // GET: ProjectParticipants
        public ActionResult Index(string keyword)
        {
            string email = Session["ID_User"].ToString();

            var links = from l in db.ProjectParticipants.Include(p => p.Project)
                        .Include(p => p.User).Where(p => p.User_ID == email && p.Role == "Student" 
                        && ( p.Project.State != "Deleted" || p.Project.Category.State != "Deleted"))
                        select l;
            if (!string.IsNullOrEmpty(keyword))
            {
                links = links.Where(b => b.Project.Name.ToLower().Contains(keyword.ToLower().Trim()));

                return View(links.ToList());
            }


            return View(links.ToList());
        }
        public ActionResult Project_management(string keyword)
        {
            string email = Session["ID_User"].ToString();

            var links = from l in db.ProjectParticipants.Include(p => p.Project)
                        .Include(p => p.User).Where(p => p.User_ID == email && p.Role == "Manager" && p.Project.Category.ID_User != email
                        && (p.Project.State != "Deleted" || p.Project.Category.State != "Deleted"))
                        select l;
           


            return View(links.ToList());
        }
        public ActionResult List_member(int project_id)
        {

            var member = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Where(p => p.Project_ID == project_id);

            ViewBag.Group = null;

            string ID_User = Session["ID_User"].ToString();
            var Check = db.ProjectParticipants.Where(r => r.User_ID == ID_User && r.Project_ID == project_id 
            && r.Group.State != "Deleted").FirstOrDefault();
            TempData["roles_Project"] = Check.Role;



            return View(member.ToList());
        }
        public ActionResult List_Group(int project_id, string keyword, int? action)
        {

            if (Session["Keyword_Group"] != null)
            {
                keyword = Session["Keyword_Group"].ToString();
            }
            string ID_User = Session["ID_User"].ToString();
            var Check = db.ProjectParticipants.Where(r => r.User_ID == ID_User && r.Project_ID == project_id).FirstOrDefault();
            TempData["roles_Project"] = Check.Role;

            var links = from l in db.ProjectParticipants.Include(p => p.Project).Include(p => p.User)
                        .Where(p => p.Project_ID == project_id && p.Group_ID != null && p.Group.State != "Deleted").OrderBy(p => p.Group_ID)
                        select l;

            if (!string.IsNullOrEmpty(keyword))
            {
                links = links.Where(b => b.Group.Name.ToLower().Contains(keyword.ToLower().Trim())
                || b.Group.Topic.ToLower().Contains(keyword.ToLower().Trim())
                || b.Group.Mentor.ToLower().Contains(keyword.ToLower().Trim())
                || b.Group.Customer.ToLower().Contains(keyword.ToLower().Trim())
                || b.User.Email.ToLower().Contains(keyword.ToLower().Trim())
                || b.User.Name.ToLower().Contains(keyword.ToLower().Trim()));
            }
            TempData["project_id"] = project_id;
            TempData["action"] = action;

            return View(links.ToList());
        }
       
        [HttpPost]
        public JsonResult List_Group_Statistics(int project_id)
        {

            
            string ID_User = Session["ID_User"].ToString();
            var Check = db.ProjectParticipants.Where(r => r.User_ID == ID_User && r.Project_ID == project_id).FirstOrDefault();
            TempData["roles_Project"] = Check.Role;

            /*var links = from l in db.ProjectParticipants.Include(p => p.Project).Include(p => p.User)
                        .Where(p => p.Project_ID == project_id && p.Group_ID != null && p.Group.State != "Deleted").OrderBy(p => p.Group_ID)
                        select l;*/
            TempData["project_id"] = project_id;

            return Json(db.ProjectParticipants.Include(p => p.Project).Include(p => p.User)
                .Where(p => p.Project_ID == project_id && p.Group_ID != null && p.Group.State != "Deleted").GroupBy(p => p.Group_ID)
                .OrderBy(p => p.Key).Select(s => new
                {
                    id = s.Key,
                    group = s.Select(p => p.Group.Name).FirstOrDefault(),
                    sum = db.MeetingMinutes.Where(m => m.Group_ID == s.Key && m.SessionReport.Project_ID == project_id && m.State !="Deleted").GroupBy(m => m.SessionReport_ID).Count()
                   
                }).ToList(), JsonRequestBehavior.AllowGet) ;

            /*return Json(links.ToList(), JsonRequestBehavior.AllowGet);*/
        }

        [HttpPost]
        public JsonResult List_Group_Statistics_Semester(int project_id, int semester_id)
        {


            string ID_User = Session["ID_User"].ToString();

            TempData["project_id"] = project_id;
            Semester semester = db.Semesters.Find(semester_id);

            return Json(db.ProjectParticipants.Include(p => p.Project).Include(p => p.User)
                .Where(p => p.Project_ID == project_id && p.Group_ID != null && p.Group.State != "Deleted").GroupBy(p => p.Group_ID)
                .OrderBy(p => p.Key).Select(s => new
                {
                    id = s.Key,
                    group = s.Select(p => p.Group.Name).FirstOrDefault(),
                    sum = db.MeetingMinutes.Where(m => m.Group_ID == s.Key && m.SessionReport.Project_ID == project_id && m.State != "Deleted"
                            && m.SessionReport.Date_Start >= semester.Date_start && m.SessionReport.Date_End <= semester.Date_end).GroupBy(m => m.SessionReport_ID).Count(),
                    point = db.Evaluates.Where(e => e.Group.State != "Deleted" && e.Semester.Project_ID == project_id && e.Semester_ID == semester_id
                            && e.Status != "Deleted" && e.Group_ID == s.Key).Select(q => q.Point).FirstOrDefault()
                }).ToList(), JsonRequestBehavior.AllowGet) ;

            /*return Json(links.ToList(), JsonRequestBehavior.AllowGet);*/
        }
      
        public ActionResult Search_Group(int project_id, string keyword)
        {

            Session["Keyword_Group"] = keyword;

            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 3 });
        }
      
        public ActionResult List_Member_Meeting(int project_id, int group_id, int? action )
        {
           
               var member = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User)
                .Where(p => p.Project_ID == project_id && p.Group_ID == group_id && p.User_ID != null);
            TempData["project_id"] = project_id;
            TempData["action"] = action;
           
           
            return View(member.ToList());
        }


        [HttpPost]
        public ActionResult JSDeleteMember(ProjectParticipant member)
        {
            if (member == null)
            {
                return HttpNotFound();
            }
            Action_list.Remove(member);

            return Json(member);
        }


        // GET: ProjectParticipants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectParticipant projectParticipant = db.ProjectParticipants.Find(id);
            if (projectParticipant == null)
            {
                return HttpNotFound();
            }
            return View(projectParticipant);
        }

        // GET: ProjectParticipants/Create
        public ActionResult Create(string[] addStudent, int project_id, int? group_id)
        {
            string exist = "Already exists ";
            string user_null = "There is no ";

            bool flat = true;
            bool flat_user = true;
            if (addStudent != null)
            {
                for (int i = 0; i < addStudent.Length; i++)
                {
                    string student = addStudent[i].Trim();
                    var check_user = db.Users.Find(student);
                    if (check_user != null)
                    {
                        var check_student = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Include(p => p.Group)
                         .Where(c => c.User_ID == student && c.Project_ID == project_id).FirstOrDefault();

                        if (check_student == null)
                        {
                            ProjectParticipant projectParticipant = new ProjectParticipant();
                            projectParticipant.Project_ID = project_id;
                            projectParticipant.User_ID = addStudent[i].Trim();
                            projectParticipant.Role = "Student";
                            if (group_id != null)
                            {
                                projectParticipant.Group_ID = group_id;
                            }
                           
                            db.ProjectParticipants.Add(projectParticipant);
                            db.SaveChanges();
                        }
                        else
                        {
                            flat = false;
                            exist += addStudent[i].Trim() + " ";
                        }
                    }
                    else
                    {
                        flat_user = false;
                        user_null += addStudent[i].Trim() + " ";
                    }
                }
                Session["ViewBag.FileStatus"] = null;
                Session["ViewBag.Success"] = null;
            }
            else
            {
                Session["ViewBag.Success"] = null;
                Session["ViewBag.FileStatus"] = "You have not entered student !";
            }

            exist += "in the project";
            user_null += "in the system yet";
            if (flat == false)
            {
                Session["ViewBag.Success"] = null;
                Session["ViewBag.FileStatus"] = exist;
            }
            if (flat_user == false)
            {
                Session["ViewBag.Success"] = null;
                Session["ViewBag.FileStatus"] = user_null;
            }
            if (Session["ViewBag.Success"] != null)
            {
                Session["notification"] = "Successfully Add student";
            }

            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 2 });
        }


       
        public ActionResult Edit(string email, int? group_id, string role, int project_id)
        {
            var projectParticipant = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Include(p => p.Group)
                                     .Where(p => p.Project_ID == project_id && p.User_ID == email).FirstOrDefault();
            projectParticipant.Group_ID = group_id;
            projectParticipant.Role = role;
                db.Entry(projectParticipant).State = EntityState.Modified;
                db.SaveChanges();
            Session["notification"] = "Successfully Edit User";
            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 2 });
        }

      
        public ActionResult Delete(int id, int project_id)
        {
            ProjectParticipant projectParticipant = db.ProjectParticipants.Find(id);
            db.ProjectParticipants.Remove(projectParticipant);
            db.SaveChanges();
            Session["ViewBag.FileStatus"] = null;
            Session["ViewBag.Success"] = null;
            Session["notification"] = "Delete user successful !";
            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 2 });
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
