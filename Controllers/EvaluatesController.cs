using System;
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
        public ActionResult Index(int group_id, int semester_id, int? action)
        {
            var evaluates = db.Evaluates.Where(e => e.Group_ID == group_id && e.Semester_ID == semester_id && e.Status != "Deleted");
            TempData["action"] = action;
            return View(evaluates.ToList());
        }

        // GET: Evaluates/Details/5
        public ActionResult Details(int? id, int? notification_id)
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
            if (notification_id != null)
            {
                Notification notification = db.Notifications.Find(notification_id);
                notification.status = "Seen";
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
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
                mailmea.Body = "<div><div ><p style="+"font-weight:normal;text-align:justify;margin-top:0;margin-bottom:0;line-height:1.8;"+"><span style="+"color: rgb(24, 26, 28); font-size: 20pt; font-family: Times New Roman , serif, EmojiFont; font-weight: bold; "+">Review :</span></p><br aria-hidden="+true+"><p style="+"font-weight:normal;text-align:justify;margin-top:0;margin-bottom:0;line-height:1.8;"+"><span style="+"color: rgb(197, 43, 16); font-size: 10 pt; font-family: Times New Roman , serif, EmojiFont; font-weight: bold; "+">Point : '+point+'</span></p><br aria-hidden="+true+"><p><span style="+"color: rgb(12, 100, 192); font-size: 10 pt; font-family: Times New Roman, serif, EmojiFont; font-weight: 400; "+">"
                    + review+ 
                    "</span></p></div><b></b><div ><table><tbody><tr><td><img data-imagetype="+"External"+" src="+"https://cosmicimg-prod.services.web.outlook.com/proxy/?u=http%3A%2F%2Fisc.vanlanguni.edu.vn%2Fsig%2Flogo-vlu-sig.png&amp;t=eyJhbGciOiJSUzI1NiIsImtpZCI6IlY2YmZseXM0bTBHU2hlSjMzRHo0U1JfT3htMCIsInR5cCI6IkpXVCIsIng1dCI6IlY2YmZseXM0bTBHU2hlSjMzRHo0U1JfT3htMCIsImlzc2xvYyI6IkhLMFBSMDFNQjI5OTMiLCJzcWlkIjo2Mzc5NTA0MTY0NjEzNjg5NDF9.eyJpYXQiOjE2NTk2MDA1MTAsInZlciI6IlNUSS5Vc2VyLlYxIiwic2FwLXZlcnNpb24iOiI1IiwiYXBwaWQiOiJmMjA0MTUwNC0yZjgxLTQ5M2QtYWY4ZC1iNGIzNTBjZWZlNTciLCJpc3NyaW5nIjoiV1ciLCJhcHBpZGFjciI6IjIiLCJ0aWQiOiIzMDExYTU0YjBhNWQ0OTI5YmYwMmEwMDc4Nzg3N2M2YSIsInNjcCI6IkltYWdlUHJveHktSW50ZXJuYWwuUmVuZGVyIiwidG9wb2xvZ3kiOiJ7XCJUeXBlXCI6XCJNYWNoaW5lXCIsXCJWYWx1ZVwiOlwiSEswUFIwMU1CMjk5My5hcGNwcmQwMS5wcm9kLmV4Y2hhbmdlbGFicy5jb21cIn0iLCJwdWlkIjoiMTAwMzIwMDA1NzRFMzMwQSIsImVwa2giOiJENGVHM3BZVTl1WXc5M0VkIiwibmJmIjoxNjU5NjAwNTEwLCJleHAiOjE2NTk2MDE0MTAsImlzcyI6Imh0dHBzOi8vc3Vic3RyYXRlLm9mZmljZS5jb20vc3RzLyIsImF1ZCI6Imh0dHBzOi8vb3V0bG9va2ltYWdlcHJveHkuYXp1cmV3ZWJzaXRlcy5uZXQiLCJwa3IiOiJQTE9rdm50Y3FONUU0SVM4cWFZZzdMS1ZJeUE9Iiwic3NlYyI6IkM1Yy95VkovQkNUY2tlUjcifQ.6ICe8hKpr8LH8DMb8ug7eYNDf5N7_MTjGFUvHGFH7gINgO5ruFLAyP_rq2RE-JvV5FW4AX0Nl5rJM0jbQTJhbLeNs2Y5FaH4ad5Ff8qhoxDFKy9jT0D0CsrVADOkfw_aK4q5pICQWF0yDo1Jy1dUGy0Z0SY8sUqisdNSH3Qf_odW7bBzZBcVFzoV7QtCUZD9LHPyNumyrGulkf7v3LR5_yLvfEsDC1kn0Y7wBYvhBwn64CqJx4QKaG2zEqJTCRluYdmXqHIABLFX46wZokVry904-gwBLFQy0_b7Vy0i3v20U2Z9JbMxcD7iM7PPwyXtl7sfSFRloiwY7mYPdstJog&amp;r=p&amp;s=b"+" originalsrc="+"http://isc.vanlanguni.edu.vn/sig/logo-vlu-sig.png"+" data-imageproxyendpoint="+"/actions/ei"+" alt="+"http://isc.vanlanguni.edu.vn/sig/logo-vlu-sig.png"+" width="+110+" height="+100+" style="+"display:inline-block; vertical-align:bottom; margin-left:5px; margin-right:5px"+"></td><td style="+"padding:8px 12px; color: rgb(24, 26, 28); font-size: 12pt; font-family: Times New Roman , serif, EmojiFont; font-weight: bold"+">Join on website Meeting VL :<a href="+"https://cntttest.vanlanguni.edu.vn:18081/SEP25Team13/"+" target="+"_blank"+" rel="+"noopener noreferrer"+" data-auth="+"NotApplicable"+" id="+"LPlnk238218" +"data-safelink="+true+" data-linkindex="+0+" >View Review </a></td></tr></tbody></table></div></div>";

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
        public ActionResult Edit(int? id, string review, int point)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluate evaluate = db.Evaluates.Find(id);
            evaluate.Review = review;
            evaluate.Point = point;
            db.Entry(evaluate).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", new {id = id });
          
        }

       

        // GET: Evaluates/Delete/5
        public ActionResult Delete(int? id, int semester_id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evaluate evaluate = db.Evaluates.Find(id);
            evaluate.Status = "Deleted";
            db.Entry(evaluate).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Session_Semester", new { semester_id = semester_id });
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
