using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MeetingVL.Middleware;
using MeetingVL.Models;

namespace MeetingVL.Controllers
{
    [AdminVerification]
    public class UsersController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.OrderByDescending(u => u.stt).ToList());
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
        [LoginVerification]
        public ActionResult MyProfile()
        {
            string ID_User = Session["ID_User"].ToString();
            User user = db.Users.Find(ID_User);
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
            Session["notification"] = "Successfully Added New User";
            return RedirectToAction("Index");

        }



        // GET: Users/Edit/5
        public ActionResult Edit(string name, string email, string role, string department, string majors, string state)
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
            string ID_User = Session["ID_User"].ToString();
            if (ID_User == email)
            {
                Session["ID_VL"] = user.ID_VanLang;
                Session["Name"] = name;
                Session["Role"] = role;
            }

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            if (Session["Role"].ToString() != "Admin")
            {
                Session["notification"] = "Successfully Edited User";
                return RedirectToAction("Index", "Categories");
            }

            Session["notification"] = "Successfully Edited User";
            return RedirectToAction("Details", new { email = email });
        }

        public ActionResult Edit_Profile(string name, string department, string majors, HttpPostedFileBase avt)
        {
            string ID_User = Session["ID_User"].ToString();
            User user = db.Users.Find(ID_User);
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


            string oldfilePath = user.Avt;
            if (avt != null && avt.ContentLength > 0)
            {
                string time = DateTime.Now.ToString("yymmssfff");
                var fileName = System.IO.Path.GetFileName(avt.FileName);
                string filePath = "~/Template/app-assets/img/Avatar/" + time + fileName;
                avt.SaveAs(Server.MapPath(filePath));
                user.Avt = "~/Template/app-assets/img/Avatar/" + time + avt.FileName;
                string fullPath = Request.MapPath(oldfilePath);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            else
            {
                if (Session["Avt"] != null)
                {
                    user.Avt = Session["Avt"].ToString();
                }
                else
                {
                    user.Avt = null;
                }

            }
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            Session["Avt"] = user.Avt;
            Session["notification"] = "Successfully Edited Profile";
            return RedirectToAction("MyProfile");

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
