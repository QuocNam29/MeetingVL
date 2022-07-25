using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MeetingVL.Models;

namespace MeetingVL.Controllers
{
    public class Import_StudentController : Controller
    {
        private SEP25Team13Entities db = new SEP25Team13Entities();
        // GET: Import_Student
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase postedFile, int project_id)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);

                if (extension.ToLower() == ".xls" || extension.ToLower() == ".xlsx")
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    postedFile.SaveAs(filePath);

                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                    }

                    DataTable dt = new DataTable();
                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;

                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                                //Get the name of First Sheet.
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();

                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();

                            }
                        }
                    } 
                    int addRow = 0;
                    int rowFailFormat = 0;
                    int rowExist = 0;
                    //Insert records to database table.
                    foreach (DataRow row in dt.Rows)
                    {
                       if (!String.IsNullOrEmpty(row["TOPIC"].ToString()))
                                {
                                    Session["Topic"] = row["TOPIC"].ToString();
                                    Session["Mentor"] = row["MENTOR"].ToString();
                                    Session["NameGroup"] = row["TEAM"].ToString();
                                    Session["Customer"] = row["CUSTOMER"].ToString();
                                }
                        if (!String.IsNullOrEmpty(row["EMAIL"].ToString()))
                        {
                            string user_id = row["EMAIL"].ToString();
                            bool check_format_student = user_id.Contains("@vanlanguni.vn");
                            bool check_format_teacher = user_id.Contains("@vlu.edu.vn");

                            if (check_format_student == true || check_format_teacher == true)
                            {
                            var check_user = db.ProjectParticipants.Where(c => c.Project_ID == project_id && c.User_ID == user_id ).FirstOrDefault();
                            if (check_user == null)
                            {
                                var user = db.Users.Find(user_id);
                                if (user == null)
                                {
                                    User addUser = new User();
                                    addUser.Email = user_id;
                                    db.Users.Add(addUser);
                                    db.SaveChanges();
                                }                               
                                string topic = Session["Topic"].ToString();
                                string mentor = Session["Mentor"].ToString();
                                string customer = Session["CUSTOMER"].ToString();
                                string nameGroup = "Team " + Session["NameGroup"].ToString();

                                var group = db.Groups.Where(g => g.Topic == topic && g.Mentor == mentor 
                                && g.Name == nameGroup && g.Customer == customer).FirstOrDefault();
                                if (group == null)
                                {
                                    Group addGroup = new Group();
                                    addGroup.Name = "Team " + row["TEAM"].ToString();
                                    addGroup.Topic = row["TOPIC"].ToString();
                                    addGroup.Mentor = row["MENTOR"].ToString();
                                    addGroup.Customer = row["CUSTOMER"].ToString();
                                    db.Groups.Add(addGroup);
                                    db.SaveChanges();

                                    var user1 = db.Users.Find(user_id);

                                    ProjectParticipant projectParticipant = new ProjectParticipant();
                                    projectParticipant.User_ID = user1.Email;
                                    projectParticipant.Group_ID = addGroup.ID;
                                    projectParticipant.Project_ID = project_id;
                                    projectParticipant.Role = "Student";
                                    db.ProjectParticipants.Add(projectParticipant);
                                    db.SaveChanges();
                                   
                                }
                                else
                                {
                                    var user1 = db.Users.Find(user_id);

                                    ProjectParticipant projectParticipant = new ProjectParticipant();
                                    projectParticipant.User_ID = user1.Email;
                                    projectParticipant.Group_ID = group.ID;
                                    projectParticipant.Project_ID = project_id;
                                    projectParticipant.Role = "Student";
                                    db.ProjectParticipants.Add(projectParticipant);
                                    db.SaveChanges();
                                    
                                }
                                addRow++;
                            }
                                else
                                {
                                    rowExist++;
                                }
                            }
                            else
                            {
                                rowFailFormat++;
                            }
                                                    
                        }
                    }
                    Session["ViewBag.FileStatus"] = null;
                    Session["ViewBag.Success"] = addRow;
                    Session["ViewBag.FailFormat"] = rowFailFormat;
                    Session["ViewBag.Exist"] =  rowExist;
                }
                else
                {
                    Session["ViewBag.Success"] = null;
                    Session["ViewBag.FileStatus"] = "Invalid file format !";
                }

            }
            else
            {
                Session["ViewBag.Success"] = null;
                Session["ViewBag.FileStatus"] = "You have not selected a file to import!";
            }
            
            return RedirectToAction("Index", "Session_Reports", new { project_id  = project_id, active = 2 });
        }
    }
}