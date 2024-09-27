using StuFeeWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ELearning.Models
{
    public class AllocateCourse
    {
        public int aId { get; set; }
        
            [Required]
            public int TeacherId { get; set; }

            [Required]
            public int CourseId { get; set; }

        public static List<AllocateCourse> AllocatedCourses()
        {
            DBAccess obj = new DBAccess();
            List<AllocateCourse> slist = new List<AllocateCourse>();
            obj.OpenCon();
            string q = "Select * from Allocation";
            obj.cmd = new SqlCommand(q, obj.con);
            SqlDataReader sdr = obj.cmd.ExecuteReader();
            while (sdr.Read())
            {
                AllocateCourse s = new AllocateCourse();
                s.aId = int.Parse(sdr[0].ToString());
                s.TeacherId = int.Parse(sdr[1].ToString());
                s.CourseId = int.Parse(sdr[2].ToString());
                slist.Add(s);
            }
            sdr.Close();
            obj.CloseCon();
            return slist;
        }

    }
}