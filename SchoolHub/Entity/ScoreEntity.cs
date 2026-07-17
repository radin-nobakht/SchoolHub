using System.ComponentModel.DataAnnotations;

namespace SchoolHub.Entity
{
    public class ScoreEntity
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public string Score { get; set; }
    }
}
