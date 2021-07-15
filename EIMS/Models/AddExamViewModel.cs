using System.ComponentModel.DataAnnotations;
using EIMS.Domain.Entities.Enum;

namespace EIMS.Models
{
    public class AddExamViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Examination Name")]
        [MaxLength(50, ErrorMessage = "Maximum of 30 characters")]
        public string ExaminationName { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Examination Code")]
        [MaxLength(10, ErrorMessage = "Maximum of 10 characters")]
        public string ExaminationCode { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Exam Category")]
        public SchoolCategory ExamCategory { get; set; }
    }
}
