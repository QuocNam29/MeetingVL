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

namespace MeetingVL.Controllers
{
    [LoginVerification]
    public class ProjectParticipantsController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: ProjectParticipants
        public ActionResult Index()
        {
            var projectParticipants = db.ProjectParticipants.Include(p => p.Project).Include(p => p.User);
            return View(projectParticipants.ToList());
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
        public ActionResult Create()
        {
            ViewBag.Project_ID = new SelectList(db.Projects, "ID", "Name");
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang");
            return View();
        }

        // POST: ProjectParticipants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,User_ID,Project_ID,Group,Role")] ProjectParticipant projectParticipant)
        {
            if (ModelState.IsValid)
            {
                db.ProjectParticipants.Add(projectParticipant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Project_ID = new SelectList(db.Projects, "ID", "Name", projectParticipant.Project_ID);
            ViewBag.User_ID = new SelectList(db.Users, "Email", "ID_VanLang", projectParticipant.User_ID);
            return View(projectParticipant);
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