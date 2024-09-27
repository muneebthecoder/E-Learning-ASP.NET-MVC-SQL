using ELearning.Models;
using StuFeeWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace ELearning.Controllers
{
    public class StudentController : Controller
    {
        DBAccess dbaobj = new DBAccess();

        // GET: Student
        public ActionResult DashBoard()
        {
            ViewBag.HideHeader = true;
            return View();

        }
        public ActionResult StudentList()
        {
            ViewBag.HideHeader = true;
            List<Student> s = Student.StudentList();
            return View(s);

        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Login", "LoginSignup");
        }

        public ActionResult EnrollCourse()
        {
            ViewBag.HideHeader = true;
            List<Course> clist = new List<Course>();
            dbaobj.OpenCon();
            string q =
                "SELECT c.cid, c.c_title, c.c_category, c.c_description, c.c_image,c.c_hours,c.c_rating, c.c_keyword,t.tid,t.t_name,t.t_email,t.t_qualification FROM   Course c LEFT JOIN Enroll e ON c.cid = e.cid AND e.sid = '" + Session["sid"] + "' LEFT JOIN Allocation a ON c.cid = a.cid LEFT JOIN  teacher t ON a.tid = t.tid WHERE e.sid IS NULL; ";
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            SqlDataReader sdr = dbaobj.cmd.ExecuteReader();
            while (sdr.Read())
            {
                Course c = new Course();
                c.cid = int.Parse(sdr["cid"].ToString());
                c.c_title = sdr["c_title"].ToString();
                c.c_category = sdr["c_category"].ToString();
                c.c_description = sdr["c_description"].ToString();
                c.c_image = sdr["c_image"].ToString();
                c.c_hours = int.Parse(sdr["c_hours"].ToString());
                c.c_rating = double.Parse(sdr["c_rating"].ToString());
                c.c_keyword = sdr["c_keyword"].ToString();
                c.teacher_id = int.Parse(sdr[8].ToString());
                c.teacher_name = sdr[9].ToString();

                clist.Add(c);
            }
            sdr.Close();
            dbaobj.CloseCon();

            return View(clist);
        }
        public ActionResult EnrolledCourse(int cid, int teacherId, string title)
        {
            dbaobj.OpenCon();

            string q = "insert into enroll values('" + Session["sid"] + "','" + teacherId + "','" + cid + "','Enrolled') ";
            dbaobj.InsertUpdateDelete(q);

            dbaobj.CloseCon();
            Response.Write("<script>alert('Course : " + title + " has been Enrolled!');</script>");


            return RedirectToAction("Dashboard");
        }
        public ActionResult ViewEnrolledCourse()
        {
            List<Course> clist = new List<Course>();
            dbaobj.OpenCon();
            string q = "SELECT c.cid,c.c_title,c.c_category,c.c_description,c.c_image,c.c_hours, c.c_rating, c.c_keyword,t.tid, t.t_name, t.t_email, t.t_qualification FROM Course c INNER JOIN Enroll e ON c.cid = e.cid INNER JOIN  Allocation a ON c.cid = a.cid INNER JOIN teacher t ON a.tid = t.tid WHERE e.sid = '" + Session["sid"] + "'";
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            SqlDataReader sdr = dbaobj.cmd.ExecuteReader();
            while (sdr.Read())
            {
                Course c = new Course();
                c.cid = int.Parse(sdr["cid"].ToString());
                c.c_title = sdr["c_title"].ToString();
                c.c_category = sdr["c_category"].ToString();
                c.c_description = sdr["c_description"].ToString();
                c.c_image = sdr["c_image"].ToString();
                c.c_hours = int.Parse(sdr["c_hours"].ToString());
                c.c_rating = double.Parse(sdr["c_rating"].ToString());
                c.c_keyword = sdr["c_keyword"].ToString();
                c.teacher_id = int.Parse(sdr[8].ToString());
                c.teacher_name = sdr[9].ToString();

                clist.Add(c);
            }
            sdr.Close();

            foreach (var item in clist) {
                int total = 0;

                for (int i = 0; i < 16; i++)
                {
                    // For get progress
                    string qq = "SELECT COUNT(*) FROM result WHERE cid ='" + item.cid + "' AND weekNo = '" + (i + 1) + "' and sid='" + Session["sid"] + "'";
                    dbaobj.cmd = new SqlCommand(qq, dbaobj.con);
                    int p = (int)dbaobj.cmd.ExecuteScalar();
                    total += p;
                }
                item.progress = ((double)total / 16) * 100;
            }
            dbaobj.CloseCon();

            return View(clist);
        }

        [HttpGet]
        public ActionResult SearchCourses(string searchTerm)
        {
            List<Course> courses = new List<Course>();
            dbaobj.OpenCon();
            string query = "SELECT c.cid, c.c_title, c.c_category, c.c_description, c.c_image, c.c_hours, c.c_rating, c.c_keyword, t.tid, t.t_name, t.t_email, t.t_qualification FROM Course c LEFT JOIN Enroll e ON c.cid = e.cid AND e.sid = '" + Session["sid"] + "' LEFT JOIN Allocation a ON c.cid = a.cid LEFT JOIN teacher t ON a.tid = t.tid WHERE e.sid IS NULL AND ('" + searchTerm + "' IS NULL OR c.c_title LIKE '%' + '" + searchTerm + "' + '%' OR c.c_keyword LIKE '%' + '" + searchTerm + "' + '%' OR t.t_name LIKE '%' + '" + searchTerm + "' + '%')";
            SqlCommand cmd = new SqlCommand(query, dbaobj.con);
            cmd.Parameters.AddWithValue(searchTerm, string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);

            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    var course = new Course
                    {
                        cid = int.Parse(sdr["cid"].ToString()),
                        c_title = sdr["c_title"].ToString(),
                        c_category = sdr["c_category"].ToString(),
                        c_description = sdr["c_description"].ToString(),
                        c_image = sdr["c_image"].ToString(),
                        c_hours = int.Parse(sdr["c_hours"].ToString()),
                        c_rating = double.Parse(sdr["c_rating"].ToString()),
                        c_keyword = sdr["c_keyword"].ToString(),
                        teacher_id = int.Parse(sdr[8].ToString()),
                        teacher_name = sdr[9].ToString(),
                    };
                    courses.Add(course);
                }
            }
            dbaobj.CloseCon();


            return PartialView("_EnrollCourses", courses);
        }

        public ActionResult CourseDetailsStudent(int id)
        {



            var studentId = Session["sid"];
            List<int> favoriteWeeks = new List<int>();

            dbaobj.OpenCon();

            // Retrieve all favorite weeks for this course and student
            string getFavoriteWeeksQuery = "SELECT weekno FROM favouriteweek WHERE cid = @Cid AND sid = @StudentId";
            dbaobj.cmd = new SqlCommand(getFavoriteWeeksQuery, dbaobj.con);

            dbaobj.cmd.Parameters.AddWithValue("@Cid", id);
            dbaobj.cmd.Parameters.AddWithValue("@StudentId", studentId);

            using (var reader = dbaobj.cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    favoriteWeeks.Add(reader.GetInt32(0));  // Assuming weekno is the first column
                }
            }



            ViewBag.FavoriteWeeks = favoriteWeeks;

            ViewBag.HideHeader = true;

            Dictionary<int, bool> quizUploaded = new Dictionary<int, bool>();
            Dictionary<int, List<Tuple<string, string>>> pdfPaths = new Dictionary<int, List<Tuple<string, string>>>();
            Dictionary<int, List<Tuple<string, string>>> videoPaths = new Dictionary<int, List<Tuple<string, string>>>();
            Dictionary<int, List<string>> quizAttemptedAndMarks = new Dictionary<int, List<string>>();
            Dictionary<int, int> totalQuestions = new Dictionary<int, int>();
            Dictionary<int, bool> readPdfvideo = new Dictionary<int, bool>();





            for (int i = 1; i <= 16; i++)
            {
                // Initialize lists to store multiple PDFs and videos for each week
                List<Tuple<string, string>> pdfPathsForWeek = new List<Tuple<string, string>>();
                List<Tuple<string, string>> videoPathsForWeek = new List<Tuple<string, string>>();
                List<string> quizAttemptedAndMarksWeek = new List<string>();

                // Query to get all PDFs for the current week
                string pdfQuery = "SELECT path,FileName FROM pdf WHERE cid = @cid AND weekNo = @weekNo";
                dbaobj.cmd = new SqlCommand(pdfQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", id);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", i);
                SqlDataReader pdfReader = dbaobj.cmd.ExecuteReader();


                // Read all PDF paths for the week
                while (pdfReader.Read())
                {
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
                totalQuestions[i] = quizCount;

                // Query to check if a video and pdf is read for the current week
                string pvq = " SELECT COUNT(*) FROM ReadContent WHERE cid ='" + id + "' AND weekNo = '" + i + "' and sid='" + Session["sid"] + "' and pdfRead=1 and videoRead=1";
                dbaobj.cmd = new SqlCommand(pvq, dbaobj.con);
                int cc = (int)dbaobj.cmd.ExecuteScalar();
                readPdfvideo[i] = cc > 0;


                // Query to get all quiz result for the current week
                string q_mQuery = "SELECT obtainMarks FROM result WHERE cid = @cid AND weekNo = @weekNo And sid=@sid";
                dbaobj.cmd = new SqlCommand(q_mQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", id);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", i);
                dbaobj.cmd.Parameters.AddWithValue("@sid", Session["sid"]);
                SqlDataReader sdr = dbaobj.cmd.ExecuteReader();

                while (sdr.Read())
                {
                    quizAttemptedAndMarksWeek.Add(sdr["obtainMarks"].ToString());
                }
                sdr.Close();
                quizAttemptedAndMarks[i] = quizAttemptedAndMarksWeek;

            }

            dbaobj.CloseCon();

            // Pass the dictionaries to the view via ViewBag
            ViewBag.QuizUploaded = quizUploaded;
            ViewBag.TotalQuestions = totalQuestions;
            ViewBag.PdfPaths = pdfPaths;
            ViewBag.VideoPaths = videoPaths;
            ViewBag.QuizAttemptedAndMarks = quizAttemptedAndMarks;
            ViewBag.ReadPdfVideo = readPdfvideo;
            Session["cid"] = id;

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

        public ActionResult AttemptQuiz(int cid, int weekNo)
        {

            string qq = "select * from Course where cid='" + cid + "'";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(qq, dbaobj.con);
            SqlDataReader sdr = dbaobj.cmd.ExecuteReader();
            if (sdr.Read() == true)
            {
                ViewBag.ctitle = sdr[1].ToString();

            }
            sdr.Close();
            dbaobj.CloseCon();

            QuizViewModel quiz = new QuizViewModel();
            string q = "select * from quiz where cid='" + cid + "' and weekNo='" + weekNo + "'";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            while (sdr1.Read())
            {
                Quiz qu = new Quiz();
                quiz.CourseId = int.Parse(sdr1["cid"].ToString());
                quiz.WeekNo = int.Parse(sdr1["weekNo"].ToString());
                qu.Id = int.Parse(sdr1["id"].ToString());
                //    qu.QuestionId= int.Parse(sdr1["questionId"].ToString());
                qu.quizTime = int.Parse(sdr1["quizTime"].ToString());
                qu.Question = sdr1["question"].ToString();
                qu.Option1 = sdr1["option1"].ToString();
                qu.Option2 = sdr1["option2"].ToString();
                qu.Option3 = sdr1["option3"].ToString();
                qu.Option4 = sdr1["option4"].ToString();
                qu.CorrectOption = sdr1["correctOption"].ToString();

                quiz.Questions.Add(qu);
            }
            sdr1.Close();
            dbaobj.CloseCon();
            if (quiz == null)
            {
                return HttpNotFound("Quiz not found.");
            }

            return View(quiz);
        }
        [HttpPost]
        public ActionResult SubmitQuiz(int cid, int weekNo, QuizViewModel model, string elapsedTime)
        {
            if (model == null)
            {
                // Handle the case where no answers are provided
                return RedirectToAction("CourseDetailsStudent", new { id = cid });
            }

            int totalMarks = 0; // To calculate total marks

            // Query to fetch the quiz questions and correct answers
            string q = "SELECT * FROM quiz WHERE cid = @cid AND weekNo = @weekNo";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            dbaobj.cmd.Parameters.AddWithValue("@cid", cid);
            dbaobj.cmd.Parameters.AddWithValue("@weekNo", weekNo);
            SqlDataReader sdr = dbaobj.cmd.ExecuteReader();

            var correctAnswers = new Dictionary<int, string>();

            while (sdr.Read())
            {
                int questionId = int.Parse(sdr["id"].ToString());
                string correctOption = sdr["CorrectOption"].ToString();
                correctAnswers[questionId] = correctOption;
            }
            sdr.Close();
            dbaobj.CloseCon();

            // Calculate total marks based on answers
            foreach (var answer in model.answers)
            {
                int questionId = answer.Id;
                string selectedOption = answer.SelectedOption;

                if (correctAnswers.TryGetValue(questionId, out string correctOption))
                {
                    if (correctOption == selectedOption)
                    {
                        totalMarks += 1;
                    }
                }
            }

            // Insert the result into the Result table
            string insertResult = "INSERT INTO Result (cid, weekNo, sid, obtainMarks, timeTaken) VALUES (@cid, @weekNo, @sid, @obtainMarks, @timeTaken)";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(insertResult, dbaobj.con);
            dbaobj.cmd.Parameters.AddWithValue("@cid", cid);
            dbaobj.cmd.Parameters.AddWithValue("@weekNo", weekNo);
            dbaobj.cmd.Parameters.AddWithValue("@sid", Session["sid"]);
            dbaobj.cmd.Parameters.AddWithValue("@obtainMarks", totalMarks);
            dbaobj.cmd.Parameters.AddWithValue("@timeTaken", elapsedTime);
            dbaobj.cmd.ExecuteNonQuery();

            // Query to check if a quiz is uploaded for the current week
            string quizQuery = "SELECT COUNT(*) FROM Quiz WHERE cid = @cid AND weekNo = @weekNo";
            dbaobj.cmd = new SqlCommand(quizQuery, dbaobj.con);
            dbaobj.cmd.Parameters.AddWithValue("@cid", cid);
            dbaobj.cmd.Parameters.AddWithValue("@weekNo", weekNo);
            int totalques = (int)dbaobj.cmd.ExecuteScalar();

            //     if ((((double)totalMarks / totalques) * 100) > 50.0 && weekNo==16)
            if (weekNo == 16)

            {
                string qa = "update enroll set status='Completed' where cid='" + cid + "' and sid='" + Session["sid"] + "'";
                dbaobj.InsertUpdateDelete(qa);
                return RedirectToAction("SubmitRating", new { cid = cid });
            }
            dbaobj.CloseCon();

            // Redirect to a result or confirmation view
            return RedirectToAction("CourseDetailsStudent", new { id = cid });
        }

        public ActionResult ReAttemptQuiz(int cid, int weekNo)
        {

            string qq = "select * from Course where cid='" + cid + "'";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(qq, dbaobj.con);
            SqlDataReader sdr = dbaobj.cmd.ExecuteReader();
            if (sdr.Read() == true)
            {
                ViewBag.ctitle = sdr[1].ToString();

            }
            sdr.Close();
            dbaobj.CloseCon();

            QuizViewModel quiz = new QuizViewModel();
            string q = "select * from quiz where cid='" + cid + "' and weekNo='" + weekNo + "'";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            while (sdr1.Read())
            {
                Quiz qu = new Quiz();
                quiz.CourseId = int.Parse(sdr1["cid"].ToString());
                quiz.WeekNo = int.Parse(sdr1["weekNo"].ToString());
                qu.Id = int.Parse(sdr1["id"].ToString());
                //    qu.QuestionId= int.Parse(sdr1["questionId"].ToString());
                qu.quizTime = int.Parse(sdr1["quizTime"].ToString());
                qu.Question = sdr1["question"].ToString();
                qu.Option1 = sdr1["option1"].ToString();
                qu.Option2 = sdr1["option2"].ToString();
                qu.Option3 = sdr1["option3"].ToString();
                qu.Option4 = sdr1["option4"].ToString();
                qu.CorrectOption = sdr1["correctOption"].ToString();

                quiz.Questions.Add(qu);
            }
            sdr1.Close();
            dbaobj.CloseCon();
            if (quiz == null)
            {
                return HttpNotFound("Quiz not found.");
            }

            return View(quiz);
        }

        [HttpPost]
        public ActionResult ReSubmitQuiz(int cid, int weekNo, QuizViewModel model, string elapsedTime)
        {
            if (model == null)
            {
                // Handle the case where no answers are provided
                return RedirectToAction("CourseDetailsStudent", new { id = cid });
            }

            int totalMarks = 0; // To calculate total marks

            // Query to fetch the quiz questions and correct answers
            string q = "SELECT * FROM quiz WHERE cid = @cid AND weekNo = @weekNo";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            dbaobj.cmd.Parameters.AddWithValue("@cid", cid);
            dbaobj.cmd.Parameters.AddWithValue("@weekNo", weekNo);
            SqlDataReader sdr = dbaobj.cmd.ExecuteReader();

            var correctAnswers = new Dictionary<int, string>();

            while (sdr.Read())
            {
                int questionId = int.Parse(sdr["id"].ToString());
                string correctOption = sdr["CorrectOption"].ToString();
                correctAnswers[questionId] = correctOption;
            }
            sdr.Close();
            dbaobj.CloseCon();

            // Calculate total marks based on answers
            foreach (var answer in model.answers)
            {
                int questionId = answer.Id;
                string selectedOption = answer.SelectedOption;

                if (correctAnswers.TryGetValue(questionId, out string correctOption))
                {
                    if (correctOption == selectedOption)
                    {
                        totalMarks += 1;
                    }
                }
            }

            // Insert the result into the Result table
            string insertResult = "update Result set obtainMarks ='" + totalMarks + "' , timeTaken='" + elapsedTime + "' where cid=@cid and weekNo=@weekNo and sid=@sid";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(insertResult, dbaobj.con);
            dbaobj.cmd.Parameters.AddWithValue("@cid", cid);
            dbaobj.cmd.Parameters.AddWithValue("@weekNo", weekNo);
            dbaobj.cmd.Parameters.AddWithValue("@sid", Session["sid"]);
            dbaobj.cmd.Parameters.AddWithValue("@obtainMarks", totalMarks);
            dbaobj.cmd.ExecuteNonQuery();

            // Query to check if a quiz is uploaded for the current week
            string quizQuery = "SELECT COUNT(*) FROM Quiz WHERE cid = @cid AND weekNo = @weekNo";
            dbaobj.cmd = new SqlCommand(quizQuery, dbaobj.con);
            dbaobj.cmd.Parameters.AddWithValue("@cid", cid);
            dbaobj.cmd.Parameters.AddWithValue("@weekNo", weekNo);
            int totalques = (int)dbaobj.cmd.ExecuteScalar();
            if ((((double)totalMarks / totalques) * 100) > 50.0 && weekNo == 16)
            {
                string qa = "update enroll set status='Completed' where cid='" + cid + "' and sid='" + Session["sid"] + "'";
                dbaobj.InsertUpdateDelete(qa);
                return RedirectToAction("SubmitRating", new { cid = cid });
            }
            dbaobj.CloseCon();

            // Redirect to a result or confirmation view
            return RedirectToAction("CourseDetailsStudent", new { id = cid });
        }
        [HttpPost]
        public ActionResult SavePdfClick(string pdfUrl, int cid, int weekNo)
        {
            dbaobj.OpenCon();
            string q = "select * from readContent where cid='" + cid + "' and weekNo='" + weekNo + "' and sid='" + Session["sid"] + "'";
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            if (sdr1.Read())
            {
                sdr1.Close();
                string qq = "update readContent set pdfRead='1' where cid='" + cid + "' and weekNo='" + weekNo + "' and sid='" + Session["sid"] + "'";
                dbaobj.InsertUpdateDelete(qq);
            }
            else
            {
                sdr1.Close();
                string qq = "insert into readContent values('" + cid + "','" + weekNo + "','" + Session["sid"] + "','1','0')";
                dbaobj.InsertUpdateDelete(qq);
            }

            dbaobj.CloseCon();

            return RedirectToAction("CourseDetailsStudent", new { id = cid });
        }
        [HttpPost]
        public ActionResult SaveVideoClick(string videoUrl, int cid, int weekNo)
        {
            dbaobj.OpenCon();
            string q = "select * from readContent where cid='" + cid + "' and weekNo='" + weekNo + "' and sid='" + Session["sid"] + "'";
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            if (sdr1.Read())
            {
                sdr1.Close();
                string qq = "update readContent set videoRead='1' where cid='" + cid + "' and weekNo='" + weekNo + "' and sid='" + Session["sid"] + "'";
                dbaobj.InsertUpdateDelete(qq);
            }
            else
            {
                sdr1.Close();
                string qq = "insert into readContent values('" + cid + "','" + weekNo + "','" + Session["sid"] + "','0','1')";
                dbaobj.InsertUpdateDelete(qq);
            }

            dbaobj.CloseCon();
            return RedirectToAction("CourseDetailsStudent", new { id = cid });
        }
        [HttpGet]
        public ActionResult SubmitRating(int cid)
        {
            dbaobj.OpenCon();

            string qq = "SELECT * FROM Course WHERE cid='" + cid + "'";
            dbaobj.cmd = new SqlCommand(qq, dbaobj.con);
            SqlDataReader sdr1 = dbaobj.cmd.ExecuteReader();
            if (sdr1.Read())
            {
                ViewBag.ctitle = sdr1["c_title"].ToString();
            }
            ViewBag.cid = cid;

            sdr1.Close();
            dbaobj.CloseCon();
            return View();
        }

        [HttpPost]
        public ActionResult SubmitRating(int rating, int cid)
        {
            dbaobj.OpenCon();

            // Step 1: Insert the new rating
            string insertRatingQuery = "INSERT INTO Rating VALUES ('" + Session["sid"] + "', '" + cid + "', '" + rating + "')";

            dbaobj.InsertUpdateDelete(insertRatingQuery);
            // Step 2: Recalculate and update the new average rating in the Course table
            string updateCourseQuery = @"UPDATE Course SET c_rating = (SELECT AVG(Rating) FROM Rating WHERE Cid = '" + cid + "') WHERE cid = '" + cid + "'";

            dbaobj.InsertUpdateDelete(updateCourseQuery);
            @ViewBag.cid = cid;
            ViewBag.Message = "Thank you for your rating!";
            return View();
        }

        public ActionResult CourseCertificate(int cid)
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
                dbaobj.cmd.Parameters.AddWithValue("@sid", Session["sid"]);
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
            // Calculate percentage and grade
            double percentage = (obtainedMarks / (double)totalMarks) * 100;
            string grade = GetGrade(percentage);

            string pvq1 = " SELECT COUNT(*) FROM grade WHERE cid ='" + cid + "' AND sid = '" + Session["sid"] + "'";
            dbaobj.cmd = new SqlCommand(pvq1, dbaobj.con);
            int cc1 = (int)dbaobj.cmd.ExecuteScalar();
            if (cc1 == 0)
            {
                dbaobj.InsertUpdateDelete("insert into grade values('" + Session["sid"] + "','" + cid + "','" + totalMarks + "','" + obtainedMarks + "','" + percentage + "','" + grade + "')");
            }

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
        public ActionResult CourseResults()
        {
            List<Course> clist = new List<Course>();
            dbaobj.OpenCon();

            string q = "SELECT c.cid,c.c_title,c.c_category,c.c_description,c.c_image,c.c_hours, c.c_rating, c.c_keyword,t.tid, t.t_name, t.t_email, t.t_qualification FROM Course c INNER JOIN Enroll e ON c.cid = e.cid INNER JOIN  Allocation a ON c.cid = a.cid INNER JOIN teacher t ON a.tid = t.tid WHERE e.sid = '" + Session["sid"] + "' and e.status ='Completed'";

            dbaobj.cmd = new SqlCommand(q, dbaobj.con);

            // Use parameterized query to avoid SQL injection
            dbaobj.cmd.Parameters.AddWithValue("@sid", Session["sid"]);

            SqlDataReader sdr = dbaobj.cmd.ExecuteReader();
            while (sdr.Read())
            {
                Course c = new Course();
                c.cid = int.Parse(sdr["cid"].ToString());
                c.c_title = sdr["c_title"].ToString();
                c.c_category = sdr["c_category"].ToString();
                c.c_description = sdr["c_description"].ToString();
                c.c_image = sdr["c_image"].ToString();
                c.c_hours = int.Parse(sdr["c_hours"].ToString());
                c.c_rating = double.Parse(sdr["c_rating"].ToString());
                c.c_keyword = sdr["c_keyword"].ToString();
                c.teacher_id = int.Parse(sdr["tid"].ToString());
                c.teacher_name = sdr["t_name"].ToString();

                clist.Add(c);
            }

            sdr.Close();
            dbaobj.CloseCon();

            return View(clist);
        }
        [HttpGet]
        public ActionResult SearchWeek(string searchTerm)
        {

            var studentId = Session["sid"];
            List<int> favoriteWeeks = new List<int>();

            dbaobj.OpenCon();

            // Retrieve all favorite weeks for this course and student
            string getFavoriteWeeksQuery = "SELECT weekno FROM favouriteweek WHERE cid = @Cid AND sid = @StudentId";
            dbaobj.cmd = new SqlCommand(getFavoriteWeeksQuery, dbaobj.con);

            dbaobj.cmd.Parameters.AddWithValue("@Cid", Session["cid"]);
            dbaobj.cmd.Parameters.AddWithValue("@StudentId", studentId);

            using (var reader = dbaobj.cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    favoriteWeeks.Add(reader.GetInt32(0));  // Assuming weekno is the first column
                }
            }



            ViewBag.FavoriteWeeks = favoriteWeeks;

            List<Course> courses = new List<Course>();
            List<int> wlist = new List<int>();

            string query = @"
    SELECT DISTINCT p.weekNo
    FROM pdf p
   WHERE p.cid = @cid and
(@searchTerm IS NULL OR p.FileName LIKE '%' + @searchTerm + '%' OR p.keyword LIKE '%' + @searchTerm + '%' OR CAST(p.weekNo AS VARCHAR) LIKE '%' + @searchTerm + '%')
    UNION
    SELECT DISTINCT v.weekNo
    FROM videos v
WHERE v.cid = @cid and
     (@searchTerm IS NULL OR v.FileName LIKE '%' + @searchTerm + '%' OR v.Keyword LIKE '%' + @searchTerm + '%' OR CAST(v.weekNo AS VARCHAR) LIKE '%' + @searchTerm + '%')
    ORDER BY weekNo";

            SqlCommand cmd = new SqlCommand(query, dbaobj.con);
            cmd.Parameters.AddWithValue("@searchTerm", string.IsNullOrEmpty(searchTerm) ? (object)DBNull.Value : searchTerm);
            cmd.Parameters.AddWithValue("@cid", Session["cid"]);
            using (SqlDataReader sdr = cmd.ExecuteReader())
            {
                while (sdr.Read())
                {
                    wlist.Add(int.Parse(sdr["weekNo"].ToString()));
                }
            }
            ViewBag.loop = wlist.Count - 1;

            Dictionary<int, bool> quizUploaded = new Dictionary<int, bool>();
            Dictionary<int, List<Tuple<int, string, string>>> pdfPaths = new Dictionary<int, List<Tuple<int, string, string>>>();
            Dictionary<int, List<Tuple<int, string, string>>> videoPaths = new Dictionary<int, List<Tuple<int, string, string>>>();
            Dictionary<int, List<string>> quizAttemptedAndMarks = new Dictionary<int, List<string>>();
            Dictionary<int, int> totalQuestions = new Dictionary<int, int>();
            Dictionary<int, bool> readPdfvideo = new Dictionary<int, bool>();





            for (int i = 0; i <= wlist.Count - 1; i++)
            {
                // Initialize lists to store multiple PDFs and videos for each week
                List<Tuple<int, string, string>> pdfPathsForWeek = new List<Tuple<int, string, string>>();
                List<Tuple<int, string, string>> videoPathsForWeek = new List<Tuple<int, string, string>>();
                List<string> quizAttemptedAndMarksWeek = new List<string>();

                // Query to get all PDFs for the current week
                string pdfQuery = "SELECT path,FileName FROM pdf WHERE cid = @cid AND weekNo = @weekNo";
                dbaobj.cmd = new SqlCommand(pdfQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", Session["cid"]);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", wlist[i]);
                SqlDataReader pdfReader = dbaobj.cmd.ExecuteReader();


                // Read all PDF paths for the week
                while (pdfReader.Read())
                {
                    pdfPathsForWeek.Add(Tuple.Create(wlist[i], pdfReader["path"].ToString(), pdfReader["FileName"].ToString()));
                }
                pdfReader.Close();

                // Store the list of PDF paths and the upload status for the current week
                pdfPaths[i] = pdfPathsForWeek;

                // Query to get all video links for the current week
                string videoQuery = "SELECT videoPath,FileName FROM videos WHERE cid = @cid AND weekNo = @weekNo";
                dbaobj.cmd = new SqlCommand(videoQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", Session["cid"]);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", wlist[i]);
                SqlDataReader videoReader = dbaobj.cmd.ExecuteReader();

                // Read all video paths for the week
                while (videoReader.Read())
                {
                    videoPathsForWeek.Add(Tuple.Create(wlist[i], videoReader["videoPath"].ToString(), videoReader["FileName"].ToString()));
                }
                videoReader.Close();

                // Store the list of video paths for the current week
                videoPaths[i] = videoPathsForWeek;

                // Query to check if a quiz is uploaded for the current week
                string quizQuery = "SELECT COUNT(*) FROM Quiz WHERE cid = @cid AND weekNo = @weekNo";
                dbaobj.cmd = new SqlCommand(quizQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", Session["cid"]);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", wlist[i]);
                int quizCount = (int)dbaobj.cmd.ExecuteScalar();

                // Store the quiz upload status for the current week
                quizUploaded[i] = quizCount > 0;
                totalQuestions[i] = quizCount;

                // Query to check if a video and pdf is read for the current week
                string pvq = " SELECT COUNT(*) FROM ReadContent WHERE cid ='" + Session["cid"] + "' AND weekNo = '" + wlist[i] + "' and sid='" + Session["sid"] + "' and pdfRead=1 and videoRead=1";
                dbaobj.cmd = new SqlCommand(pvq, dbaobj.con);
                int cc = (int)dbaobj.cmd.ExecuteScalar();
                readPdfvideo[i] = cc > 0;


                // Query to get all quiz result for the current week
                string q_mQuery = "SELECT obtainMarks FROM result WHERE cid = @cid AND weekNo = @weekNo And sid=@sid";
                dbaobj.cmd = new SqlCommand(q_mQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@cid", Session["cid"]);
                dbaobj.cmd.Parameters.AddWithValue("@weekNo", wlist[i]);
                dbaobj.cmd.Parameters.AddWithValue("@sid", Session["sid"]);
                SqlDataReader sdr = dbaobj.cmd.ExecuteReader();

                while (sdr.Read())
                {
                    quizAttemptedAndMarksWeek.Add(sdr["obtainMarks"].ToString());
                }
                sdr.Close();
                quizAttemptedAndMarks[i] = quizAttemptedAndMarksWeek;

            }

            dbaobj.CloseCon();

            // Pass the dictionaries to the view via ViewBag
            ViewBag.QuizUploaded = quizUploaded;
            ViewBag.TotalQuestions = totalQuestions;
            ViewBag.PdfPaths = pdfPaths;
            ViewBag.VideoPaths = videoPaths;
            ViewBag.QuizAttemptedAndMarks = quizAttemptedAndMarks;
            ViewBag.ReadPdfVideo = readPdfvideo;

            Course c = new Course();
            string q = "select * from Course where cid=@cid";
            dbaobj.OpenCon();
            dbaobj.cmd = new SqlCommand(q, dbaobj.con);
            dbaobj.cmd.Parameters.AddWithValue("@cid", Session["cid"]);
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


            return PartialView("_CourseWeeks", ps);
        }

        [HttpPost]
        public JsonResult ToggleFavoriteWeek(int weekNo, int cid)
        {
            var studentId = Session["sid"];


            dbaobj.OpenCon();

            // Check if the week is already marked as favorite
            string checkFavoriteQuery = "SELECT COUNT(*) FROM favouriteweek WHERE weekno = @WeekNo AND cid = @Cid AND sid = @StudentId";
            dbaobj.cmd = new SqlCommand(checkFavoriteQuery, dbaobj.con);
            dbaobj.cmd.Parameters.AddWithValue("@WeekNo", weekNo);
            dbaobj.cmd.Parameters.AddWithValue("@Cid", cid);
            dbaobj.cmd.Parameters.AddWithValue("@StudentId", studentId);

            int favoriteCount = (int)dbaobj.cmd.ExecuteScalar();

            if (favoriteCount > 0)
            {
                // If already favorite, remove it
                string deleteFavoriteQuery = "DELETE FROM favouriteweek WHERE weekno = @WeekNo AND cid = @Cid AND sid = @StudentId";
                dbaobj.cmd = new SqlCommand(deleteFavoriteQuery, dbaobj.con);
                dbaobj.cmd.Parameters.AddWithValue("@WeekNo", weekNo);
                dbaobj.cmd.Parameters.AddWithValue("@Cid", cid);
                dbaobj.cmd.Parameters.AddWithValue("@StudentId", studentId);
                dbaobj.cmd.ExecuteNonQuery();


                return Json(new { isFavorite = false });
            }
            else
            {
                // If not favorite, add it
                string insertFavoriteQuery = "INSERT INTO favouriteweek (weekno, cid, sid, isfavourite) VALUES (@WeekNo, @Cid, @StudentId, 1)";
                dbaobj.cmd = new SqlCommand(insertFavoriteQuery, dbaobj.con);

                dbaobj.cmd.Parameters.AddWithValue("@WeekNo", weekNo);
                dbaobj.cmd.Parameters.AddWithValue("@Cid", cid);
                dbaobj.cmd.Parameters.AddWithValue("@StudentId", studentId);
                dbaobj.cmd.ExecuteNonQuery();


                return Json(new { isFavorite = true });
            }


        }

    }
}