using StuFeeWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ELearning.Models
{


    public class Teacher
    {

        [Display(Name = "Teacher Id")]
        public int tid { get; set; }

        [Required(ErrorMessage = "Teacher Name is required")]
        [Display(Name = "Teacher Name")]
        public string tname { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string tpassword { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string temail { get; set; }
        [Required(ErrorMessage = "Qualification is required")]
        [Display(Name = "Qualification")]
        public string tqualification { get; set; }


        public static List<Teacher> TeachersList()
        {
            DBAccess obj = new DBAccess();
            List<Teacher> slist = new List<Teacher>();
            obj.OpenCon();
            string q = "Select * from teacher";
            obj.cmd = new SqlCommand(q, obj.con);
            SqlDataReader sdr = obj.cmd.ExecuteReader();
            while (sdr.Read())
            {
                Teacher s = new Teacher();
                s.tid = int.Parse(sdr[0].ToString());
                s.tname = sdr[1].ToString();
                s.tpassword = sdr[2].ToString();
                s.temail = sdr[3].ToString();
                s.tqualification = sdr[4].ToString();

                slist.Add(s);
            }
            sdr.Close();
            obj.CloseCon();
            return slist;
        }
    }

}