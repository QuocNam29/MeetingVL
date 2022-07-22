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
        public ActionResult EditForm(int MeetingMinute_id, int? Group_id)
        {

            var actions = db.Actions.Include(a => a.MeetingMinute).Include(a => a.User).Where(a => a.Meeting_ID == MeetingMinute_id).OrderBy(a => a.User.Name);
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
        public ActionResult FormCreate(int meeting_id, int group_id)
        {
            TempData["group_id"] = group_id;
            TempData["meeting_id"] = meeting_id;
            MeetingMinute meetingMinute = db.MeetingMinutes.Find(meeting_id);
            TempData["session_id"] = meetingMinute.SessionReport_ID;
            TempData["project_id"] = meetingMinute.SessionReport.Project_ID;
            return View();
        }


        public ActionResult Create(int meeting_id, int group_id, string action, string Member, DateTime Deadline, string Descriptions)
        {
            Action action1 = new Action();
            action1.Meeting_ID = meeting_id;
            action1.Work = action;
            action1.User_ID = Member;
            action1.Deadline = Deadline;
            action1.Description = Descriptions;
            db.Actions.Add(action1);
            db.SaveChanges();
            Session["notification"] = "Successfully Add Action";
            return RedirectToAction("FormCreate", "Actions", new { meeting_id = meeting_id, group_id = group_id });
        }

        // GET: Actions/Edit/5
        public ActionResult Edit(int? id, string Member, string Action, 
            DateTime Deadline, string Descriptions, int meeting_id, int group_id)
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
            return RedirectToAction("FormCreate", "Actions", new { meeting_id = meeting_id, group_id = group_id });
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
        public ActionResult DeleteConfirmed(int id, int? meeting_id)
        {
            Action action = db.Actions.Find(id);
            db.Actions.Remove(action);
            db.SaveChanges();
            Session["notification"] = "Successfully Delete Action";
            
            return RedirectToAction("Details", "MeetingMinutes", new { id = meeting_id });
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
