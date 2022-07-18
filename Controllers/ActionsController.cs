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
using Action = MeetingVL.Models.Action;

namespace MeetingVL.Controllers
{
    [LoginVerification]
    public class ActionsController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();
       
       private List<Action> Action_list = null;

       public ActionsController()
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
        // GET: Actions
        public ActionResult Index(int MeetingMinute_id, int? Group_id)
        {
            
            var actions = db.Actions.Include(a => a.MeetingMinute).Include(a => a.User).Where(a => a.Meeting_ID == MeetingMinute_id).OrderBy( a => a.User.Name);
            MeetingMinute meetingMinute = db.MeetingMinutes.Find(MeetingMinute_id);
            SessionReport sessionReport = db.SessionReports.Find(meetingMinute.SessionReport_ID);

            TempData["Group_id"] = Group_id;
            TempData["Project_id"] = sessionReport.Project_ID;

            return View(actions.ToList());
        }

        [HttpPost]
        public ActionResult InsertAction(Action tire)
        {
            if (tire == null)
            {
                 return HttpNotFound();
            }
            Action_list.Add(new Action
            {
                Work = tire.Work,
                Deadline = tire.Deadline, 
                Description = tire.Description,
                User_ID = tire.User_ID
            });

            return Json(tire);
        }

        // GET: Actions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Action action = db.Actions.Find(id);
            if (action == null)
            {
                return HttpNotFound();
            }
            return View(action);
        }

        // GET: Actions/Create
        public ActionResult Create()
        {
            ViewBag.Meeting_ID = new SelectList(db.MeetingMinutes, "ID", "User_ID");
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang");
            return View();
        }

        // POST: Actions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,User_ID,Meeting_ID,Work,Deadline")] Action action)
        {
            if (ModelState.IsValid)
            {
                db.Actions.Add(action);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Meeting_ID = new SelectList(db.MeetingMinutes, "ID", "User_ID", action.Meeting_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", action.User_ID);
            return View(action);
        }

        // GET: Actions/Edit/5
        public ActionResult Edit(int? id, string Member, string Action, 
            DateTime Deadline, string Descriptions, int meeting_id)
        {
            Action action = db.Actions.Find(id);
            action.User_ID = Member;
            action.Work = Action;
            action.Deadline = Deadline;
            if (!String.IsNullOrEmpty(Descriptions))
            {
                 action.Description = Descriptions;
            }

            db.Entry(action).State = EntityState.Modified;
            db.SaveChanges();
            Session["notification"] = "Successfully Edit Action";
            return RedirectToAction("Details", "MeetingMinutes", new { id = meeting_id });
        }


        // GET: Actions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Action action = db.Actions.Find(id);
            if (action == null)
            {
                return HttpNotFound();
            }
            return View(action);
        }

        // POST: Actions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Action action = db.Actions.Find(id);
            db.Actions.Remove(action);
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
