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
                        .Include(p => p.User).Where(p => p.User_ID == email && p.Role == "Student")
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

            var member = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Where(p => p.Project_ID == project_id);         
            TempData["project_id"] = project_id;

            return View(member.ToList());
        }
        public ActionResult List_Group(int project_id, string keyword)
        {

            var member = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Where(p => p.Project_ID == project_id && p.Group_ID != null);
            if (Session["Keyword_Group"] != null)
            {
                keyword = Session["Keyword_Group"].ToString();
            }
            
            var links = from l in db.ProjectParticipants.Include(p => p.Project).Include(p => p.User)
                        .Where(p => p.Project_ID == project_id && p.Group_ID != null)
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

        public ActionResult Search_Group(int project_id, string keyword)
        {

            Session["Keyword_Group"] = keyword;

            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 3 });
        }
        public ActionResult List_Member_Group(int project_id, int group_id)
        {

            var member = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Where(p => p.Project_ID == project_id && p.Group_ID == group_id);
            TempData["project_id"] = project_id;

            return View(member.ToList());
        }
        public ActionResult List_Member_Meeting(int project_id, int group_id)
        {

            var member = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Where(p => p.Project_ID == project_id && p.Group_ID == group_id);
            TempData["project_id"] = project_id;

            return View(member.ToList());
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
        public ActionResult Create(string[] addStudent, int project_id)
        {
            string exist = "Đã tồn tại ";
            string user_null = "Chưa tồn tại ";
            
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
           
            exist += "trong project";
            user_null += "trong hệ thống";
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


        // GET: ProjectParticipants/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.Project_ID = new SelectList(db.Projects, "ID", "Name", projectParticipant.Project_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", projectParticipant.User_ID);
            return View(projectParticipant);
        }

        // POST: ProjectParticipants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,User_ID,Project_ID,Group,Role")] ProjectParticipant projectParticipant)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectParticipant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Project_ID = new SelectList(db.Projects, "ID", "Name", projectParticipant.Project_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", projectParticipant.User_ID);
            return View(projectParticipant);
        }

        // GET: ProjectParticipants/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: ProjectParticipants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectParticipant projectParticipant = db.ProjectParticipants.Find(id);
            db.ProjectParticipants.Remove(projectParticipant);
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
