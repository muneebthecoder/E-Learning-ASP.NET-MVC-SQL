using ELearning.Models;
using StuFeeWebApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ELearning.Controllers
{
    public class CourseController : Controller
    {
        DBAccess dbaobj = new DBAccess();
        // GET: Course
        public ActionResult CourseList()
        {
            ViewBag.HideHeader = true;
            List<Course> c = Course.CoursesList();

            return View(c);
        }

        [HttpGet]
        public ActionResult SearchCourses(string searchTerm)
        {
            List<Course> courses = new List<Course>();


            dbaobj.OpenCon();

            string query = "SELECT * FROM Course WHERE ('" + searchTerm + "' IS NULL OR c_title LIKE '%' + '" + searchTerm + "' + '%' OR c_keyword LIKE '%' + '" + searchTerm + "' + '%')";
            SqlCommand cmd = new SqlCommand(query, dbaobj.con);
            cmd.Parameters.AddWithValue(searchTerm, string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var course = new Course
                    {
                        c_title = reader["c_title"].ToString(),
                        c_category = reader["c_category"].ToString(),
                        c_description = reader["c_description"].ToString(),
                        c_image = reader["c_image"].ToString(),
                        c_hours = Convert.ToInt32(reader["c_hours"]),
                        c_rating = Convert.ToInt32(reader["c_rating"])
                    };
                    courses.Add(course);
                }
            }
            dbaobj.CloseCon();


            return PartialView("_CourseTable", courses);
        }

        public ActionResult AddCousre()
        {
            ViewBag.HideHeader = true;
            return View();

        }
        [HttpPost]
        public ActionResult AddCousre(Course c)
        {
            ViewBag.HideHeader = true;

            if (ModelState.IsValid)
            {
                string ext = Path.GetExtension(c.ufile.FileName);
                string[] extarr = { ".png", ".PNG", ".jpg", ".bmp" };
                if (extarr.Contains(ext))
                {
                    string ufilename = c.c_title + ext;
                    string path = Path.Combine(Server.MapPath("/images"), ufilename);
                    c.ufile.SaveAs(path);
                    c.c_image = "/images/" + ufilename;
                    string q = "insert into course values('" + c.c_title + "','" + c.c_category + "','" + c.c_description + "','" + c.c_image + "','" + c.c_hours + "','" + c.c_rating + "','" + c.c_keyword + "') ";
                    dbaobj.InsertUpdateDelete(q);
                    Response.Write("<script>alert('" + c.c_title + " Registered!');</script>");
                    return RedirectToAction("CourseList");
                }
            }

            return View(c);
        }

        public ActionResult AssignCourseToTeacher()
        {
            ViewBag.HideHeader = true;
            List<SelectListItem> slist = new List<SelectListItem>();
            string s = "select tid,t_name from teacher";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(s, dbaobj.con);
            SqlDataReader sdr = dbaobj.cmd.ExecuteReader();
            while (sdr.Read())
            {
                slist.Add(new SelectListItem { Text = "" + sdr["t_name"].ToString() + "", Value = "" + sdr["tid"].ToString() + "" });
            }
            sdr.Close();
            dbaobj.CloseCon();
            TempData["tlist"] = slist;

            List<SelectListItem> clist = new List<SelectListItem>();
            string c = "select cid,c_title from Course";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(c, dbaobj.con);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            while (sdr1.Read())
            {
                clist.Add(new SelectListItem { Text = "" + sdr1["c_title"].ToString() + "", Value = "" + sdr1["cid"].ToString() + "" });
            }
            sdr1.Close();
            dbaobj.CloseCon();
            TempData["clist"] = clist;


            return View();
        }
        [HttpPost]
        public ActionResult AssignCourseToTeacher(AllocateCourse a)
        {

            dbaobj.OpenCon();

            string q = "insert into Allocation values('" + a.TeacherId + "','" + a.CourseId + "') ";
            dbaobj.InsertUpdateDelete(q);

            dbaobj.CloseCon();
            return RedirectToAction("Dashboard", "Admin");
        }


        public ActionResult CourseDetails(int id)
        {
            ViewBag.HideHeader = true;
            Dictionary<int, bool> quizUploaded = new Dictionary<int, bool>();
            Dictionary<int, List<Tuple<string, string>>> pdfPaths = new Dictionary<int, List<Tuple<string, string>>>();
            Dictionary<int, List<Tuple<string, string>>> videoPaths = new Dictionary<int, List<Tuple<string, string>>>();
            Dictionary<int, bool> StudentResults = new Dictionary<int, bool>();


            dbaobj.OpenCon();

            for (int i = 1; i <= 16; i++)
            {
                // Initialize lists to store multiple PDFs and videos for each week
                List < Tuple<string, string> > pdfPathsForWeek = new List<Tuple<string, string>> ();
                List < Tuple<string, string> > videoPathsForWeek = new List<Tuple<string, string>>();

                // Query to get all PDFs for the current week
                string pdfQuery = "SELECT path,FileName FROM pdf WHERE cid = @cid AND weekNo = @weekNo";
                dbaobj.cmd = new SqlCommand(pdfQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", id);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", i);
                SqlDataReader pdfReader = dbaobj.cmd.ExecuteReader();

                // Read all PDF paths for the week
                while (pdfReader.Read())
                {
                //    pdfPathsForWeek.Add(pdfReader["path"].ToString());
                    pdfPathsForWeek.Add(Tuple.Create(pdfReader["path"].ToString(), pdfReader["FileName"].ToString()));
                }
                pdfReader.Close();

                // Store the list of PDF paths and the upload status for the current week
                pdfPaths[i] = pdfPathsForWeek;

                // Query to get all video links for the current week
                string videoQuery = "SELECT videoPath,FileName FROM videos WHERE cid = @cid AND weekNo = @weekNo";
                dbaobj.cmd = new SqlCommand(videoQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", id);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", i);
                SqlDataReader videoReader = dbaobj.cmd.ExecuteReader();

                // Read all video paths for the week
                while (videoReader.Read())
                {
              //      videoPathsForWeek.Add(videoReader["videoPath"].ToString());
                    videoPathsForWeek.Add(Tuple.Create(videoReader["videoPath"].ToString(), videoReader["FileName"].ToString()));
                }
                videoReader.Close();

                // Store the list of video paths for the current week
                videoPaths[i] = videoPathsForWeek;

                // Query to check if a quiz is uploaded for the current week
                string quizQuery = "SELECT COUNT(*) FROM Quiz WHERE cid = @cid AND weekNo = @weekNo";
                dbaobj.cmd = new SqlCommand(quizQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", id);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", i);
                int quizCount = (int)dbaobj.cmd.ExecuteScalar();

                // Store the quiz upload status for the current week
                quizUploaded[i] = quizCount > 0;

                // Query to check students result for the current week
                string sr = "SELECT COUNT(*) FROM result WHERE cid = @cid AND weekNo = @weekNo";
                dbaobj.cmd = new SqlCommand(sr, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", id);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", i);
                int cs = (int)dbaobj.cmd.ExecuteScalar();
                StudentResults[i] = cs > 0;
            }

            dbaobj.CloseCon();

            // Pass the dictionaries to the view via ViewBag
            ViewBag.QuizUploaded = quizUploaded;  // Add this to pass quiz upload status
            ViewBag.PdfPaths = pdfPaths;
            ViewBag.VideoPaths = videoPaths;
            ViewBag.StudentResults = StudentResults;

            Course c = new Course();
            string q = "select * from Course where cid=@cid";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            dbaobj.cmd.Parameters.AddWithValue("@cid", id);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            if (sdr1.Read())
            {
                c.cid = int.Parse(sdr1["cid"].ToString());
                c.c_title = sdr1["c_title"].ToString();
                c.c_category = sdr1["c_category"].ToString();
                c.c_description = sdr1["c_description"].ToString();
                c.c_image = sdr1["c_image"].ToString();
                c.c_hours = int.Parse(sdr1["c_hours"].ToString());
                c.c_rating = int.Parse(sdr1["c_rating"].ToString());
            }
            sdr1.Close();
            dbaobj.CloseCon();

            if (c == null)
            {
                return HttpNotFound();
            }

            PartialViewCourse ps = new PartialViewCourse
            {
                course = c
            };
            return View(ps);
        }



        [HttpPost]
        public ActionResult UploadPdf(int weekNo, HttpPostedFileBase pdfFile, int cid, string pdfName)
        {
            ViewBag.HideHeader = true;

            if (pdfFile != null && pdfFile.ContentLength > 0)
            {
                // Save the PDF to the server
                string path = Path.Combine(Server.MapPath("~/Uploads/"), pdfFile.FileName);
                pdfFile.SaveAs(path);


                dbaobj.OpenCon();

                string q = "   insert into pdf values(' ','" + ("/Uploads/" + pdfFile.FileName) + "','" + weekNo + "', '" + cid + "','"+pdfName+"','')";
                dbaobj.InsertUpdateDelete(q);

                dbaobj.CloseCon();



                // Save the path in the database against the week number (pseudo code)
                // SavePdfPathInDatabase(weekNo, path);

                TempData["Message"] = $"PDF for Week {weekNo} uploaded successfully!";
            }

            return RedirectToAction("CourseDetails", "Course", new { id = cid });
        }

        [HttpPost]
        public ActionResult UploadVideoLink(int weekNo, string VideoLink, int cid,string VideoName)
        {
            if (!string.IsNullOrEmpty(VideoLink))
            {
                dbaobj.OpenCon();

                string q = "   insert into videos values(' ','" + VideoLink + "','" + weekNo + "', '" + cid + "','"+VideoName+"','')";
                dbaobj.InsertUpdateDelete(q);
                dbaobj.CloseCon();

                TempData["Message"] = $"Video link for Week {weekNo} uploaded successfully!";
            }

            return RedirectToAction("CourseDetails", "Course", new { id = cid });
        }

        [HttpGet]
        public ActionResult UploadQuiz(int weekNo, int cid)
        {
            ViewBag.HideHeader = true;

            string q = "select * from Course where cid='" + cid + "'";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            if (sdr1.Read() == true)
            {
                ViewBag.ctitle = sdr1[1].ToString();

            }
            sdr1.Close();
            dbaobj.CloseCon();


            ViewBag.WeekNo = weekNo;
            ViewBag.CourseId = cid;
            return View(new QuizViewModel { WeekNo = weekNo, CourseId = cid });
        }

        [HttpPost]
        public ActionResult UploadQuiz(QuizViewModel model, int cid, int weekNo)
        {
            ViewBag.HideHeader = true;
            dbaobj.OpenCon();
            int a=  model.Questions[0].quizTime;
            for (int i = 0; i < model.Questions.Count; i++)
            {
                string q = "insert into quiz values(1,'" + weekNo + "','" + cid + "', '" + model.Questions[i].Question + "', '" + model.Questions[i].Option1 + "', '" + model.Questions[i].Option2 + "', '" + model.Questions[i].Option3 + "', '" + model.Questions[i].Option4 + "', '" + model.Questions[i].CorrectOption + "','" + model.Questions[0].quizTime + "')";
                dbaobj.InsertUpdateDelete(q);

            }
            dbaobj.CloseCon();

            // For now, just redirect to a success page or back to the course details
            return RedirectToAction("CourseDetails", "Course", new { id = cid });
        }

        public ActionResult ViewUploadedQuiz(int weekNo, int cid)
        {
            ViewBag.HideHeader = true;

            dbaobj.OpenCon();

            string query = "SELECT Question, Option1, Option2, Option3, Option4, CorrectOption , quizTime FROM Quiz WHERE cid = @cid AND weekNo = @weekNo";
            dbaobj.cmd = new SqlCommand(query, dbaobj.con);
            dbaobj.cmd.Parameters.AddWithValue("@cid", cid);
            dbaobj.cmd.Parameters.AddWithValue("@weekNo", weekNo);
            SqlDataReader reader = dbaobj.cmd.ExecuteReader();

            List<Quiz> quizData = new List<Quiz>();

            while (reader.Read())
            {
                quizData.Add(new Quiz
                {
                    Question = reader["Question"].ToString(),
                    Option1 = reader["Option1"].ToString(),
                    Option2 = reader["Option2"].ToString(),
                    Option3 = reader["Option3"].ToString(),
                    Option4 = reader["Option4"].ToString(),
                    CorrectOption = reader["CorrectOption"].ToString(),
                     quizTime = int.Parse(reader["quizTime"].ToString())
                });
            }

            reader.Close();
            dbaobj.CloseCon();

            string q = "select * from Course where cid='" + cid + "'";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            if (sdr1.Read() == true)
            {
                ViewBag.ctitle = sdr1[1].ToString();

            }
            sdr1.Close();
            dbaobj.CloseCon();


            ViewBag.WeekNo = weekNo;
            ViewBag.CourseId = cid;

            return View(quizData);
        }

        public ActionResult SeeStudentsResult(int cid,int weekNo)
        {
            List<Student> slist = new List<Student>();
            dbaobj.OpenCon();
            string q = "SELECT r.sid, s.s_name, r.obtainMarks, r.weekNo,r.cid FROM [ELearningDB1].[dbo].[Result] r JOIN [ELearningDB1].[dbo].[Student] s ON r.sid = s.sid WHERE   r.weekNo = '"+weekNo+"'   AND r.cid = '"+cid+"'";
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            SqlDataReader sdr = dbaobj.cmd.ExecuteReader();
            while (sdr.Read())
            {
                Student s = new Student();
                s.sid = int.Parse(sdr["sid"].ToString());
                s.sname = sdr["s_name"].ToString();
                s.obtainMarks =int.Parse( sdr["obtainMarks"].ToString());
                slist.Add(s);
            }
            sdr.Close();

            string qq = "select * from Course where cid='" + cid + "'";
            dbaobj.cmd = new SqlCommand(qq, dbaobj.con);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            if (sdr1.Read() == true)
            {
                ViewBag.ctitle = sdr1[1].ToString();

            }
            sdr1.Close();

            string qqq = "select Count(*) from quiz where cid='" + cid + "' and weekNo='"+weekNo+"'";
            dbaobj.cmd = new SqlCommand(qqq, dbaobj.con);
            ViewBag.totalquestions = (int)dbaobj.cmd.ExecuteScalar();

            dbaobj.CloseCon();


            ViewBag.WeekNo = weekNo;

            return View(slist);
        }


        public ActionResult ViewEnrolledStudents(int cid)
        {
            List<Student> slist = new List<Student>();
            dbaobj.OpenCon();
            string q = "SELECT S.sid, S.s_name, S.s_email, S.s_interest, S.image\r\nFROM Enroll E\r\nINNER JOIN Student S ON E.sid = S.sid\r\nWHERE E.cid = '"+cid+"'";
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            SqlDataReader sdr = dbaobj.cmd.ExecuteReader();
            while (sdr.Read())
            {
                Student s = new Student();
                s.sid = int.Parse(sdr[0].ToString());
                s.sname = sdr[1].ToString();
                s.semail = sdr[2].ToString();
                s.sInterset = sdr[3].ToString();

                slist.Add(s);
            }
            sdr.Close();

            foreach (var item in slist)
            {

                string sr = "SELECT grade FROM grade WHERE cid = '" + cid + "' AND sid = '" + item.sid + "'";
                dbaobj.cmd = new SqlCommand(sr, dbaobj.con);
                   var grade = dbaobj.cmd.ExecuteScalar();
                if (grade != null)
                {
                    item.grade = grade.ToString();
                }
                else 
                {
                    item.grade = "Not Completed";
                }
            }
            dbaobj.CloseCon();
            ViewBag.cid = cid;
            return View(slist);
        }

        public ActionResult SeeStudentResult(int cid,int sid)
        {
            Dictionary<int, int> totalQuestions = new Dictionary<int, int>();
            Dictionary<int, int> obtainMarkss = new Dictionary<int, int>();
            int totalMarks = 0;
            int obtainedMarks = 0;
            dbaobj.OpenCon();
            for (int i = 1; i <= 16; i++)
            {
                // Query to check if a video and pdf is read for the current week
                string pvq = " SELECT COUNT(*) FROM quiz WHERE cid ='" + cid + "' AND weekNo = '" + i + "'";
                dbaobj.cmd = new SqlCommand(pvq, dbaobj.con);
                int cc = (int)dbaobj.cmd.ExecuteScalar();
                totalMarks += cc;
                totalQuestions[i] = cc;

                // Query to get all quiz result for the current week
                string q_mQuery = "SELECT obtainMarks FROM result WHERE cid = @cid AND weekNo = @weekNo And sid=@sid";
                dbaobj.cmd = new SqlCommand(q_mQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", cid);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", i);
                dbaobj.cmd.Parameters.AddWithValue("@sid", sid);
                SqlDataReader sdr = dbaobj.cmd.ExecuteReader();

                if (sdr.Read())
                {
                    obtainedMarks += int.Parse(sdr["obtainMarks"].ToString());
                    obtainMarkss[i] = int.Parse(sdr["obtainMarks"].ToString());
                }
                else { obtainMarkss[i] = 0; }
                sdr.Close();
            }


            string qq = "SELECT * FROM Course WHERE cid='" + cid + "'";
            dbaobj.cmd = new SqlCommand(qq, dbaobj.con);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            if (sdr1.Read())
            {
                ViewBag.CourseTitle = sdr1["c_title"].ToString();
            }
            sdr1.Close();

            string qqq = "SELECT * FROM student WHERE sid='" + sid + "'";
            dbaobj.cmd = new SqlCommand(qqq, dbaobj.con);
            SqlDataReader sdr12 = dbaobj.cmd.ExecuteReader();
            if (sdr12.Read())
            {
                ViewBag.Name = sdr12["s_name"].ToString();
            }
            sdr12.Close();
            // Calculate percentage and grade
            double percentage = (obtainedMarks / (double)totalMarks) * 100;
            string grade = GetGrade(percentage);


            dbaobj.CloseCon();
            // Pass data to the view
            ViewBag.ObtainMarks = obtainMarkss;
            ViewBag.Total = totalQuestions;
            ViewBag.TotalMarks = totalMarks;
            ViewBag.ObtainedMarks = obtainedMarks;
            ViewBag.Percentage = percentage;
            ViewBag.Grade = grade;

            return View();
        }

        // Example method to calculate grade
        private string GetGrade(double percentage)
        {
            if (percentage >= 85) return "A";
            else if (percentage >= 70) return "B";
            else if (percentage >= 50) return "C";
            else if (percentage >= 33) return "D";
            else return "F";
        }
    }
}