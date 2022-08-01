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
    public class CommentsController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: Comments
        public ActionResult Index(int evalute_id)
        {
            var comments = db.Comments.Include(c => c.Evaluate).Include(c => c.User).Where(c => c.Evalute_ID == evalute_id);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create(int evalute_id, string comment)
        {
            string ID_User = Session["ID_User"].ToString();
            Comment comment1 = new Comment();
            comment1.User_ID = ID_User;
            comment1.Evalute_ID = evalute_id;
            comment1.Time = DateTime.Now;
            comment1.Comment1 = comment;
            db.Comments.Add(comment1);
            db.SaveChanges();

            Evaluate evaluate = db.Evaluates.Find(evalute_id);
            var check_user = db.ProjectParticipants.Where(p => p.Project_ID == evaluate.Semester.Project_ID
          && p.User_ID == ID_User).FirstOrDefault();
            if (check_user.Role == "Manager")
            {
                var list_member_notification = db.ProjectParticipants.Where(p => p.Project_ID == evaluate.Semester.Project_ID
           && p.Group_ID == evaluate.Group_ID && p.User_ID != null).ToArray();
                for (int i = 0; i < list_member_notification.Length; i++)
                {
                    Notification notification = new Notification();
                    notification.User_ID = list_member_notification[i].User_ID;
                    notification.Time = DateTime.Now;
                    notification.Comment_ID = comment1.ID;
                    notification.Content = comment;
                    db.Notifications.Add(notification);
                    db.SaveChanges();
                }
            }
            else
            {
                var find_manager = db.ProjectParticipants.Where(p => p.Project_ID == evaluate.Semester.Project_ID
        && p.Role == "Manager").FirstOrDefault();
                Notification notification1 = new Notification();
                notification1.User_ID = find_manager.User_ID;
                notification1.Time = DateTime.Now;
                notification1.Comment_ID = comment1.ID;
                notification1.Content = comment;
                db.Notifications.Add(notification1);
                db.SaveChanges();

                var list_member_notification = db.ProjectParticipants.Where(p => p.Project_ID == evaluate.Semester.Project_ID
           && p.Group_ID == evaluate.Group_ID && p.User_ID != null && p.User_ID != ID_User).ToArray();
                for (int i = 0; i < list_member_notification.Length; i++)
                {
                    Notification notification = new Notification();
                    notification.User_ID = list_member_notification[i].User_ID;
                    notification.Time = DateTime.Now;
                    notification.Comment_ID = comment1.ID;
                    notification.Content = comment;
                    db.Notifications.Add(notification);
                    db.SaveChanges();
                }
            }
            

            return RedirectToAction("Details", "Evaluates", new { id = evalute_id });
        }

           
        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Evalute_ID = new SelectList(db.Evaluates, "ID", "User_ID", comment.Evalute_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", comment.User_ID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,User_ID,Evalute_ID,Comment1,Time")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Evalute_ID = new SelectList(db.Evaluates, "ID", "User_ID", comment.Evalute_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", comment.User_ID);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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
