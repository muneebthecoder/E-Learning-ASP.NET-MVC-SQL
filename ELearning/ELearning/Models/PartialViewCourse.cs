using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class PartialViewCourse
    {
        public Course course {get;set;}
        public Pdf pdfs { get; set; }
        public Videos videos { get; set; }

    }
}