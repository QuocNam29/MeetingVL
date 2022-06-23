using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MeetingVL.Models;

namespace MeetingVL.Controllers
{
    public class UsersController : Controller
    {
        private MeetingVLEntities db = new MeetingVLEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }
        public ActionResult List_Edit()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(string email)
        {
            User user = db.Users.Find(email);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create(string name, string email, string role, string department, string majors)
        {
            User user = new User();
            if (!string.IsNullOrWhiteSpace(name))
            {
                user.Name = name;
            }
            if (!string.IsNullOrWhiteSpace(department))
            {
                user.Department = department;
            }
            if (!string.IsNullOrWhiteSpace(majors))
            {
                user.Majors = majors;
            }
            user.Email = email;
            user.Role = role;
            
           
            db.Users.Add(user);
            db.SaveChanges();
            return RedirectToAction("Index");
            
        }

      

        // GET: Users/Edit/5
        public ActionResult Edit( string name, string email, string role, string department, string majors, string state)
        {
           
            User user = db.Users.Find(email);
            if (!string.IsNullOrWhiteSpace(name))
            {
                user.Name = name;
            }
            if (!string.IsNullOrWhiteSpace(department))
            {
                user.Department = department;
            }
            if (!string.IsNullOrWhiteSpace(majors))
            {
                user.Majors = majors;
            }
            if (state == "true")
            {
                user.State = true;
            }
            else if (state == "false")
            {
                user.State = false;
            }
           
            user.Email = email;
            user.Role = role;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Details", new { email = email });
        }

        
        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
