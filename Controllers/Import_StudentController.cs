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
                    int missData = 0;
                    try
                        {
                    //Insert records to database table.
                    foreach (DataRow row in dt.Rows)
                    {
                                if (!String.IsNullOrEmpty(row["TOPIC"].ToString().Trim())
                               && !String.IsNullOrEmpty(row["TEAM"].ToString().Trim())
                               && !String.IsNullOrEmpty(row["MENTOR"].ToString().Trim()))
                                {
                                    Session["Topic"] = row["TOPIC"].ToString().Trim();
                                    Session["Mentor"] = row["MENTOR"].ToString().Trim();
                                    Session["NameGroup"] = row["TEAM"].ToString().Trim();
                                    Session["Customer"] = row["CUSTOMER"].ToString().Trim();
                            }
                            else if (String.IsNullOrEmpty(row["TOPIC"].ToString().Trim())
                               && String.IsNullOrEmpty(row["TEAM"].ToString().Trim())
                               && String.IsNullOrEmpty(row["MENTOR"].ToString().Trim()))
                            {

                            }
                            else
                            {
                                Session["Topic"] = null;
                            }                     
                            
                           
                        if (!String.IsNullOrEmpty(row["EMAIL"].ToString().Trim()))
                        {
                            string user_id = row["EMAIL"].ToString().Trim();
                            bool check_format_student = user_id.Contains("@vanlanguni.vn");
                            bool check_format_teacher = user_id.Contains("@vlu.edu.vn");
                                if (Session["Topic"] != null)
                                {
                                    if (check_format_student == true || check_format_teacher == true)
                                    {
                                        var check_user = db.ProjectParticipants.Where(c => c.Project_ID == project_id && c.User_ID == user_id).FirstOrDefault();
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
                                            string topic = Session["Topic"].ToString().Trim();
                                            string mentor = Session["Mentor"].ToString().Trim();
                                            string customer = Session["CUSTOMER"].ToString().Trim();
                                            string nameGroup = "Team " + Session["NameGroup"].ToString().Trim();

                                            var group = db.Groups.Where(g => g.Topic == topic && g.Mentor == mentor
                                            && g.Name == nameGroup && g.Customer == customer).FirstOrDefault();
                                            if (group == null)
                                            {
                                                Group addGroup = new Group();
                                                addGroup.Name = "Team " + row["TEAM"].ToString().Trim();
                                                addGroup.Topic = row["TOPIC"].ToString().Trim();
                                                addGroup.Mentor = row["MENTOR"].ToString().Trim();
                                                addGroup.Customer = row["CUSTOMER"].ToString().Trim();
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
                                else
                                {
                                    missData++;
                                }
                           

                        }
  
                    }
                        Session["ViewBag.FileStatus"] = null;
                        Session["ViewBag.Success"] = addRow;
                        Session["ViewBag.FailFormat"] = rowFailFormat;
                        Session["ViewBag.Exist"] = rowExist;
                        Session["ViewBag.MissData"] = missData;
                    }
                    catch (Exception e)
                    {
                        string bug = e.ToString();
                        bool check_bug = bug.Contains("does not belong to table");
                        bool check_bug1 = bug.Contains("Object reference not set to an instance of an object");
                        if (check_bug == true)
                        {
                            Session["ViewBag.Success"] = null;
                            Session["ViewBag.FileStatus"] = "Table structure or table name in excel file is not in the correct format!";
                        }
                        else if (check_bug1 == true)
                        {
                            Session["ViewBag.Success"] = null;
                            Session["ViewBag.FileStatus"] = "You missed the data in the first line in the excel file!";
                        }
                        else
                        {
                            Session["ViewBag.Success"] = null;
                            Session["ViewBag.FileStatus"] = e.ToString();
                        }


                     

                    }
                    Session["Topic"] = null;
                    Session["Mentor"] = null;
                    Session["NameGroup"] = null;
                    Session["Customer"] = null;

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



        public FileResult DownLoad()
        {
            string path = Server.MapPath("~/Content/Files");
            string filename = Path.GetFileName("FileFormat.xlsx");

            string fullPath = Path.Combine(path, filename);
            return File(fullPath, "download/xlsx", "FileFormat.xlsx");
        }

        
    }
}