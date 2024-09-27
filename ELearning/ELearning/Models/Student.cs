using StuFeeWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class Student
    {
        [Display(Name = "Student Id")]
        public int sid { get; set; }

        [Required(ErrorMessage = "Student Name is required")]
        [Display(Name = "Student Name")]
        public string sname { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string spassword { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string semail { get; set; }
        [Required(ErrorMessage = "Interset is required")]
        [Display(Name = "Interset")]
        public string sInterset { get; set; }
        public int obtainMarks { get; set; }
        public string grade { get; set; }


        public static List<Student> StudentList()
        {
            DBAccess obj = new DBAccess();
            List<Student> slist = new List<Student>();
            obj.OpenCon();
            string q = "Select * from student";
            obj.cmd = new SqlCommand(q, obj.con);
            SqlDataReader sdr = obj.cmd.ExecuteReader();
            while (sdr.Read())
            {
                Student s = new Student();
                s.sid = int.Parse(sdr[0].ToString());
                s.sname = sdr[1].ToString();
                s.semail = sdr[2].ToString();
                s.spassword = sdr[3].ToString();
                s.sInterset = sdr[4].ToString();

                slist.Add(s);
            }
            sdr.Close();
            obj.CloseCon();
            return slist;
        }
    }
}