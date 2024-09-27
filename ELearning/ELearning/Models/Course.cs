using StuFeeWebApp.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class Course
    {
        [Display(Name = "Course ID")]
        public int cid { get; set; }

        [Required(ErrorMessage = "Course Title is required")]
        [Display(Name = "Course Title")]
        [StringLength(100, ErrorMessage = "Course Title cannot be longer than 100 characters")]
        public string c_title { get; set; }

        [Required(ErrorMessage = "Course Category is required")]
        [Display(Name = "Course Category")]
        [StringLength(50, ErrorMessage = "Course Category cannot be longer than 50 characters")]
        public string c_category { get; set; }

        [Display(Name = "Course Description")]
        [StringLength(500, ErrorMessage = "Course Description cannot be longer than 500 characters")]
        public string c_description { get; set; }

        [Display(Name = "Course Image")]
   //     [StringLength(255, ErrorMessage = "Course Image URL cannot be longer than 255 characters")]
        public string c_image { get; set; }
        public HttpPostedFileBase ufile { get; set; }

        [Required(ErrorMessage = "Course Hours is required")]
        [Display(Name = "Course Hours")]
        public int c_hours { get; set; }

        [Display(Name = "Course Rating")]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double c_rating { get; set; }

        [Required(ErrorMessage = "Keyword is required")]
        [Display(Name = "Course Keyword")]
        public string c_keyword { get; set; }

        public string teacher_name { get; set; }
        public int teacher_id { get; set; }
        public double progress { get; set; }


        public static List<Course> CoursesList()
        {
            DBAccess obj = new DBAccess();
            List<Course> clist = new List<Course>();
            obj.OpenCon();
            string q = "SELECT * FROM Course"; // Ensure the table name is correct
            obj.cmd = new SqlCommand(q, obj.con);
            SqlDataReader sdr = obj.cmd.ExecuteReader();
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

                clist.Add(c);
            }
            sdr.Close();
            obj.CloseCon();
            return clist;
        }
        }
}