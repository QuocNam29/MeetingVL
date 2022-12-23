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
            evaluate.Review = review.Trim();
            evaluate.Point = point;
            evaluate.Semester_ID = semester_id;
            evaluate.Time = DateTime.Now;
             db.Evaluates.Add(evaluate);
            db.SaveChanges();

            Semester semester = db.Semesters.Find(semester_id);
            Group group = db.Groups.Find(group_id);
            var list_member_notification = db.ProjectParticipants.Where(p => p.Project_ID == project_id 
            && p.Group_ID == group_id && p.User_ID != null && p.Role =="Student").ToArray();
            
            //Khởi tạo nội dung gửi mail
            MailMessage mm = new MailMessage();
            mm.From = new MailAddress("meetingvl2022@gmail.com", "Meeting VL");         
            for (int i = 0; i < list_member_notification.Length; i++)
            {
                mm.To.Add((list_member_notification[i].User_ID));
                Notification notification = new Notification();
                notification.User_ID = list_member_notification[i].User_ID;
                notification.Time = DateTime.Now;
                notification.Evalute_ID = evaluate.ID;
                notification.Content = evaluate.Review;
                notification.status = "New";
                db.Notifications.Add(notification);
                db.SaveChanges();
            }              
                mm.IsBodyHtml = true;
                mm.Body = @"<div>
    <p style=""font-weight:normal;text-align:justify;margin-top:0;margin-bottom:0;line-height:1.8;"">
        <span style=""color: rgb(24, 26, 28); font-size: 20pt; font-family: Times New Roman , serif, EmojiFont; font-weight: bold; "">Kết quả đánh giá biên bản họp kì: " + semester.Name + @" </span>
    </p>
    <br aria-hidden=""true"">

    <p style=""font-weight:normal;text-align:justify;margin-top:0;margin-bottom:0;line-height:1.8;"">
<span style="" font-size:13pt; font-family: Times New Roman , serif, EmojiFont "">Nhóm :<b> 
" + group.Name + @"</b> <br/><br/></span>
        <span style="" font-size:13pt; font-family: Times New Roman , serif, EmojiFont "">Điểm (thang điểm 10) :<b> 
" + point + @"</b></span>
    </p>
    <br aria-hidden=""true"">
    <div class=""x_copy-paste-block"" itemprop=""copy-paste-block"">
        <p>
            <span style=""font-size:13pt"">
                <span style=""font-family: Times New Roman,serif"">
                    <span style=""font-size:13pt"">
                        <span style=""font-family: &quot,Times New Roman,serif"">Nhận xét: </span>
                    </span>
                    <span style=""font-family: &quot; Times New Romant;,serif"">
                    </span>
                </span>
            </span>
        </p>
      
                <span style=""font-size:13pt"">
                    <span>
                        <span style=""font-family: Times New Roman,serif"">
                            <span style=""font-size:13pt; white-space: pre-wrap;overflow-wrap: break-word;"">
                                <span style=""font-family: &quot,Times New Roman,serif; "">
   <b> " + review +
                    @"</b></span>
                            </span>
                            <span style=""font-family: &quot,Times New Roman,serif""></span>
                        </span>
                    </span>
                </span>
           <p style=""margin-bottom:13px""><span style=""font-size:13pt""><span style=""font-family: Times New Roman,serif""><span lang=""VI"" style=""font-family: &quot,Times New Roman,serif""></span></span></span></p>
    </div>
   
        <hr />
    
    <div>
        <table>
            <tbody>
                <tr>
                    <td style=""padding:8px 12px; color: rgb(24, 26, 28); font-size: 12pt; font-family: Times New Roman , serif, EmojiFont; font-weight: bold"">
                        Truy cập vào website Meeting VL :
                        <a  target=""_blank"" rel=""noopener noreferrer"" data-auth=""NotApplicable"" id=""LPlnk238218"" data-safelink=""true"" data-linkindex=""0"">
                            xem đánh giá
                        </a>

                    </td>
                </tr>
            </tbody>
        </table>
    </div>  
        <hr />
    <br />
    <span style=""font-size:10pt"">
        <span>
            <span style=""font-family: Times New Roman,serif"">
                <i><span style=""font-family:quot,Times New Roman,serif"">Đây là email tự động, Sinh viên vui lòng không phản hồi email này.</span></i></b><b><i><span style=""font-family: &quot,Times New Roman,serif""></span></i>
            </span>
        </span>
    </span>
</div>";
            
           
            mm.Subject = "Đánh giá báo cáo biên bản họp:   " + semester.Name;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("meetingvl2022@gmail.com", "nbuwikbmthtroyst");

                     
            try
                {
                    smtp.Send(mm);
                }
                catch (Exception ex)
                {
                    Session["thongbao-loi"] = ex.Message;
                }
            
            Session["notification"] = "Successfully Create Evalute";

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
