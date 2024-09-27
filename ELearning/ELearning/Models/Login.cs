using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Email")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public string UserRole { get; set; }
    }
}