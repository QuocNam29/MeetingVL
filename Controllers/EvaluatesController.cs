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
    public class EvaluatesController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: Evaluates
        public ActionResult Index()
        {
            var evaluates = db.Evaluates.Include(e => e.Group).Include(e => e.User);
            return View(evaluates.ToList());
        }

        // GET: Evaluates/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluate evaluate = db.Evaluates.Find(id);
            if (evaluate == null)
            {
                return HttpNotFound();
            }
            return View(evaluate);
        }

        // GET: Evaluates/Create
        public ActionResult Create(int group_id, string review, int point)
        {
            string ID_User = Session["ID_User"].ToString();
            Evaluate evaluate = new Evaluate();
            evaluate.User_ID = ID_User;
            evaluate.Group_ID = group_id;
            evaluate.Review = review;
            evaluate.Point = point;
            db.Evaluates.Add(evaluate);
            db.SaveChanges();
            return View();
        }

       
        // GET: Evaluates/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluate evaluate = db.Evaluates.Find(id);
            if (evaluate == null)
            {
                return HttpNotFound();
            }
            ViewBag.Group_ID = new SelectList(db.Groups, "ID", "Name", evaluate.Group_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", evaluate.User_ID);
            return View(evaluate);
        }

        // POST: Evaluates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,User_ID,Group_ID,State,Review,Comment,Status")] Evaluate evaluate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(evaluate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Group_ID = new SelectList(db.Groups, "ID", "Name", evaluate.Group_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", evaluate.User_ID);
            return View(evaluate);
        }

        // GET: Evaluates/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluate evaluate = db.Evaluates.Find(id);
            if (evaluate == null)
            {
                return HttpNotFound();
            }
            return View(evaluate);
        }

        // POST: Evaluates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Evaluate evaluate = db.Evaluates.Find(id);
            db.Evaluates.Remove(evaluate);
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
