using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingVL.Models;
using Action = MeetingVL.Models.Action;

namespace MeetingVL.Controllers
{
    

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
        public ActionResult Index(int session_id)
        {
            var meetingMinutes = db.MeetingMinutes.Include(m => m.User);
            TempData["session_id"] = session_id;
            return View(meetingMinutes.ToList());
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
        public ActionResult Create(int session_id)
        {
            string ID_User = Session["ID_User"].ToString();
            TempData["session_id"] = session_id;

            SessionReport sessionReport = db.SessionReports.Find(session_id);
            TempData["project_id"] = sessionReport.Project_ID;

            var participant = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User).Where(p => p.Project_ID == sessionReport.Project_ID && p.User_ID == ID_User).FirstOrDefault();
            TempData["group_id"] = participant.Group_ID;
            return View();
        }

        [ValidateAntiForgeryToken]
        public ActionResult Create_Meeting(DateTime meetingDate, string location,
             int process, string stages, string content, string objectives,
             string issues, string NA, int session_id)
        {
            var session = System.Web.HttpContext.Current.Session;
            GetAction_list();
            string ID_User = Session["ID_User"].ToString();
            MeetingMinute meetingMinute = new MeetingMinute();
            meetingMinute.User_ID = ID_User;
            meetingMinute.SessionReport_ID = session_id;
            meetingMinute.Date = meetingDate;
            meetingMinute.Location = location;
            meetingMinute.Objectives = objectives;
            meetingMinute.Content = content;
            meetingMinute.Time = DateTime.Now;
            meetingMinute.Process = process;
            meetingMinute.Issues = issues;
            meetingMinute.NA = NA;
            meetingMinute.Stages = stages;
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
            session["ShoppingCart"] = null;

            return RedirectToAction("Index", new { session_id = session_id });
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
