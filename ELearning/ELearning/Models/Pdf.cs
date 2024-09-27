using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class Pdf
    {
        public int Id { get; set; }  // Corresponds to [id] column
        public string Title { get; set; }  // Corresponds to [title] column
        public string Path { get; set; }  // Corresponds to [path] column
        public int WeekNo { get; set; }  // Corresponds to [weekNo] column
        public int Cid { get; set; }  // Corresponds to [cid] column (Foreign Key to Course)
    }
}