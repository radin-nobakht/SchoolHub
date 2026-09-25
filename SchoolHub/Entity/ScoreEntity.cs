using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SchoolHub.Entity
{
    public class ScoreEntity
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        public string Score { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public bool Status { get; set; } 

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int GeneralSubjectId { get; set; }
    }
}
