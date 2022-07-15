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
    public class GroupsController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: Groups
        public ActionResult Index()
        {
            return View(db.Groups.ToList());
        }

        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // GET: Groups/Create
        public ActionResult Create(string name, string topic, string mentor,string customer, string[] addStudent, int project_id)
        {
            Group group = new Group();
            group.Name = name;
            group.Topic = topic;
            group.Mentor = mentor;
            group.Customer = customer;
            db.Groups.Add(group);
            db.SaveChanges();

            string check_User = "Already exists ";
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
                            projectParticipant.Group_ID = group.ID;
                            db.ProjectParticipants.Add(projectParticipant);
                            db.SaveChanges();
                        }
                        else
                        {
                            if (check_student.Group_ID == null)
                            {
                                 check_student.Group_ID = group.ID;
                                db.Entry(check_student).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                flat = false;
                                check_User += check_student.User_ID + " ";
                            }
                           
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
                         
                ProjectParticipant projectParticipant1 = new ProjectParticipant();
                projectParticipant1.Project_ID = project_id;               
                projectParticipant1.Group_ID = group.ID;
                db.ProjectParticipants.Add(projectParticipant1);
                db.SaveChanges();
            

            check_User += "in another group";
            user_null += "in the system yet";
            if (flat == false)
            {
                Session["ViewBag.Success"] = null;
                Session["ViewBag.FileStatus"] = check_User;
            }
            if (flat_user == false)
            {
                Session["ViewBag.Success"] = null;
                Session["ViewBag.FileStatus"] = user_null;
            }
            return RedirectToAction("Index", "Session_Reports", new { project_id = project_id, active = 3 });
        }

       
        

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Topic,Mentor,Customer")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
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
