using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class Videos
    {
        public int Id { get; set; }  // Corresponds to [id] column
        public string VideoTitle { get; set; }  // Corresponds to [videoTitle] column
        public string VideoPath { get; set; }  // Corresponds to [videoPath] column
        public int WeekNo { get; set; }  // Corresponds to [weekNo] column
        public int Cid { get; set; }  // Corresponds to [cid] column (Foreign Key to Course)
    }
}