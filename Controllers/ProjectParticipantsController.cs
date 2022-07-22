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
    public class ProjectParticipantsController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: ProjectParticipants
        public ActionResult Index(string keyword)
        {
            string email = Session["ID_User"].ToString();
            var projectParticipants = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Where(p => p.User_ID == email && p.Role == "Student").OrderBy(p => p.Group.Name);

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
        public ActionResult List_member(int project_id)
        {

            var member = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Where(p => p.Project_ID == project_id && p.User_ID != null);

            ViewBag.Group = null;

            string ID_User = Session["ID_User"].ToString();
            var Check = db.ProjectParticipants.Where(r => r.User_ID == ID_User && r.Project_ID == project_id 
            && r.Group.State != "Deleted").FirstOrDefault();
            TempData["roles_Project"] = Check.Role;



            return View(member.ToList());
        }
        public ActionResult List_Group(int project_id, string keyword)
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

            return View(links.ToList());
        }
        public ActionResult List_Group_Semester(int project_id, int semester_id)
        {
            var links = from l in db.ProjectParticipants.Include(p => p.Project).Include(p => p.User)
                        .Where(p => p.Project_ID == project_id && p.Group_ID != null && p.Group.State != "Deleted")
                        select l;

            
            TempData["project_id"] = project_id;
            TempData["semester_id"] = semester_id;

            return View(links.ToList());
        }

        public ActionResult Search_Group(int project_id, string keyword)
        {

            Session["Keyword_Group"] = keyword;

            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 3 });
        }
        public ActionResult List_Member_Group(int project_id, int group_id)
        {

            var member = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User)
                .Where(p => p.Project_ID == project_id && p.Group_ID == group_id && p.User_ID != null);
            TempData["project_id"] = project_id;

            return View(member.ToList());
        }
        public ActionResult List_Member_Meeting(int project_id, int group_id )
        {
         
            var member = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User)
                .Where(p => p.Project_ID == project_id && p.Group_ID == group_id && p.User_ID != null);
            TempData["project_id"] = project_id;

            return View(member.ToList());
        }
        public ActionResult List_Group_formAdd(int project_id)
        {
            var group = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Where(p => p.Project_ID == project_id && p.Group_ID != null).OrderBy(p => p.Group.ID);

            return View(group.ToList());
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
                Session["ViewBag.Success"] = "Import student successful !";

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
              
            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 2 });
        }

      
        public ActionResult Delete(int id, int project_id)
        {
            ProjectParticipant projectParticipant = db.ProjectParticipants.Find(id);
            db.ProjectParticipants.Remove(projectParticipant);
            db.SaveChanges();
            Session["ViewBag.FileStatus"] = null;
            Session["ViewBag.Success"] = "Delete user successful !";
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
