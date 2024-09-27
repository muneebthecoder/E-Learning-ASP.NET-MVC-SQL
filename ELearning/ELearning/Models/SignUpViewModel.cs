using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class SignUpViewModel
    {
        public string UserRole { get; set; }

        // Student properties
        [Display(Name = "Student Id")]
        public string sid { get; set; }

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
        // Add other student-specific properties

        // Teacher properties
        [Display(Name = "Teacher Id")]
        public string tid { get; set; }

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
        // Add other teacher-specific properties
    }
}