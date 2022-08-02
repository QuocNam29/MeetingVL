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
    public class NotificationsController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: Notifications
        public ActionResult Index()
        {
            string ID_User = Session["ID_User"].ToString();
            var notifications = db.Notifications.Include(n => n.Comment).Include(n => n.Evaluate).Include(n => n.User).Where(n => n.User_ID == ID_User).OrderByDescending(n => n.ID);
            return View(notifications.ToList());
        }
        public ActionResult Notification_header()
        {
            string ID_User = Session["ID_User"].ToString();
            var notifications = db.Notifications.Include(n => n.Comment).Include(n => n.Evaluate).Include(n => n.User).Where(n => n.User_ID == ID_User).OrderByDescending(n => n.ID);
            return View(notifications.ToList());
        }

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // GET: Notifications/Create
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.Comments, "ID", "User_ID");
            ViewBag.Evalute_ID = new SelectList(db.Evaluates, "ID", "User_ID");
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Evalute_ID,Comment_ID,User_ID,Content,Time,status")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.Comments, "ID", "User_ID", notification.ID);
            ViewBag.Evalute_ID = new SelectList(db.Evaluates, "ID", "User_ID", notification.Evalute_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", notification.User_ID);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.Comments, "ID", "User_ID", notification.ID);
            ViewBag.Evalute_ID = new SelectList(db.Evaluates, "ID", "User_ID", notification.Evalute_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", notification.User_ID);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Evalute_ID,Comment_ID,User_ID,Content,Time,status")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.Comments, "ID", "User_ID", notification.ID);
            ViewBag.Evalute_ID = new SelectList(db.Evaluates, "ID", "User_ID", notification.Evalute_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", notification.User_ID);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
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
