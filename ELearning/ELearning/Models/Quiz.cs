using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ELearning.Models
{
    public class QuizViewModel
    {
        public int WeekNo { get; set; }
        public int CourseId { get; set; }
        public List<Quiz> Questions { get; set; } = new List<Quiz>();
        public List<QuestionViewModel> answers { get; set; } = new List<QuestionViewModel>();
    //    public Dictionary<int, string> answers { get; set; } = new Dictionary<int, string>()

    }
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public string SelectedOption { get; set; }
    }
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

      //  public int QuestionId { get; set; }

        [Required]
        public int WeekNo { get; set; }

        [Required]
        public int Cid { get; set; } // Course ID
        [Display(Name = "Question")]

        [Required(ErrorMessage = "Question is required.")]
        [StringLength(500, ErrorMessage = "Question cannot exceed 500 characters.")]
        public string Question { get; set; }
        [Display(Name = "Option 1")]
        [Required(ErrorMessage = "Option 1 is required.")]
        [StringLength(100, ErrorMessage = "Option 1 cannot exceed 100 characters.")]
        public string Option1 { get; set; }
        [Display(Name = "Option 2")]
        [Required(ErrorMessage = "Option 2 is required.")]
        [StringLength(100, ErrorMessage = "Option 2 cannot exceed 100 characters.")]
        public string Option2 { get; set; }
        [Display(Name = "Option 3")]
        [Required(ErrorMessage = "Option 3 is required.")]
        [StringLength(100, ErrorMessage = "Option 3 cannot exceed 100 characters.")]
        public string Option3 { get; set; }
        [Display(Name = "Option 4")]
        [Required(ErrorMessage = "Option 4 is required.")]
        [StringLength(100, ErrorMessage = "Option 4 cannot exceed 100 characters.")]
        public string Option4 { get; set; }
        [Display(Name = "Correct Option")]
        [Required(ErrorMessage = "Correct Option is required.")]
        [StringLength(100, ErrorMessage = "Correct Option cannot exceed 100 characters.")]
        public string CorrectOption { get; set; }
        public int quizTime { get; set; }
    }
}