using ELearning.Models;
using StuFeeWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELearning.Controllers
{
    public class TeacherController : Controller
    {
        // GET: Teacher
        DBAccess dbaobj=new DBAccess();
        public ActionResult DashBoard()
        {
            ViewBag.HideHeader = true;
            List<Course> courses = new List<Course>();
            List<Course> c = Course.CoursesList();
            List<AllocateCourse> a = AllocateCourse.AllocatedCourses();
            foreach (AllocateCourse aa in a) {
                if (Session["tid"].ToString() == aa.TeacherId.ToString()) 
                {
                    courses.Add(   c.Where(s => s.cid == aa.CourseId).First());
                }
                    }

            return View(courses);

        }
        public ActionResult TeacherList()
        {
            ViewBag.HideHeader = true;
            List<Teacher> teachers = Teacher.TeachersList();

            return View(teachers);

        }
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Login", "LoginSignup");
        }
    }
}