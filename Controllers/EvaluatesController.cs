﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using MeetingVL.Middleware;
using MeetingVL.Models;

namespace MeetingVL.Controllers
{
    [LoginVerification]
    public class EvaluatesController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();

        // GET: Evaluates
        public ActionResult Index(int group_id, int semester_id)
        {
            var evaluates = db.Evaluates.Where(e => e.Group_ID == group_id && e.Semester_ID == semester_id);
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
            string ID_User1 = Session["ID_User"].ToString();
           
            var projectParticipant = db.ProjectParticipants.Where(r => r.User_ID == ID_User1 && r.Project_ID == evaluate.Semester.Project_ID
            && r.Group.State != "Deleted").FirstOrDefault();
           
            if (projectParticipant != null)
            {
                TempData["group_id"] = projectParticipant.Group_ID;
                TempData["role"] = projectParticipant.Role;
            }

            return View(evaluate);
        }

        // GET: Evaluates/Create
        public ActionResult Create(int group_id, string review, int point, int semester_id, int project_id)
        {
            string ID_User = Session["ID_User"].ToString();
            Evaluate evaluate = new Evaluate();
            evaluate.User_ID = ID_User;
            evaluate.Group_ID = group_id;
            evaluate.Review = review;
            evaluate.Point = point;
            evaluate.Semester_ID = semester_id;
            evaluate.Time = DateTime.Now;
             db.Evaluates.Add(evaluate);
            db.SaveChanges();

            Semester semester = db.Semesters.Find(semester_id);

            var list_member_notification = db.ProjectParticipants.Where(p => p.Project_ID == project_id 
            && p.Group_ID == group_id && p.User_ID != null && p.Role =="Student").ToArray();
            for (int i = 0; i < list_member_notification.Length; i++)
            {
                Notification notification = new Notification();
                notification.User_ID = list_member_notification[i].User_ID; 
                notification.Time = DateTime.Now;
                notification.Evalute_ID = evaluate.ID;
                notification.Content = evaluate.Review;
                notification.status = "New";
                db.Notifications.Add(notification);
                db.SaveChanges();

                //Khởi tạo nội dung gửi mail
                MailMessage mailmea = new MailMessage();
                mailmea.To.Add(list_member_notification[i].User_ID);
                mailmea.From = new MailAddress(@"meetingvanlang@hotmail.com");
                mailmea.Subject = "Đánh giá báo cáo meeting " + semester.Name;
                mailmea.IsBodyHtml = true;
                mailmea.Body = "<font size=10>Review: </font><br>" + "<font size=5><b>   " + review + "</b></font>";

                //Phương thức gửi mail
                SmtpClient smtp = new SmtpClient("smtp-mail.outlook.com", 587);
                smtp.UseDefaultCredentials = true;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Credentials = new NetworkCredential(@"meetingvanlang@hotmail.com", "MeetingVLTeam13"); //Email, mật khẩu ứng dụng
                try
                {
                    smtp.Send(mailmea);
                }
                catch (Exception ex)
                {
                    Session["thongbao-loi"] = ex.Message;
                }
            }

           




            return RedirectToAction("Index", "Session_Semester", new { semester_id = semester_id });
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