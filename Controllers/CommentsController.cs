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
