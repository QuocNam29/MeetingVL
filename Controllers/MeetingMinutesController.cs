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
    public class MeetingMinutesController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: MeetingMinutes
        public ActionResult Index()
        {
            var meetingMinutes = db.MeetingMinutes.Include(m => m.User);
            return View(meetingMinutes.ToList());
        }

        public ActionResult Content_Meeting()
        {
            var meetingMinutes = db.MeetingMinutes.Include(m => m.User);
            return View(meetingMinutes.ToList());
        }

        // GET: MeetingMinutes/Details/5
        public ActionResult Details(int? id)
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
            return View(meetingMinute);
        }

        // GET: MeetingMinutes/Create
        public ActionResult Create()
        {
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang");
            return View();
        }

        // POST: MeetingMinutes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,User_ID,SessionReport,Date,Location,Objectives,Content,Customer,Mentor,TeamMember,Time")] MeetingMinute meetingMinute)
        {
            if (ModelState.IsValid)
            {
                db.MeetingMinutes.Add(meetingMinute);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", meetingMinute.User_ID);
            return View(meetingMinute);
        }

        // GET: MeetingMinutes/Edit/5
        public ActionResult Edit(int? id)
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
            return View(meetingMinute);
        }

        // POST: MeetingMinutes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,User_ID,SessionReport,Date,Location,Objectives,Content,Customer,Mentor,TeamMember,Time")] MeetingMinute meetingMinute)
        {
            if (ModelState.IsValid)
            {
                db.Entry(meetingMinute).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", meetingMinute.User_ID);
            return View(meetingMinute);
        }

        // GET: MeetingMinutes/Delete/5
        public ActionResult Delete(int? id)
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
            return View(meetingMinute);
        }

        // POST: MeetingMinutes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MeetingMinute meetingMinute = db.MeetingMinutes.Find(id);
            db.MeetingMinutes.Remove(meetingMinute);
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
